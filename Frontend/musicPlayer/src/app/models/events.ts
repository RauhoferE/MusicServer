import { NzTableQueryParams } from "ng-zorro-antd/table";
import { PlaylistSongModel } from "./playlist-models";

export interface TableQuery{
    params: NzTableQueryParams;
    query: string;
}

export interface PlaylistSongModelParams{
    index: number;
    songModel: PlaylistSongModel;
}