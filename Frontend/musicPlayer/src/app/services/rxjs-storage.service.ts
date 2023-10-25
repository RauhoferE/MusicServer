import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { PaginationModel } from '../models/storage';

@Injectable({
  providedIn: 'root'
})
export class RxjsStorageService {
  private paginationSongModel$ = new BehaviorSubject<any>({});
  currentPaginationSongModel$ = this.paginationSongModel$.asObservable();

  constructor() { }

  setCurrentPaginationSongModel(model: PaginationModel){
    this.paginationSongModel$.next(model);
  }
}
