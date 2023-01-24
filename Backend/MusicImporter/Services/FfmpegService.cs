using MusicImporter.Interfaces;
using MusicImporter.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicImporter.Services
{
    public class FfmpegService : IFfmpegService
    {
        private readonly MusicDataSettings _musicDataSettings;
        public FfmpegService(MusicDataSettings settings)
        {
            this._musicDataSettings = settings;
        }

        public async Task<double> GetDurationOfMp3(string filePath)
        {
            var fi = new FileInfo(filePath);

            if (!fi.Exists)
            {
                throw new FileNotFoundException(filePath);
            }

            var processStartInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = _musicDataSettings.MP3InfoPath,
                Arguments = $@"-p ""%S"" ""{filePath}"""
            };

            var p = Process.Start(processStartInfo);

            // StandardError returns the whole output
            // StandardOutput returns nothing

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                Log.Error($"Error trying to get duration of song {filePath}.");
                throw new Exception($"Error trying to get duration of song {filePath}.");
            }

            return Double.Parse(output);
        }
    }
}
