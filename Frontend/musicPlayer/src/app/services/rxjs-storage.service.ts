import { Injectable } from '@angular/core';
import { BehaviorSubject, first, firstValueFrom } from 'rxjs';
import { PaginationModel, QueueModel } from '../models/storage';
import { PlaylistSongModel, SongPaginationModel } from '../models/playlist-models';

@Injectable({
  providedIn: 'root'
})
export class RxjsStorageService {
  private paginationSongModel$ = new BehaviorSubject<any>({});
  currentPaginationSongModel$ = this.paginationSongModel$.asObservable();

  private songTableLoadingState$ = new BehaviorSubject<any>({});
  currentSongTableLoading$ = this.songTableLoadingState$.asObservable();

  private updateDashboardState$ = new BehaviorSubject<any>({});
  updateDashboardBoolean$ = this.updateDashboardState$.asObservable();

  private updateCurrentTableState$ = new BehaviorSubject<any>({});
  updateCurrentTableBoolean$ = this.updateCurrentTableState$.asObservable();

  private updateQueueState$ = new BehaviorSubject<any>({});
  updateQueueBoolean$ = this.updateQueueState$.asObservable();

  private isSongPlayingChangedState$ = new BehaviorSubject<any>(false);
  isSongPlayingState = this.isSongPlayingChangedState$.asObservable();

  private showMediaPlayerChangedState$ = new BehaviorSubject<any>({});
  showMediaPlayerState = this.showMediaPlayerChangedState$.asObservable();

  private currentPlaylingSongModel$ = new BehaviorSubject<any>({});
  currentPlayingSong = this.currentPlaylingSongModel$.asObservable();

  private queueFilterAndPagination$ = new BehaviorSubject<any>({});
  currentQueueFilterAndPagination = this.queueFilterAndPagination$.asObservable();

  private replaySongState$ = new BehaviorSubject<any>({});
  updateReplaySongState = this.replaySongState$.asObservable();

  private updateProfilePicState$ = new BehaviorSubject<any>({});
  updateProfilePicBoolean$ = this.updateProfilePicState$.asObservable();

  private updatePlaylistViewState$ = new BehaviorSubject<any>({});
  updatePlaylistViewBoolean = this.updatePlaylistViewState$.asObservable();


  constructor() { }

  async setUpdateSongState(){

    let oldState = await firstValueFrom(this.updateReplaySongState);
    this.replaySongState$.next(!oldState);
  }

  setCurrentPaginationSongModel(model: PaginationModel){
    this.paginationSongModel$.next(model);
  }

  setSongTableLoadingState(isLoading: boolean){
    this.songTableLoadingState$.next(isLoading);
  }

  setUpdateDashboardBoolean(val: boolean){
    this.updateDashboardState$.next(val);
  }

  setProfilePicBoolean(val: boolean){
    this.updateProfilePicState$.next(val);
  }

  setUpdateCurrentTableBoolean(val: boolean){
    this.updateCurrentTableState$.next(val);
  }

  setUpdateQueueBoolean(val: boolean){
    this.updateQueueState$.next(val);
  }

  setUpdatePlaylistViewBoolean(val: boolean){
    this.updatePlaylistViewState$.next(val);
  }

  setIsSongPlaylingState(isPlayling: boolean){
    this.isSongPlayingChangedState$.next(isPlayling);
  }

  showMediaPlayer(show: boolean){
    this.showMediaPlayerChangedState$.next(show);
  }

  setCurrentPlayingSong(song: PlaylistSongModel){
    this.currentPlaylingSongModel$.next(song);
  }

  // This is used for getting the songs for the queue
  setQueueFilterAndPagination(model: QueueModel){
    this.queueFilterAndPagination$.next(model);
  }


}
