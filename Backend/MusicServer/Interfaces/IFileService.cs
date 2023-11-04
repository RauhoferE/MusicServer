namespace MusicServer.Interfaces
{
    public interface IFileService
    {

        public Task UploadPlaylistCover(Guid playlistId, IFormFile image, string extension);

        public Task UploadUserAvatar(IFormFile image, string extension);

        public IAsyncEnumerable<byte> GetSongStream(Guid songId);

        public Task<byte[]> GetPlaylistCover(Guid playlistId);

        public Task<byte[]> GetUserAvatar(long userId);

        public Task<byte[]> GetAlbumCover(Guid albumId);

        public Task<byte[]> GetArtistCover(Guid artistId);

        //public Task<string> GetMimeTypeForPlaylistCover(Guid playlistid);

        //public Task<string> GetMimeTypeForUserCover(long userId);
    }
}
