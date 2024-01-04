export interface QueueTypes{
    playlist: string;
    album: string;
    favorites: string;
    song: string;
}

export const QUEUETYPES: QueueTypes = {
    album: 'album',
    favorites: 'favorites',
    playlist: 'playlist',
    song: 'song'
}