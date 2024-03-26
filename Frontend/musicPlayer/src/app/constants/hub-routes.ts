
export interface HubInvokes {
    joinSession:string;
    sendCurrentSongToJoinedUser: string;
    getCurrentQueue: string;
    addSongsToQueue: string;
    skipBackInQueue: string;
    skipForwardInQueue: string;
    clearQueue: string;
    removeSongsInQueue: string;
    pushSongInQueue: string;
    updatePlayerData: string;
    leaveGroup: string;
}

export interface HubEmits{
    getGroupName: string;
    userJoinedSession: string;
    userDisconnected: string;
    groupDeleted: string;
    getPlayerData: string;
    getQueue: string;
    getCurrentPlayingSong: string;
    getUserList: string;
}

export const HUBINVOKES: HubInvokes = {
    addSongsToQueue: 'AddSongsToQueue',
    clearQueue: 'ClearQueue',
    getCurrentQueue: 'GetCurrentQueue',
    joinSession: 'JoinSession',
    pushSongInQueue: 'PushSongInQueue',
    removeSongsInQueue: 'RemoveSongsInQueue',
    sendCurrentSongToJoinedUser: 'SendCurrentSongToJoinedUser',
    skipBackInQueue: 'SkipBackInQueue',
    skipForwardInQueue: 'SkipForwardInQueue',
    updatePlayerData: 'UpdatePlayerData',
    leaveGroup: 'LeaveGroup'
}

export const HUBEMITS: HubEmits = {
    getGroupName: 'GetGroupName',
    userJoinedSession: 'UserJoinedSession',
    userDisconnected: 'UserDisconnected',
    groupDeleted: 'GroupDeleted',
    getPlayerData: 'GetPlayerData',
    getQueue: 'GetQueue',
    getCurrentPlayingSong: 'GetCurrentPlayingSong',
    getUserList: 'GetUserList'
}