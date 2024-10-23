using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Disco.Entities;
using Microsoft.Extensions.Logging;
using Websocket.Client;

namespace Disco.Services
{
    public class ElectronDebugger
    {
        private const ushort PORT = 30069;
        private WebsocketClient? _websocket;
        private readonly ILogger _logger;

        // event for receiving DebuggerIncomingPayload
        public event OnMessageReceivedDelegate? OnMessageReceived;
        public delegate void OnMessageReceivedDelegate(DebuggerIncomingPayload payload);

        public ElectronDebugger(ILogger<ElectronDebugger> logger)
        {
            _logger = logger;
        }

        public async Task LaunchAsync()
        {
            var discordDirectory = findDiscord();
            if (discordDirectory == null)
            {
                throw new Exception("Failed to find Discord! 😭");
            }

            string name = Path.GetFileName(discordDirectory);
            string directory = Path.GetDirectoryName(discordDirectory) ?? "";

            _logger.LogInformation("Killing Existing Discords: Processname is {0}", name);
            foreach (var discord in Process.GetProcessesByName(name.Replace(".exe", "")))
            {
                discord.Kill();
            }

            _logger.LogInformation("Launching a new Discord with debugger on port {0}", PORT);
            // Force Discord to launch a debugger
            var startInfo = new ProcessStartInfo()
            {
                Arguments = $"/C start {discordDirectory} --remote-debugging-port={PORT}",
                FileName = "cmd",
                UseShellExecute = true
            };

            var proc = Process.Start(startInfo);

            var jsonUri = $"http://localhost:{PORT}/json";

            _logger.LogInformation("Fetching debugger websocket URI from {0}", jsonUri);

            string websocketUri = await WaitForDebugUrlAsync(jsonUri);

            _websocket = new WebsocketClient(new Uri(websocketUri));

            _websocket.MessageReceived.Subscribe(receiveMessage);

            await _websocket.Start();
            _logger.LogInformation("Started websocket connection with Discord! ✨");
            SendPayload(new DebuggerPayload()
            {
                Method = "Runtime.enable"
            });
        }

        public void Stop()
        {
            _websocket?.Stop(WebSocketCloseStatus.NormalClosure, "");
            _websocket?.Dispose();
        }

        private string? findDiscord()
        {
            // Trying to find Discord in the user's AppData
            var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord");
            var appDirs = Directory.GetDirectories(basePath).Where(x => x.Contains("app-"));

            string? discordLocation = null;
            if (appDirs.Any())
            {
                foreach (var dir in appDirs)
                {
                    // if the directory contains Discord.exe, we found it
                    if (Directory.GetFiles(dir, "Discord.exe").Any())
                    {
                        discordLocation = dir;
                        break;
                    }
                }
            }

            return discordLocation != null ? Path.Combine(discordLocation, "Discord.exe") : null;
        }

        private async Task<string> WaitForDebugUrlAsync(string jsonUri)
        {
            _logger.LogInformation("Waiting for useable websocket debugger url");
            string debuggerUrl = "";

            while (string.IsNullOrEmpty(debuggerUrl))
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

            _logger.LogInformation("Found useable websocket debugger url! {0}", debuggerUrl);

            return debuggerUrl;
        }

        public void SendPayload(DebuggerPayload payload)
        {
            _logger.LogInformation("Pushing payload {0} with ID {1}", payload.Method, payload.Id);
            var stringPayload = JsonSerializer.Serialize(payload);
            _websocket?.Send(stringPayload);
        }

        private void receiveMessage(ResponseMessage message)
        {
            var payload = JsonSerializer.Deserialize<DebuggerIncomingPayload>(message.Text);
             OnMessageReceived?.Invoke(payload);
        }
    }
}
