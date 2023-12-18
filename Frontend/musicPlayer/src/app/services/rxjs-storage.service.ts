import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { PaginationModel, QueueModel } from '../models/storage';
import { PlaylistSongModel, PlaylistSongPaginationModel } from '../models/playlist-models';

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

  private isSongPlayingChangedState$ = new BehaviorSubject<any>(false);
  isSongPlayingState = this.isSongPlayingChangedState$.asObservable();

  private showMediaPlayerChangedState$ = new BehaviorSubject<any>({});
  showMediaPlayerState = this.showMediaPlayerChangedState$.asObservable();

  private songqueue$ = new BehaviorSubject<any>({});
  currentSongQueue = this.songqueue$.asObservable();

  private currentPlaylingSongModel$ = new BehaviorSubject<any>({});
  currentPlayingSong = this.currentPlaylingSongModel$.asObservable();

  private queueFilterAndPagination$ = new BehaviorSubject<any>({});
  currentQueueFilterAndPagination = this.queueFilterAndPagination$.asObservable();

  private playedSongs$ = new BehaviorSubject<any>({});
  currentPlayedSongs = this.playedSongs$.asObservable();

  // private songsInTable$ = new BehaviorSubject<any>({});
  // currentSongsInTable = this.songsInTable$.asObservable();


  constructor() { }

  setCurrentPaginationSongModel(model: PaginationModel){
    this.paginationSongModel$.next(model);
  }

  setSongTableLoadingState(isLoading: boolean){
    this.songTableLoadingState$.next(isLoading);
  }

  setUpdateDashboardBoolean(val: boolean){
    this.updateDashboardState$.next(val);
  }

  setUpdateCurrentTableBoolean(val: boolean){
    this.updateCurrentTableState$.next(val);
  }

  setIsSongPlaylingState(isPlayling: boolean){
    this.isSongPlayingChangedState$.next(isPlayling);
  }

  showMediaPlayer(show: boolean){
    this.showMediaPlayerChangedState$.next(show);
  }

  setSongQueue(songs: PlaylistSongModel[]){
    this.songqueue$.next(songs);
  }

  setPlayedSongs(songs: PlaylistSongModel[]){
    this.playedSongs$.next(songs);
  }

  // setCurrentSongsInTable(songs: PlaylistSongPaginationModel){
  //   this.songsInTable$.next(songs);
  // }

  // replaceCurrentSongInSongsTable(song: PlaylistSongModel){
  //   let songsWithPag = this.songsInTable$.getValue() as PlaylistSongPaginationModel;
  //   const indexOfElement = songsWithPag.songs.findIndex(x => x.id == song.id);
  //   if (indexOfElement == -1) {
  //     // Song is not in Table 
  //     // No need to replace it
  //     return;
  //   }

  //   // Replace element and save
  //   songsWithPag.songs.splice(indexOfElement, 1, song);
  //   this.songsInTable$.next(songsWithPag);
  // }

  addSongToQueue(song: PlaylistSongModel){
    let currentQueue: PlaylistSongModel[] = [];
    let queue = this.songqueue$.getValue()as PlaylistSongModel[];

    if (queue && queue.length > 0) {
      currentQueue = queue;
      currentQueue.push(song);
      this.songqueue$.next(currentQueue);
      return;
    }

    this.songqueue$.next([song]);
  }

  addSongToPlayed(song: PlaylistSongModel){
    let currentPlayed: PlaylistSongModel[] = [];
    let played = this.playedSongs$.getValue()as PlaylistSongModel[];

    if (played && played.length > 0) {
      currentPlayed = played;
      currentPlayed.push(song);
      this.playedSongs$.next(currentPlayed);
      return;
    }

    this.playedSongs$.next([song]);
  }

  removeLastPlayedSong(){
    let currentPlayed: PlaylistSongModel[] = [];
    let played = this.playedSongs$.getValue()as PlaylistSongModel[];

    if (played && played.length > 0) {
      currentPlayed = played;
      currentPlayed.splice((currentPlayed.length - 1),1);
      this.playedSongs$.next(currentPlayed);
      return;
    }
  }

  addSongsToQueue(songs: PlaylistSongModel[]){
    let currentQueue: PlaylistSongModel[] = [];
    let queue = this.songqueue$.getValue()as PlaylistSongModel[];

    if (queue && queue.length > 0) {
      currentQueue = queue;
      currentQueue = currentQueue.concat(songs);
      this.songqueue$.next(currentQueue);
      return;
    }

    this.songqueue$.next(songs);
  }

  replaceSongInQueue(song: PlaylistSongModel){
    let queue = this.songqueue$.getValue()as PlaylistSongModel[];

    if (!queue) {
      return;
    }

    if (!Array.isArray(queue) || queue.length == 0) {
      return;
    }

    const indexOfElement = queue.findIndex(x => x.id == song.id);

    if (indexOfElement == -1) {
      return;
    }

    queue.splice(indexOfElement, 1, song);

    this.songqueue$.next(queue);
  }

  replaceSongsInQueue(songs: PlaylistSongModel[]){
    let queue = this.songqueue$.getValue()as PlaylistSongModel[];

    if (!queue) {
      return;
    }

    if (!Array.isArray(queue) || queue.length == 0) {
      return;
    }

    for (let index = 0; index < songs.length; index++) {
      const indexOfElement = queue.findIndex(x => x.id == songs[index].id);

      if (indexOfElement != -1) {
        queue.splice(indexOfElement, 1, songs[index]);
      }

    }

    this.songqueue$.next(queue);
  }

  pushSongToPlaceInQueue(sourceIndex: number, targetIndex: number){
    let queue = this.songqueue$.getValue()as PlaylistSongModel[];

    if (!queue) {
      return;
    }

    if (!Array.isArray(queue) ||queue.length == 0) {
      return;
    }

    if (queue.length < targetIndex || targetIndex < 0 
      || queue.length < sourceIndex || sourceIndex < 0
      || sourceIndex == targetIndex) {
      return;
    }

    let srcElement = queue.splice(sourceIndex, 1);

    queue.splice(targetIndex, 0, srcElement[0]);

    this.songqueue$.next(queue);
  }

  setCurrentPlayingSong(song: PlaylistSongModel){
    this.currentPlaylingSongModel$.next(song);
  }

  checkFoReplaceingCurrentPlayingSong(songs: PlaylistSongModel[]){

    const currentPlayingSong = this.currentPlaylingSongModel$.getValue() as PlaylistSongModel;

    if (!currentPlayingSong) {
      return;
    }

    for (let index = 0; index < songs.length; index++) {
      if (currentPlayingSong.id == songs[index].id) {
        this.currentPlaylingSongModel$.next(songs[index]);
      }
    }
  }

  clearSongQueue(){
    this.songqueue$.next([]);
  }

  removeSongWithIndexFromQueue(index: number){
    let queue = this.songqueue$.getValue() as PlaylistSongPaginationModel[];

    if (!queue) {
      return;
    }

    if (!Array.isArray(queue) ||queue.length == 0) {
      return;
    }

    if (queue.length < index || index < 0) {
      return;
    }

    queue.splice(index, 1);
    this.songqueue$.next(queue);
  }

  // This is used for getting the songs for the queue
  setQueueFilterAndPagination(model: QueueModel){
    this.queueFilterAndPagination$.next(model);
  }

  incrementSongsInQueueFilter(){
    let queuePagination = this.queueFilterAndPagination$.getValue() as QueueModel;

    if (!queuePagination) {
      return;
    }

    queuePagination.page = queuePagination.page + 1;
    this.queueFilterAndPagination$.next(queuePagination);
  }

  decrementSongsInQueueFilter(){
    let queuePagination = this.queueFilterAndPagination$.getValue() as QueueModel;

    if (!queuePagination) {
      return;
    }

    if (queuePagination.page == 0) {
      return;
    }

    queuePagination.page = queuePagination.page - 1;
    this.queueFilterAndPagination$.next(queuePagination);
  }


}
