import { Injectable } from '@angular/core';
import { PaginationModel } from '../models/storage';

@Injectable({
  providedIn: 'root'
})
export class SessionStorageService {

  constructor() { }

  public GetLastPaginationOfFavorites(): PaginationModel | null{
    var val = sessionStorage.getItem('favorites_pagination');

    if (val == null) {
      return val;
    }

    return JSON.parse(val) as PaginationModel; 
  }

  public SaveLastPaginationOfFavorites(model: PaginationModel): void{
    sessionStorage.setItem('favorites_pagination', JSON.stringify(model));
  }

  public GetLastPaginationOfPlaylist(): PaginationModel | null{
    var val = sessionStorage.getItem('playlist_pagination');

    if (val == null) {
      return val;
    }

    return JSON.parse(val) as PaginationModel; 
  }

  public SaveLastPaginationOfPlaylist(model: PaginationModel): void{
    sessionStorage.setItem('playlist_pagination', JSON.stringify(model));
  }
}
