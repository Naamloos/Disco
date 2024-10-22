using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Disco
{
    internal class ClientModhandler
    {
        private ElectronDebugger _debugger;
        private FileSystemWatcher _watcher;
        private string _path;

        public ClientModhandler(ElectronDebugger debugger, string path)
        {
            _path = path;
            remakeFile();
            _debugger = debugger;
            _watcher = new FileSystemWatcher(Path.GetDirectoryName(path)!, Path.GetFileName(path));
            Console.WriteLine("Created file system watcher for style.css");
        }

        public async Task StartAsync()
        {
            LogToConsole("Helloooo Discord! 😈");
            injectStyle();
            injectExperiments();

            _watcher.Deleted += (sender, e) => { remakeFile(); };
            _watcher.Changed += onFileChange;

            _watcher.EnableRaisingEvents = true;
            _watcher.NotifyFilter = NotifyFilters.LastWrite;

            SendNotification("Disco External Mod", "Disco (external client mod) now running!");

            await Task.Delay(-1);
        }

        private void remakeFile()
        {
            if (!File.Exists(_path))
            {
                File.Create(_path).Close();
            }
        }

        private void onFileChange(object sender, FileSystemEventArgs e)
        {
            injectStyle();
        }

        private void injectStyle()
        {
            _debugger.SendJavascript(loadJs("Disco.Javascript.Patcher.js", loadStyle()));
            LogToConsole("Injected DISCO update into Discord! <3");
        }

        private void injectExperiments()
        {
            _debugger.SendJavascript(loadJs("Disco.Javascript.Experiments.js"));
            LogToConsole("Enabled experiments!");
        }

        public void LogToConsole(string message)
        {
            _debugger.SendJavascript(loadJs("Disco.Javascript.Logger.js", message));
            Console.WriteLine(message);
        }

        public void SendNotification(string title, string message)
        {
            _debugger.SendJavascript(loadJs("Disco.Javascript.SendNotification.js", title, message));
        }

        private string loadJs(string resource, params string[] substitute)
        {
            var str = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            using var reader = new StreamReader(str!);
            var js = reader.ReadToEnd();

            for(int i = 0; i < substitute.Length; i++)
            {
                js = js.Replace("{{{" + i + "}}}", substitute[i]);
            }

            return js;
        }

        private string loadStyle()
        {
            using var file = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(file);
            var text = reader.ReadToEnd();
            // load file from resources
            var str = Assembly.GetExecutingAssembly().GetManifestResourceStream("Disco.Css.baseStyle.css");
            using var reader2 = new StreamReader(str!);
            var baseStyle = reader2.ReadToEnd();
            return baseStyle + "\n\n\n\n" + text;
        }

        private const string injectKeyword = "INJECT";
    }
}
