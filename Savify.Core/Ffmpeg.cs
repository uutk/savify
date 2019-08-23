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
            _ffmpeg.StartInfo.CreateNoWindow = true;
            return _ffmpeg;
        }

        public static void Run(string args)
        {
            Process _ffmpeg = GetProcess(Settings.Default.FFmpeg, args);
            _ffmpeg.Start();
            _ffmpeg.WaitForExit();
            _ffmpeg.Close();
        }
    }
}
