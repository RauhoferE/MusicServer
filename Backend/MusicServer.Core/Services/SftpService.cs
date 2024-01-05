using MusicServer.Core.Interfaces;
using MusicServer.Core.Settings;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicServer.Core.Services
{
    public class SftpService : ISftpService
    {
        private readonly FileServerCredentials credentials;

        public SftpService(FileServerCredentials serverCredentials)
        {
            this.credentials = serverCredentials;
        }

        public async Task DeleteFileAsync(string path)
        {
            using (var client = new SftpClient(this.credentials.Host, this.credentials.Port, this.credentials.UserName, this.credentials.Password))
            {
                client.Connect();

                client.DeleteFile(path);
            }
        }

        public async Task<byte[]> DownloadFileAsync(string path)
        {
            using (var client = new SftpClient(this.credentials.Host, this.credentials.Port, this.credentials.UserName, this.credentials.Password))
            {
                client.Connect();

                var byteArray = new byte[0];
                using (var stream = client.OpenRead(path))
                {
                    byteArray = new byte[stream.Length];
                    stream.Read(byteArray, 0, byteArray.Length);
                }

                return byteArray;
            }
        }

        public async Task<bool> FileExistsAsync(string path)
        {
            using (var client = new SftpClient(this.credentials.Host, this.credentials.Port, this.credentials.UserName, this.credentials.Password))
            {
                client.Connect();
                
                return client.Exists(path);
            }
        }

        public async Task<string[]> GetFileNameAsync(string path)
        {
            using (var client = new SftpClient(this.credentials.Host, this.credentials.Port, this.credentials.UserName, this.credentials.Password))
            {
                client.Connect();
                return client.ListDirectory(path).Select(x => x.Name).ToArray();
            }
        }

        public async Task<Stream> StreamFileAsync(string path)
        {
            using (var client = new SftpClient(this.credentials.Host, this.credentials.Port, this.credentials.UserName, this.credentials.Password))
            {
                client.Connect();
                return client.OpenRead(path);
            }
        }

        public async Task UploadFileAsync(Stream stream, string remotePath)
        {
            using (var client = new SftpClient(this.credentials.Host, this.credentials.Port, this.credentials.UserName, this.credentials.Password))
            {
                client.Connect();
                using (var ws = client.OpenWrite(remotePath))
                {
                    await stream.CopyToAsync(ws);
                }
            }
        }

        public async Task<string> GetFileExtensionAsync(string path)
        {
            using (var client = new SftpClient(this.credentials.Host, this.credentials.Port, this.credentials.UserName, this.credentials.Password))
            {
                client.Connect();

                var file = client.Get(path);
                return file.Name.Substring(file.Name.LastIndexOf('.'), file.Name.Length - file.Name.LastIndexOf('.'));
            }
        }
    }
}
