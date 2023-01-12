using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Disco
{
    internal class StylesheetListener
    {
        private ElectronDebugger _debugger;
        private FileSystemWatcher _watcher;
        private string _path;

        public StylesheetListener(ElectronDebugger debugger, string path)
        {
            _path = path;
            remakeFile();
            _debugger = debugger;
            _watcher = new FileSystemWatcher(Path.GetDirectoryName(path)!, Path.GetFileName(path));
            Console.WriteLine("Created file system watcher for style.css");
        }

        public async Task StartAsync()
        {
            logConsole("Helloooo Discord! 😈");
            injectStyle();
            injectExperiments();

            _watcher.Deleted += (sender, e) => { remakeFile(); };
            _watcher.Changed += onFileChange;

            _watcher.EnableRaisingEvents = true;
            _watcher.NotifyFilter = NotifyFilters.LastWrite;

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
            logConsole("Injected DISCO update into Discord! <3");
        }

        private void injectExperiments()
        {
            _debugger.SendJavascript(loadJs("Disco.Javascript.Experiments.js"));
            logConsole("Enabled experiments!");
        }

        private void logConsole(string message)
        {
            _debugger.SendJavascript(loadJs("Disco.Javascript.Logger.js", message));
            Console.WriteLine(message);
        }

        private string loadJs(string resource, string? substitute = null)
        {
            var str = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            using var reader = new StreamReader(str!);
            var js = reader.ReadToEnd();
            return substitute != null? js.Replace(injectString, substitute) : js;
        }

        private string loadStyle()
        {
            using var file = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(file);
            var text = reader.ReadToEnd();
            return text;
        }

        private const string injectString = "{{INJECT}}";
    }
}
