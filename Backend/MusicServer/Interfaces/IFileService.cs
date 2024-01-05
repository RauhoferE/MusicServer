namespace MusicServer.Interfaces
{
    public interface IFileService
    {

        public Task UploadPlaylistCoverAsync(Guid playlistId, IFormFile image, string extension);

        public Task UploadUserAvatarAsync(IFormFile image, string extension);

        public Task<byte[]> GetSongStreamAsync(Guid songId);

        public Task<byte[]> GetPlaylistCoverAsync(Guid playlistId);

        public Task<byte[]> GetUserAvatarAsync(long userId);

        public Task<byte[]> GetAlbumCoverAsync(Guid albumId);

        public Task<byte[]> GetArtistCoverAsync(Guid artistId);

        //public Task<string> GetMimeTypeForPlaylistCover(Guid playlistid);

        //public Task<string> GetMimeTypeForUserCover(long userId);
    }
}
