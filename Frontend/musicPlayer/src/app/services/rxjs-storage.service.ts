import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { PaginationModel } from '../models/storage';

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

  constructor() { }

  setCurrentPaginationSongModel(model: PaginationModel){
    this.paginationSongModel$.next(model);
  }

  setSongTableLoadingState(isLoading: boolean){
    this.songTableLoadingState$.next(isLoading);
  }

  setSongInTableChangedState(isLoading: boolean){
    this.songInTableChangedState$.next(isLoading);
  }
}
