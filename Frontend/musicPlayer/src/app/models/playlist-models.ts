import { AlbumArtistModel, ArtistShortModel } from "./artist-models";

export interface FollowedPlaylistModel{
    id: string;
    name: string;
    songCount: number;
    creatorName: string;
}

export interface PlaylistSongPaginationModel{
    totalCount: number;
    songs: PlaylistSongModel[];
}

export interface PlaylistSongModel{
    checked: boolean;
    id: string;
    name: string;
    duration: number;
    album: AlbumArtistModel;
    artists: ArtistShortModel[];
    created: Date;
    modified: Date;
    order: number;
    isInFavorites: boolean;
}

export interface GuidNameModel{
    id: string;
    name: string;
    followedByUser: boolean;
    receiveNotifications: boolean;
}

export interface ModifieablePlaylistModel{
    playlists: GuidNameModel[];
}