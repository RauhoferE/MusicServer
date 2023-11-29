import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ModifieablePlaylistModel, PlaylistSongPaginationModel, PlaylistUserShortModel } from '../models/playlist-models';
import { environment } from 'src/environments/environment';
import { APIROUTES } from '../constants/api-routes';

@Injectable({
  providedIn: 'root'
})
export class PlaylistService {

  constructor(private httpClient: HttpClient) { }

  public GetFavorites(skip: number, take: number, sortAfter: string, asc: boolean, query: string): Observable<PlaylistSongPaginationModel>{
    return this.httpClient.get<PlaylistSongPaginationModel>(`${environment.apiUrl}/${APIROUTES.playlist}/favorites?skip=${skip}&take=${take}&query=${query}&sortAfter=${sortAfter}&asc=${asc}`,{
      withCredentials: true
    })
  }

  public GetSongsFromPlaylist(skip: number, take: number, sortAfter: string, asc: boolean, query: string, playlistId: string): Observable<PlaylistSongPaginationModel>{
    return this.httpClient.get<PlaylistSongPaginationModel>(`${environment.apiUrl}/${APIROUTES.playlist}/songs/${playlistId}?skip=${skip}&take=${take}&query=${query}&sortAfter=${sortAfter}&asc=${asc}`,{
      withCredentials: true
    })
  }

  public GetPlaylistInfo(playlistId: string): Observable<PlaylistUserShortModel>{
    return this.httpClient.get<PlaylistUserShortModel>(`${environment.apiUrl}/${APIROUTES.playlist}?playlistId=${playlistId}`,{
      withCredentials: true
    })
  }

  public AddSongsToFavorites(songIds: string[]): Observable<Object>{
    return this.httpClient.post(`${environment.apiUrl}/${APIROUTES.playlist}/favorites`,
    {
      "songIds":songIds
    },
    {
      withCredentials: true
    })
  }

  public RemoveSongsFromFavorites(songIds: string[]): Observable<Object>{
    return this.httpClient.delete(`${environment.apiUrl}/${APIROUTES.playlist}/favorites`,
    {
      withCredentials: true,
      body: {
        "songIds":songIds
      }
    }
    )
  }

  public RemoveSongsFromPlaylist(orderids: number[], playlistId: string): Observable<Object>{
    return this.httpClient.delete(`${environment.apiUrl}/${APIROUTES.playlist}/songs/${playlistId}`,
    {
      withCredentials: true,
      body: {
        "orderIds":orderids
      }
    }
    )
  }

  public GetModifieablePlaylists(userId: number): Observable<ModifieablePlaylistModel>{
    return this.httpClient.get<ModifieablePlaylistModel>(`${environment.apiUrl}/${APIROUTES.playlist}/playlists/modifieable?userId=${userId}`,{
      withCredentials: true
    })
  }

  public AddSongsToPlaylist(songIds: string[], playlistId: string): Observable<Object>{
    return this.httpClient.post(`${environment.apiUrl}/${APIROUTES.playlist}/songs/${playlistId}`,
    {
      "songIds":songIds
    },
    {
      withCredentials: true
    })
  }


}
