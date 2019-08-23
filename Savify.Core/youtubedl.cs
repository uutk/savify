using System.Diagnostics;

namespace Savify.Core
{
    public static class Youtubedl
    {
        private static Process GetProcess(string path, string args)
        {
            Process _youtubedl = new Process();
            _youtubedl.StartInfo.UseShellExecute = false;
            _youtubedl.StartInfo.FileName = path;
            _youtubedl.StartInfo.Arguments = args;
            _youtubedl.StartInfo.CreateNoWindow = true;
            return _youtubedl;
        }

        public static void Run(string args)
        {
            Process _youtubedl = GetProcess(Settings.Default.YouTubeDl, args);
            _youtubedl.Start();
            _youtubedl.WaitForExit();
            _youtubedl.Close();
        }

        public static void Update()
        {
            Process _youtubedl = GetProcess(Settings.Default.YouTubeDl, "-U");
            _youtubedl.Start();
            _youtubedl.WaitForExit();
            _youtubedl.Close();
        }
    }
}
