using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Disco.Services
{
    public class StyleListener
    {
        private ElectronDebugger _debugger;
        private JavascriptLoader _javascriptLoader;
        private FileSystemWatcher _watcher;
        private ILogger _logger;
        private string _path;

        public StyleListener(ElectronDebugger debugger, JavascriptLoader javascriptLoader, 
            ILogger<StyleListener> logger)
        {
            _debugger = debugger;
            _javascriptLoader = javascriptLoader;
            _logger = logger;
            _path = Path.Combine(Directory.GetCurrentDirectory(), "style.css");

            remakeFile();

            _watcher = new FileSystemWatcher(Path.GetDirectoryName(_path)!, Path.GetFileName(_path));
        }

        public void Start()
        {
            updateStyle();

            _watcher.Deleted += (sender, e) => { remakeFile(); };
            _watcher.Changed += onFileChange;

            _watcher.EnableRaisingEvents = true;
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _logger.LogInformation("StyleListener is now running and ready for Hot Reloading! ✨");
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
        }

        private void updateStyle()
        {
            _javascriptLoader.SendStyle(loadStyle());
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
            updateStyle();
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
