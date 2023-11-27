import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { PaginationModel } from '../models/storage';
import { PlaylistSongModel } from '../models/playlist-models';

@Injectable({
  providedIn: 'root'
})
export class RxjsStorageService {
  private paginationSongModel$ = new BehaviorSubject<any>({});
  currentPaginationSongModel$ = this.paginationSongModel$.asObservable();

  private songTableLoadingState$ = new BehaviorSubject<any>({});
  currentSongTableLoading$ = this.songTableLoadingState$.asObservable();

  private songInTableChangedState$ = new BehaviorSubject<any>({});
  currentSongInTableChanged$ = this.songInTableChangedState$.asObservable();

  private isSongPlayingChangedState$ = new BehaviorSubject<any>({});
  isSongPlayingState = this.isSongPlayingChangedState$.asObservable();

  private showMediaPlayerChangedState$ = new BehaviorSubject<any>({});
  showMediaPlayerState = this.showMediaPlayerChangedState$.asObservable();

  private songqeue$ = new BehaviorSubject<any>({});
  currentSongQueue = this.songqeue$.asObservable();

  private currentPlaylingSongModel$ = new BehaviorSubject<any>({});
  currentPlayingSong = this.currentPlaylingSongModel$.asObservable();


  constructor() { }

  setCurrentPaginationSongModel(model: PaginationModel){
    this.paginationSongModel$.next(model);
  }

  setSongTableLoadingState(isLoading: boolean){
    this.songTableLoadingState$.next(isLoading);
  }

  setIsSongPlaylingState(isPlayling: boolean){
    this.isSongPlayingChangedState$.next(isPlayling);
  }

  showMediaPlayer(show: boolean){
    this.showMediaPlayerChangedState$.next(show);
  }

  setSongQueue(songs: PlaylistSongModel[]){
    this.songqeue$.next(songs);
  }

  addSongToQueue(song: PlaylistSongModel){
    let currentQueue: PlaylistSongModel[] = [];
    let queue = this.songqeue$.getValue()as PlaylistSongModel[];

    if (queue && queue.length > 0) {
      currentQueue = queue;
      currentQueue.push(song);
      this.songqeue$.next(currentQueue);
      return;
    }

    this.songqeue$.next([song]);
  }

  addSongsToQueue(songs: PlaylistSongModel[]){
    let currentQueue: PlaylistSongModel[] = [];
    let queue = this.songqeue$.getValue()as PlaylistSongModel[];

    if (queue && queue.length > 0) {
      currentQueue = queue;
      currentQueue = currentQueue.concat(songs);
      this.songqeue$.next(currentQueue);
      return;
    }

    this.songqeue$.next(songs);
  }

  pushSongToPlaceInQueue(sourceIndex: number, targetIndex: number){
    let queue = this.songqeue$.getValue()as PlaylistSongModel[];

    if (!queue) {
      return;
    }




  }

  setCurrentPlayingSong(song: PlaylistSongModel){
    this.currentPlaylingSongModel$.next(song);
  }

  //clearQueue
  // RemoveSongWithIndexFromQueue
}
