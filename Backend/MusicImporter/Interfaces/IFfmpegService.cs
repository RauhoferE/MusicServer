using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicImporter.Interfaces
{
    public interface IFfmpegService
    {
        Task<double> GetDurationOfMp3(string filePath);
    }
}
