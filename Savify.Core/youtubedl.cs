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
            _youtubedl.StartInfo.RedirectStandardOutput = true;
            _youtubedl.StartInfo.CreateNoWindow = true;
            return _youtubedl;
        }

        public static string Run(string args)
        {
            string output;
            Process _youtubedl = GetProcess(Settings.Default.YouTubeDl, args);
            _youtubedl.Start();
            _youtubedl.WaitForExit();
            output = _youtubedl.StandardOutput.ReadToEnd();
            _youtubedl.Close();

            return output;
        }

        public static string Update()
        {
            string output;
            Process _youtubedl = GetProcess(Settings.Default.YouTubeDl, "-U");
            _youtubedl.Start();
            _youtubedl.WaitForExit();
            output = _youtubedl.StandardOutput.ReadToEnd();
            _youtubedl.Close();

            return output;
        }
    }
}
