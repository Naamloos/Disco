using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Disco.Services
{
    public class DiscoHostService : IHostedService
    {
        private ElectronDebugger _electronDebugger;
        private StyleListener _styleListener;
        private JavascriptLoader _javascriptLoader;
        private ILogger<DiscoHostService> _logger;
        private DiscoNativeProxy _discoNativeProxy;

        public DiscoHostService(ILogger<DiscoHostService> logger, ElectronDebugger electronDebugger, 
            StyleListener styleListener, JavascriptLoader javascriptLoader, DiscoNativeProxy discoNativeProxy)
        {
            _electronDebugger = electronDebugger;
            _styleListener = styleListener;
            _javascriptLoader = javascriptLoader;
            _logger = logger;
            _discoNativeProxy = discoNativeProxy;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var str = Assembly.GetExecutingAssembly().GetManifestResourceStream("Disco.IntroText.txt");
            using var reader = new StreamReader(str!);
            var logo = reader.ReadToEnd();
            _logger.LogInformation(logo);

            await _electronDebugger.LaunchAsync();

            _javascriptLoader.Preload();
            _javascriptLoader.InjectDiscoPatcher();
            _javascriptLoader.InjectExperiments();
            _javascriptLoader.InjectExtendedSelectors();

            _styleListener.Start();

            _logger.LogInformation("Disco is now running! ✨");
            _javascriptLoader.CreateNotification("Disco", "Disco is now running! ✨");

            _javascriptLoader.SendPatcherResponse("DISCO_DEBUG", "{\"value\": 0}");
            
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Disco is shutting down... bai bai! 😊");
            _styleListener.Stop();
            _electronDebugger.Stop();
            _logger.LogInformation("Disco has shut down.");
        }
    }
}
