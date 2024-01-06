export interface ArtistShortModel{
    id: string;
    name: string;
    followedByUser: boolean;
    receiveNotifications: boolean;
    description: string;
}

export interface AlbumArtistModel{
    id: string;
    name: string;
    release: Date;
}

export interface AlbumModel{
    id: string;
    name: string;
    artists: ArtistShortModel[];
    release: Date;
    songCount: number;
    duration: number;
}

export interface AlbumPaginationModel{
    totalCount: number;
    albums: AlbumModel[];
}