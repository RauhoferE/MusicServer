using MusicImporter.Exceptions;
using MusicImporter.Interfaces;
using MusicImporter.JsonObjects;
using MusicImporter.Settings;
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

        public MusicBrainzService(IHttpClientFactory httpClientFactory, FileserverSettings fileserverSettings)
        {
            this.httpClientFactory = httpClientFactory;
            this.fileserverSettings = fileserverSettings;
        }

        public async Task DownloadAlbumCover(string albumName, Guid albumId)
        {
            var client = this.httpClientFactory.CreateClient();

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

            // Get Cover Art xml
            // https://coverartarchive.org/release/21234ce3-9283-4eeb-9df7-0e938b1f46af

            var coverArtInfoResp = await client.GetAsync($"https://coverartarchive.org/release/{mbidRespAsJson.Releases[0].Id}/index.json");

            if (mbidResp.StatusCode == HttpStatusCode.NotFound)
            {
                return;
            }

            if (mbidResp.StatusCode != HttpStatusCode.OK)
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
        }

        public async Task<DateTime> GetAlbumReleaseDate(string albumName)
        {
            var client = this.httpClientFactory.CreateClient();

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
