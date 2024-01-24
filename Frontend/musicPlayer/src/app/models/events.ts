import { NzTableQueryParams } from "ng-zorro-antd/table";
import { GuidNameModel, PlaylistSongModel, PlaylistUserShortModel } from "./playlist-models";
import { AlbumModel } from "./artist-models";

export interface TableQuery{
    params: NzTableQueryParams;
    query: string;
}

export interface PlaylistSongModelParams{
    index: number;
    songModel: PlaylistSongModel;
}

export interface PlaylistModelParams{
    index: number;
    playlistModel: PlaylistUserShortModel;
}

export interface EditPlaylistModalParams{
    playlistModel: PlaylistUserShortModel;
    newCoverFile: File |undefined;

}

export interface DragDropSongParams{
    srcSong: PlaylistSongModel;
    destSong: PlaylistSongModel;
    srcIndex: number;
    destIndex: number;
}

export interface DragDropQueueParams extends DragDropSongParams{
    // -1 for automaitc
    // 0 mark as false
    // 1 mark as true
    markAsManuallyAdded: number;
}