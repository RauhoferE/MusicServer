using MusicImporter.Exceptions;
using MusicImporter.Interfaces;
using MusicImporter.JsonObjects;
using MusicImporter.Settings;
using MusicServer.Core.Interfaces;
using MusicServer.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MusicImporter.Services
{
    public class MusicBrainzService : IMusicBrainzService
    {
        private readonly IHttpClientFactory httpClientFactory;

        private readonly FileserverSettings fileserverSettings;

        private readonly ISftpService sftpService;

        public MusicBrainzService(IHttpClientFactory httpClientFactory, 
            FileserverSettings fileserverSettings,
            ISftpService sftpService)
        {
            this.httpClientFactory = httpClientFactory;
            this.fileserverSettings = fileserverSettings;
            this.sftpService = sftpService;
        }

        public async Task DownloadAlbumCover(string albumName, Guid albumId)
        {
            var client = this.httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.UserAgent
                .Add(new System.Net.Http.Headers.ProductInfoHeaderValue("Musicimporter", "0.1"));

            // Get MBID
            //https://musicbrainz.org/ws/2/release?query=No%20grave%20But%20the%20Sea%20(deluxe%20edition)&limit=1&fmt=json
            var mbidResp = await client.GetAsync($"https://musicbrainz.org/ws/2/release?query={WebUtility.UrlEncode(albumName)}&limit=10&fmt=json");

            if (mbidResp.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException("Http Error when trying to reach musicbrainz.org");
            }

            var mbidRespAsJson = await mbidResp.Content.ReadFromJsonAsync<MusicBrainzSearch>();

            if (mbidRespAsJson.Releases.Length == 0)
            {
                throw new MusicSearchException("No results found.");
            }

            // We want the album MBID for the cover
            var mbidId = mbidRespAsJson.Releases.FirstOrDefault(x => x.ReleaseGroup.PrimaryType == "Album");

            if (mbidId == null)
            {
                return;
            }

            // Get Cover Art xml
            // https://coverartarchive.org/release/21234ce3-9283-4eeb-9df7-0e938b1f46af

            var coverArtInfoResp = await client.GetAsync($"https://coverartarchive.org/release/{mbidId.Id}");

            if (coverArtInfoResp.StatusCode == HttpStatusCode.NotFound)
            {
                return;
            }

            if (coverArtInfoResp.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException("Http Error when trying to reach coverartarchive.org");
            }

            var coverArtInfoAsJson = await coverArtInfoResp.Content.ReadFromJsonAsync<CoverArtResponse>();

            if (coverArtInfoAsJson.Images.Length == 0)
            {
                return;
            }

            var bytes = await client.GetByteArrayAsync(coverArtInfoAsJson.Images[0].Thumbnails.Large);

            // Save File on samba server
            using (var ms = new MemoryStream(bytes))
            {
                await this.sftpService.UploadFile(ms, $"{fileserverSettings.AlbumCoverFolder}/{albumId}.jpg");
            }
            
        }

        public async Task<DateTime> GetAlbumReleaseDate(string albumName)
        {
            var client = this.httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.UserAgent
    .Add(new System.Net.Http.Headers.ProductInfoHeaderValue("Musicimporter", "0.1"));
            // Get MBID
            //https://musicbrainz.org/ws/2/release?query=No%20grave%20But%20the%20Sea%20(deluxe%20edition)&limit=1&fmt=json
            var mbidResp = await client.GetAsync($"https://musicbrainz.org/ws/2/release?query={WebUtility.UrlEncode(albumName)}&limit=1&fmt=json");

            if (mbidResp.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException("Http Error when trying to reach musicbrainz.org");
            }

            var mbidRespAsJson = await mbidResp.Content.ReadFromJsonAsync<MusicBrainzSearch>();

            if (mbidRespAsJson.Releases.Length == 0)
            {
                throw new MusicSearchException("No results found.");
            }

            return mbidRespAsJson.Releases[0].Date;
        }
    }
}
