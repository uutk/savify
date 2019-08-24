using System.Diagnostics;

namespace Savify.Core
{
    public static class Ffmpeg
    {
        private static Process GetProcess(string path, string args)
        {
            Process _ffmpeg = new Process();
            _ffmpeg.StartInfo.UseShellExecute = false;
            _ffmpeg.StartInfo.FileName = path;
            _ffmpeg.StartInfo.Arguments = args;
            _ffmpeg.StartInfo.RedirectStandardOutput = true;
            _ffmpeg.StartInfo.CreateNoWindow = true;
            return _ffmpeg;
        }

        public static string Run(string args)
        {
            string output;
            Process _ffmpeg = GetProcess(Settings.Default.FFmpeg, args);
            _ffmpeg.Start();
            _ffmpeg.WaitForExit();
            output = _ffmpeg.StandardOutput.ReadToEnd();
            _ffmpeg.Close();

            return output;
        }
    }
}
