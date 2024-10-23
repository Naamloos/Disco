using Disco.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Disco.Services
{
    public class DiscoNativeProxy
    {
        private ElectronDebugger _electronDebugger;
        private ILogger _logger;
        private JavascriptLoader _javascriptLoader;

        public DiscoNativeProxy(ElectronDebugger electronDebugger, JavascriptLoader javascriptLoader, ILogger<DiscoNativeProxy> logger)
        {
            this._electronDebugger = electronDebugger;
            this._logger = logger;
            this._javascriptLoader = javascriptLoader;

            this._electronDebugger.OnMessageReceived += messageReceived;
        }

        private void messageReceived(DebuggerIncomingPayload payload)
        {
            if(payload.Method == "Runtime.consoleAPICalled")
            {
                if(payload.Params == null)
                {
                    return;
                }
                var requestPayload = JsonSerializer.Deserialize<ConsoleApiCalledParams>(payload.Params);
                var foundPayload = requestPayload.Args.FirstOrDefault(x => x.Value.StartsWith("!__DISCO "));
                if(foundPayload == default)
                {
                    return;
                }

                var jsonData = foundPayload.Value.Substring(9);
                var jsonObj = JsonSerializer.Deserialize<JsonObject>(jsonData);
                _logger.LogInformation("Received Disco message: {0}", jsonData);
                // TODO remove pingback for test
                _javascriptLoader.SendPatcherResponse(jsonObj["id"].AsValue().GetValue<string>(), jsonData);
            }
        }
    }
}
