import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PlaylistSongModel, SongPaginationModel } from '../models/playlist-models';
import { environment } from 'src/environments/environment';
import { APIROUTES } from '../constants/api-routes';
import { Observable } from 'rxjs';
import { AlbumModel } from '../models/artist-models';

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

  public GetAlbumDetails(id: string): Observable<AlbumModel>{
    return this.httpClient.get<AlbumModel>(`${environment.apiUrl}/${APIROUTES.song}/album/${id}`,{
      withCredentials: true
    })
  }

  public GetAlbumSongs(id: string, skip: number, take: number): Observable<SongPaginationModel>{
    return this.httpClient.get<SongPaginationModel>(`${environment.apiUrl}/${APIROUTES.song}/album/songs/${id}?skip=${skip}&take=${take}`,{
      withCredentials: true
    })
  }
}
