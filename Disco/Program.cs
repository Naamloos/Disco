using System.Reflection;

namespace Disco
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var str = Assembly.GetExecutingAssembly().GetManifestResourceStream("Disco.IntroText.txt");
            using var reader = new StreamReader(str!);
            var logo = reader.ReadToEnd();

            Console.WriteLine(logo);

            var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord");
            var appDirs = Directory.GetDirectories(basePath).Where(x => x.Contains("app-"));
            if(appDirs.Any())
            {
                var discordAppDir = "";
                foreach(var dir in appDirs)
                {
                    if(Directory.GetFiles(dir, "Discord.exe").Any())
                    {
                        discordAppDir = dir;
                        break;
                    }
                    Console.WriteLine("Found a Discord at " + dir);
                }
                if(discordAppDir == "")
                {
                    Console.WriteLine("Could not find a Discord executable in any of the Discord app directories 😔🤘");
                    return;
                }
                var appDir = Path.Combine(discordAppDir, "Discord.exe");
                Console.WriteLine("Found a Discord stable at " + appDir);
                var debugger = new ElectronDebugger(30069, appDir);

                await debugger.LaunchAsync();

                string path = Path.Combine(Directory.GetCurrentDirectory(), "style.css");

                var styler = new ClientModhandler(debugger, path);
                await styler.StartAsync();
            }

            Console.WriteLine("Could not find Discord 😔🤘");
        }
    }
}