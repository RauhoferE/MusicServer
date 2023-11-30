import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PlaylistSongModel } from '../models/playlist-models';
import { environment } from 'src/environments/environment';
import { APIROUTES } from '../constants/api-routes';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SongService {

  constructor(private httpClient: HttpClient) { }

  public GetSongDetails(id: string): Observable<PlaylistSongModel>{
    return this.httpClient.get<PlaylistSongModel>(`${environment.apiUrl}/${APIROUTES.song}/${id}`,{
      withCredentials: true
    })
  }
}
