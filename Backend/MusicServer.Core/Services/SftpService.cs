using MusicServer.Core.Interfaces;
using MusicServer.Core.Settings;
using Renci.SshNet;
using System;
using System.Collections.Generic;
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

        public async Task<byte[]> DownloadFile(string path)
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

        public async Task<string[]> GetFileName(string path)
        {
            using (var client = new SftpClient(this.credentials.Host, this.credentials.Port, this.credentials.UserName, this.credentials.Password))
            {
                client.Connect();
                return client.ListDirectory(path).Select(x => x.Name).ToArray();
            }
        }

        public async Task<Stream> StreamFile(string path)
        {
            using (var client = new SftpClient(this.credentials.Host, this.credentials.Port, this.credentials.UserName, this.credentials.Password))
            {
                client.Connect();
                return client.OpenRead(path);
            }
        }

        public async Task UploadFile(Stream stream, string remotePath)
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
    }
}
