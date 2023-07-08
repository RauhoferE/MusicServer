using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicServer.Core.Interfaces
{
    public interface ISftpService
    {
        Task<bool> FileExists(string path);

        Task DeleteFile(string path);

        Task<string> GetFileExtension(string path);

        Task<string[]> GetFileName(string path);

        Task UploadFile(Stream stream, string remotePath);

        Task<Stream> StreamFile(string path);

        Task<byte[]> DownloadFile(string path);
    }
}
