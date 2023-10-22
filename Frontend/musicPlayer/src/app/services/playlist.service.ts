import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PlaylistSongPaginationModel } from '../models/playlist-models';
import { environment } from 'src/environments/environment';
import { APIROUTES } from '../constants/api-routes';

@Injectable({
  providedIn: 'root'
})
export class PlaylistService {

  constructor(private httpClient: HttpClient) { }

  public GetFavorites(page: number, take: number, sortAfter: string, asc: boolean, query: string): Observable<PlaylistSongPaginationModel>{
    return this.httpClient.get<PlaylistSongPaginationModel>(`${environment.apiUrl}/${APIROUTES.playlist}/favorites?page=${page}&take=${take}&query=${query}&sortAfter=${sortAfter}&asc=${asc}`,{
      withCredentials: true
    })
  }


}
