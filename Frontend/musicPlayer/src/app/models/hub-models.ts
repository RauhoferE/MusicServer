export interface CurrentMediaPlayerData{
    itemId: string;
    target: string;
    isPlaying: boolean;
    secondsPlayed: number;
    random: boolean;
    loopMode: string;
}

export interface SessionUserData{
    email: string;
    userId: number;
    isMaster: boolean;

}