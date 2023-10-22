export interface ArtistShortModel{
    id: string;
    name: string;
    followedByUser: boolean;
    receiveNotifications: boolean;
}

export interface AlbumArtistModel{
    id: string;
    name: string;
    release: Date;
}