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