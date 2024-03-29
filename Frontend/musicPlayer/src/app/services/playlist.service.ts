import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ModifieablePlaylistModel, SongPaginationModel, PlaylistUserShortModel, PlaylistPaginationModel } from '../models/playlist-models';
import { environment } from 'src/environments/environment';
import { APIROUTES } from '../constants/api-routes';

@Injectable({
  providedIn: 'root'
})
export class PlaylistService {

  constructor(private httpClient: HttpClient) { }

  public GetFavorites(skip: number, take: number, sortAfter: string, asc: boolean, query: string): Observable<SongPaginationModel>{
    return this.httpClient.get<SongPaginationModel>(`${environment.apiUrl}/${APIROUTES.playlist}/favorites?skip=${skip}&take=${take}&query=${query}&sortAfter=${sortAfter}&asc=${asc}`,{
      withCredentials: true
    })
  }

  public GetSongsFromPlaylist(skip: number, take: number, sortAfter: string, asc: boolean, query: string, playlistId: string): Observable<SongPaginationModel>{
    return this.httpClient.get<SongPaginationModel>(`${environment.apiUrl}/${APIROUTES.playlist}/songs/${playlistId}?skip=${skip}&take=${take}&query=${query}&sortAfter=${sortAfter}&asc=${asc}`,{
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

  public AddAlbumToPlaylist(playlistId: string, albumid: string): Observable<object>{
    return this.httpClient.get<object>(`${environment.apiUrl}/${APIROUTES.playlist}/album/${playlistId}?albumId=${albumid}`,{
      withCredentials: true
    })
  }

  public ChangeOrderOfSongInFavorites(oldOrder: number, newOrder: number): Observable<Object>{
    return this.httpClient.get(`${environment.apiUrl}/${APIROUTES.playlist}/order/favorite?oldOrder=${oldOrder}&newOrder=${newOrder}`,{
      withCredentials: true
    })
  }

  public ChangeOrderOfSongInPlaylist(playlistId: string, oldOrder: number, newOrder: number): Observable<Object>{
    return this.httpClient.get(`${environment.apiUrl}/${APIROUTES.playlist}/order/song?playlistId=${playlistId}&oldOrder=${oldOrder}&newOrder=${newOrder}`,{
      withCredentials: true
    })
  }

  public GetPlaylists(userId: string, page: number, take: number, query: string, sortAfter: string, asc: boolean): Observable<PlaylistPaginationModel>{
    return this.httpClient.get<PlaylistPaginationModel>(`${environment.apiUrl}/${APIROUTES.playlist}/playlists?userId=${userId}&page=${page}&take=${take}&query=${query}&sortAfter=${sortAfter}&asc=${asc}`,{
      withCredentials: true
    })
  }

  public SetPlaylistNotifications(playlistId: string): Observable<object>{
    return this.httpClient.get<object>(`${environment.apiUrl}/${APIROUTES.playlist}/notification/${playlistId}`,{
      withCredentials: true
    })
  }

  public ModifyPlaylistInfo(playlistId: string, name:string, description: string, isPublic: boolean, receiveNotifications: boolean): Observable<object>{
    return this.httpClient.patch<object>(`${environment.apiUrl}/${APIROUTES.playlist}?playlistId=${playlistId}`,
    {
      description: description,
      name: name,
      isPublic: isPublic,
      receiveNotifications: receiveNotifications
    },
    {
      withCredentials: true
    })
  }

  public DeletePlaylist(playlistId: string): Observable<object>{
    return this.httpClient.delete<object>(`${environment.apiUrl}/${APIROUTES.playlist}?playlistId=${playlistId}`,{
      withCredentials: true
    })
  }

  public AddPlaylistToLibrary(playlistId: string): Observable<object>{
    return this.httpClient.get<object>(`${environment.apiUrl}/${APIROUTES.playlist}/add/${playlistId}`,{
      withCredentials: true
    })
  }

  public CopyPlaylistToLibrary(playlistId: string): Observable<object>{
    return this.httpClient.get<object>(`${environment.apiUrl}/${APIROUTES.playlist}/copy/${playlistId}`,{
      withCredentials: true
    })
  }

  public ChangeOrderOfPlaylist(playlistId: string, newIndex: number): Observable<object>{
    return this.httpClient.get<object>(`${environment.apiUrl}/${APIROUTES.playlist}/order/playlist?playlistId=${playlistId}&order=${newIndex}`,{
      withCredentials: true
    })
  }

  public CreatePlaylist(name:string, description: string, isPublic: boolean, receiveNotifications: boolean): Observable<string>{
    return this.httpClient.post<string>(`${environment.apiUrl}/${APIROUTES.playlist}`,
    {
      description: description,
      name: name,
      isPublic: isPublic,
      receiveNotifications: receiveNotifications
    },
    {
      withCredentials: true
    })
  }




}
