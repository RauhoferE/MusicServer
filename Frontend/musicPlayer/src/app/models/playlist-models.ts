import { AlbumArtistModel, ArtistShortModel } from "./artist-models";
import { UserModel } from "./user-models";

export interface FollowedPlaylistModel{
    id: string;
    name: string;
    songCount: number;
    creatorName: string;
}

export interface SongPaginationModel{
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

export interface QueueSongModel extends PlaylistSongModel{
    addedManualy: boolean;
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

export interface PlaylistUserShortModel{
    id: string;
    name: string;
    songCount: number;
    isModifieable: boolean;
    receiveNotifications: boolean;
    order: number;
    isPublic: boolean;
    isCreator: boolean;
    created: Date;
    LastListened: Date;
    users: UserModel[];
    

}