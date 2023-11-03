import { ArtistShortModel } from "./artist-models";
import { FollowedPlaylistModel } from "./playlist-models";

export interface RegisterModel{
    email: string;
    password: string;
    userName: string;
    birth: Date;
    registrationCode: string;
}

export interface UserModel{
    id: number;
    userName: string;
    isFollowedByUser: boolean;
    receiveNotifications: boolean;
    isCreator: boolean;
}

export interface AllFollowedEntitiesModel{
    followedUsers: UserModel[];
    followedArtists: ArtistShortModel[];
    followedPlaylists: FollowedPlaylistModel[];
    favoritesSongCount: number;
}

