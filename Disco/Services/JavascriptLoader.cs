using Disco.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Disco.Services
{
    public class JavascriptLoader
    {
        private Dictionary<string, string> loadedScripts = new Dictionary<string, string>();
        private ElectronDebugger _debugger;
        private ILogger _logger;

        public JavascriptLoader(ElectronDebugger debugger, ILogger<JavascriptLoader> logger)
        {
            _debugger = debugger;
            _logger = logger;
        }

        public void Preload()
        {
            foreach (var name in Assembly.GetExecutingAssembly().GetManifestResourceNames().ToList())
            {
                if (name.StartsWith("Disco.Javascript.") && name.EndsWith(".js"))
                {
                    var scriptName = name.Replace("Disco.Javascript.", "").Replace(".js", "");

                    var str = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
                    using var reader = new StreamReader(str!);
                    var js = reader.ReadToEnd();

                    loadedScripts.Add(scriptName, js);
                }
            }

            string scripts = string.Join(", ", loadedScripts.Keys);
            _logger.LogInformation("Preloaded scripts: {0}", scripts);
        }

        public void SendStyle(string css)
        {
            InjectScript("StylePatcher", css);
        }

        public void CreateNotification(string title, string body)
        {
            InjectScript("SendNotification", title, body);
        }

        public void LogToElectronConsole(string message)
        {
            InjectScript("Logger", message);
        }

        public void InjectExperiments()
        {
            InjectScript("Experiments");
        }

        public void InjectExtendedSelectors()
        {
            InjectScript("ExtendedSelectors");
        }

        public void InjectScript(string scriptName, params string[] substitutes)
        {
            if (loadedScripts.ContainsKey(scriptName))
            {
                var js = loadedScripts[scriptName];

                for (int i = 0; i < substitutes.Length; i++)
                {
                    js = js.Replace("{{{" + i + "}}}", substitutes[i]);
                }

                var payload = new DebuggerPayload()
                {
                    Params = new DebuggerParams()
                    {
                        Expression = js
                    }
                };

                _debugger.SendPayload(payload);
            }
            else
            {
                _logger.LogError("Script {0} not found in preloaded scripts", scriptName);
            }
        }
    }
}
