using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicServer.Core.Interfaces
{
    public interface ISftpService
    {
        Task<bool> FileExistsAsync(string path);

        Task DeleteFileAsync(string path);

        Task<string> GetFileExtensionAsync(string path);

        Task<string[]> GetFileNameAsync(string path);

        Task UploadFileAsync(Stream stream, string remotePath);

        Task<Stream> StreamFileAsync(string path);

        Task<byte[]> DownloadFileAsync(string path);
    }
}
