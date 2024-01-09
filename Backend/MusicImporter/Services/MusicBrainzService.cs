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

        public async Task DownloadAlbumCover(string albumName, Guid albumId, string artistName)
        {
            var client = this.httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.UserAgent
                .Add(new System.Net.Http.Headers.ProductInfoHeaderValue("Musicimporter", "0.1"));

            // Get MBID
            //https://musicbrainz.org/ws/2/release?query=No%20grave%20But%20the%20Sea%20(deluxe%20edition)&limit=1&fmt=json
            var mbidResp = await client.GetAsync($"https://musicbrainz.org/ws/2/release?query={WebUtility.UrlEncode(albumName)}%20AND%20{WebUtility.UrlEncode(artistName)}&limit=20&fmt=json");

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
            var mbidIds = mbidRespAsJson.Releases.Where(x => x.ReleaseGroup.PrimaryType == "Album" 
            && x.ArtistCredits.Any(y => y.Name.ToLower() == artistName.ToLower()));

            if (mbidIds.Count() == 0)
            {
                return;
            }

            HttpResponseMessage coverArtInfoResp = null;
            // Get Cover Art xml
            // https://coverartarchive.org/release/21234ce3-9283-4eeb-9df7-0e938b1f46af
            foreach (var mbidId in mbidIds)
            {
                coverArtInfoResp = await client.GetAsync($"https://coverartarchive.org/release/{mbidId.Id}");

                if (coverArtInfoResp.StatusCode == HttpStatusCode.NotFound)
                {
                    continue;
                }

                if (coverArtInfoResp.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpRequestException("Http Error when trying to reach coverartarchive.org");
                }

                if (coverArtInfoResp.IsSuccessStatusCode)
                {
                    break;
                }
            }

            if (coverArtInfoResp == null)
            {
                throw new MusicSearchException("No valid covers found.");
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
                await this.sftpService.UploadFileAsync(ms, $"{fileserverSettings.AlbumCoverFolder}/{albumId}.jpg");
            }
            
        }

        public async Task<DateTime> GetAlbumReleaseDate(string albumName)
        {
            var client = this.httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.UserAgent
    .Add(new System.Net.Http.Headers.ProductInfoHeaderValue("Musicimporter", "0.1"));
            // Get MBID
            //https://musicbrainz.org/ws/2/release?query=No%20grave%20But%20the%20Sea%20(deluxe%20edition)&limit=1&fmt=json
            var mbidResp = await client.GetAsync($"https://musicbrainz.org/ws/2/release?query={WebUtility.UrlEncode(albumName)}&limit=20&fmt=json");

            if (mbidResp.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException("Http Error when trying to reach musicbrainz.org");
            }

            var mbidRespAsJson = await mbidResp.Content.ReadFromJsonAsync<MusicBrainzSearch>();

            if (mbidRespAsJson.Releases.Length == 0)
            {
                throw new MusicSearchException("No results found.");
            }

            foreach (var result in mbidRespAsJson.Releases)
            {
                if (result.Date > new DateTime(1800,1,1))
                {
                    return result.Date;
                }
            }

            return DateTime.Now;
        }
    }
}
