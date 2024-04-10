
export interface HubInvokes {
    createSession:string;
    joinSession:string;
    sendCurrentSongProgress: string;
    getCurrentSongQueue: string;
    skipBackInQueue: string;
    skipForwardInQueue: string;
    clearQueue: string;
    addSongsToQueue: string;
    removeSongsInQueue: string;
    pushSongInQueue: string;
    getCurrentSong: string;
    getQueueData: string;
    updateQueueData: string;
    updateLoopMode: string;
    randomizeQueue: string;
    createQueueFromAlbum: string;
    createQueueFromPlaylist: string;
    createQueueFromFavorites: string;
    createQueueFromSingleSong: string;
    addAlbumToQueue: string;
    addPlaylistToQueue: string;
    playPauseSong: string;
}

export interface HubEmits{
    receiveGroupName: string;
    receiveQueueData: string;
    receiveQueueEntities: string;
    receiveCurrentPlayingSong: string;
    userDisconnected: string;
    groupDeleted: string;
    receiveUserList: string;
    userJoinedSession: string;
    receiveSongProgress: string;
    updateQueueView: string;
    updateCurrentSong: string;
    receivePlayPauseSongState: string;
    receiveErrorMessage: string;
}

export const HUBINVOKES: HubInvokes = {
    createSession: 'CreateSession',
    addSongsToQueue: 'AddSongsToQueue',
    clearQueue: 'ClearQueue',
    getCurrentSongQueue: 'GetCurrentSongQueue',
    joinSession: 'JoinSession',
    pushSongInQueue: 'PushSongInQueue',
    removeSongsInQueue: 'RemoveSongsInQueue',
    sendCurrentSongProgress: 'SendCurrentSongProgress',
    skipBackInQueue: 'SkipBackInQueue',
    skipForwardInQueue: 'SkipForwardInQueue',
    addAlbumToQueue: 'AddAlbumToQueue',
    addPlaylistToQueue: 'AddPlaylistToQueue',
    createQueueFromAlbum: 'CreateQueueFromAlbum',
    createQueueFromFavorites: 'CreateQueueFromFavorites',
    createQueueFromPlaylist: 'CreateQueueFromPlaylist',
    createQueueFromSingleSong: 'CreateQueueFromSingleSong',
    getCurrentSong: 'GetCurrentSong',
    randomizeQueue: 'RandomizeQueue',
    updateLoopMode:'UpdateLoopMode',
    updateQueueData: 'UpdateQueueData',
    getQueueData: 'GetQueueData',
    playPauseSong: 'PlayPauseSong'
}

export const HUBEMITS: HubEmits = {
    groupDeleted: 'GroupDeleted',
    receiveCurrentPlayingSong: 'ReceiveCurrentPlayingSong',
    receiveGroupName: 'ReceiveGroupName',
    receiveQueueData: 'ReceiveQueueData',
    receiveQueueEntities: 'ReceiveQueueEntities',
    receiveSongProgress: 'ReceiveSongProgress',
    receiveUserList: 'ReceiveUserList',
    updateCurrentSong: 'UpdateCurrentSong',
    updateQueueView: 'UpdateQueueView',
    userDisconnected: 'UserDisconnected',
    userJoinedSession: 'UserJoinedSession',
    receivePlayPauseSongState: 'ReceivePlayPauseSongState',
    receiveErrorMessage: 'ReceiveErrorMessage'

}