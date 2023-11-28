export interface PaginationModel{
    page: number;
    take: number;
    sortAfter: string;
    asc: boolean;
    query: string;
}

export interface QueueModel extends PaginationModel{
    // Is -1 for favorites and none
    itemGuid: string;
    // Can be favorites, playlist, album, artist, none (for single song)
    type: string;

}