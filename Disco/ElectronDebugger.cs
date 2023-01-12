using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Disco.Entities;
using Websocket.Client;

namespace Disco
{
    internal class ElectronDebugger
    {
        private ushort _port;
        private string _discordPath;
        private bool _killExisting;
        private WebsocketClient? _websocket;

        public ElectronDebugger(ushort port, string discordPath, bool killExisting = true)
        {
            _port = port;
            _discordPath = discordPath;
            _killExisting = killExisting;
        }

        public async Task LaunchAsync()
        {
            string name = Path.GetFileName(_discordPath);
            string directory = Path.GetDirectoryName(_discordPath) ?? "";

            if (_killExisting)
            {
                Console.WriteLine($"Killing Existing Discords: Processname is {name.Replace(".exe", "")}");
                foreach(var discord in Process.GetProcessesByName(name.Replace(".exe", "")))
                {
                    discord.Kill();
                }
            }

            Console.WriteLine($"Launching a new Discord with debugger on port {_port}");
            // Force Discord to launch a debugger
            var startInfo = new ProcessStartInfo()
            {
                Arguments = $"/C start {_discordPath} --remote-debugging-port={_port}",
                FileName = "cmd",
                UseShellExecute = true
            };

            var proc = Process.Start(startInfo);

            var jsonUri = $"http://localhost:{_port}/json";
            
            Console.WriteLine($"Fetching debugger websocket URI from {jsonUri}");
            
            string websocketUri = await WaitForDebugUrlAsync(jsonUri);

            _websocket = new WebsocketClient(new Uri(websocketUri));

            await _websocket.Start();
            Console.WriteLine("Started websocket connection with Discord!");
        }

        private async Task<string> WaitForDebugUrlAsync(string jsonUri)
        {
            Console.WriteLine("Waiting for useable websocket debugger url");
            string debuggerUrl = "";

            while(string.IsNullOrEmpty(debuggerUrl))
            {
                await Task.Delay(500);

                using HttpClient http = new HttpClient();

                var data = await http.GetAsync(jsonUri);
                var json = await data.Content.ReadAsStringAsync();

                var responses = JsonSerializer.Deserialize<DebuggerJsonResponse[]>(json);

                if (responses != null)
                {
                    foreach (var resp in responses)
                    {
                        if (resp.Url.Contains("discord.com/channels/"))
                        {
                            debuggerUrl = resp.WebsocketDebuggerUrl;
                            break;
                        }
                    }
                }
            }

            Console.WriteLine($"Found useable websocket debugger url! {debuggerUrl}");

            return debuggerUrl;
        }

        public void SendJavascript(string js)
        {
            var payload = new DebuggerPayload()
            {
                Params = new DebuggerParams()
                {
                    Expression = js
                }
            };

            var stringPayload = JsonSerializer.Serialize(payload);
            _websocket?.Send(stringPayload);
        }
    }
}
