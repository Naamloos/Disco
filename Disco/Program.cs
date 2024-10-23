using Disco.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Runtime.ConstrainedExecution;

namespace Disco
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ElectronDebugger>();
                    services.AddSingleton<JavascriptLoader>();
                    services.AddSingleton<StyleListener>();
                    services.AddHostedService<DiscoHostService>();
                    services.AddLogging();
                })
                .Build();

            host.Run();
        }
    }
}