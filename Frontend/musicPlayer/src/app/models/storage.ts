export interface PaginationModel{
    page: number;
    take: number;
    sortAfter: string;
    asc: boolean;
    query: string;
}

export interface QueueModel extends PaginationModel{
    // Is -1 for favorites and none
    itemId: string;
    // Can be favorites, playlist, album, artist, none (for single song)
    target: string;

    // If the mediaplayer has loopmode activated
    loopMode: string;

    // IF the mediaplayer has random actived
    random: boolean;

}