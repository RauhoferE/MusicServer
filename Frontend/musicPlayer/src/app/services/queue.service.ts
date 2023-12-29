import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PlaylistSongModel } from '../models/playlist-models';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { APIROUTES } from '../constants/api-routes';

@Injectable({
  providedIn: 'root'
})
export class QueueService {

  constructor(private httpClient: HttpClient) { }

  public CreateQueueFromAlbum(albumId: string, randomize: boolean, playFromIndex: number): Observable<PlaylistSongModel[]>{
    return this.httpClient.get<PlaylistSongModel[]>(`${environment.apiUrl}/${APIROUTES.queue}/create/album/${albumId}?randomize=${randomize}&playFromIndex=${playFromIndex}`,{
      withCredentials: true
    })
  }

  public CreateQueueFromPlaylist(playlistId: string, randomize: boolean, sortAfter: string, asc: boolean, playFromOrder: number): Observable<PlaylistSongModel[]>{
    return this.httpClient.get<PlaylistSongModel[]>(`${environment.apiUrl}/${APIROUTES.queue}/create/playlist/${playlistId}?randomize=${randomize}&sortAfter=${sortAfter}&asc=${asc}&playFromOrder=${playFromOrder}`,{
      withCredentials: true
    })
  }

  public CreateQueueFromFavorites(randomize: boolean, sortAfter: string, asc: boolean, playFromOrder: number): Observable<PlaylistSongModel[]>{
    return this.httpClient.get<PlaylistSongModel[]>(`${environment.apiUrl}/${APIROUTES.queue}/create/favorites?randomize=${randomize}&sortAfter=${sortAfter}&asc=${asc}&playFromOrder=${playFromOrder}`,{
      withCredentials: true
    })
  }

  public RandomizeQueueFromAlbum(albumId: string): Observable<PlaylistSongModel[]>{
    return this.httpClient.get<PlaylistSongModel[]>(`${environment.apiUrl}/${APIROUTES.queue}/randomize?albumId=${albumId}`,{
      withCredentials: true
    })
  }

  public RandomizeQueueFromFavorites(): Observable<PlaylistSongModel[]>{
    return this.httpClient.get<PlaylistSongModel[]>(`${environment.apiUrl}/${APIROUTES.queue}/randomize`,{
      withCredentials: true
    })
  }

  public RandomizeQueueFromPlaylist(playlistId: string): Observable<PlaylistSongModel[]>{
    return this.httpClient.get<PlaylistSongModel[]>(`${environment.apiUrl}/${APIROUTES.queue}/randomize?playlistId=${playlistId}`,{
      withCredentials: true
    })
  }

  public RandomizeQueueFromSong(songId: string): Observable<PlaylistSongModel[]>{
    return this.httpClient.get<PlaylistSongModel[]>(`${environment.apiUrl}/${APIROUTES.queue}/randomize?songId=${songId}`,{
      withCredentials: true
    })
  }

  public GetCurrentQueue(): Observable<PlaylistSongModel[]>{
    return this.httpClient.get<PlaylistSongModel[]>(`${environment.apiUrl}/${APIROUTES.queue}`,{
      withCredentials: true
    })
  }

  public GetCurrentSong(): Observable<PlaylistSongModel>{
    return this.httpClient.get<PlaylistSongModel>(`${environment.apiUrl}/${APIROUTES.queue}/playing-item`,{
      withCredentials: true
    })
  }

  public GetSongWithIndexFromQueue(index: number): Observable<PlaylistSongModel>{
    return this.httpClient.get<PlaylistSongModel>(`${environment.apiUrl}/${APIROUTES.queue}/song?index=${index}`,{
      withCredentials: true
    })
  }

  public ClearQueue(): Observable<object>{
    return this.httpClient.delete<object>(`${environment.apiUrl}/${APIROUTES.queue}`,{
      withCredentials: true
    })
  }

  public SkipForwardInQueue(index: number): Observable<PlaylistSongModel>{

    if (index == -1) {
      return this.httpClient.get<PlaylistSongModel>(`${environment.apiUrl}/${APIROUTES.queue}/next`,{
        withCredentials: true
      })
    }

    return this.httpClient.get<PlaylistSongModel>(`${environment.apiUrl}/${APIROUTES.queue}/next?index=${index}`,{
      withCredentials: true
    })
  }

  public SkipBackInQueue(): Observable<PlaylistSongModel>{
    return this.httpClient.get<PlaylistSongModel>(`${environment.apiUrl}/${APIROUTES.queue}/prev`,{
      withCredentials: true
    })
  }

  public RemoveSongsFromQueue(indexList: number[]): Observable<PlaylistSongModel[]>{
    return this.httpClient.post<PlaylistSongModel[]>(`${environment.apiUrl}/${APIROUTES.queue}/remove`, {
      OrderIds: indexList
    },{
      withCredentials: true
    })
  }

  public PushSongInQueue(srcIndex: number, targetIndex: number): Observable<PlaylistSongModel[]>{
    return this.httpClient.get<PlaylistSongModel[]>(`${environment.apiUrl}/${APIROUTES.queue}/push?srcIndex=${srcIndex}&targetIndex=${targetIndex}`,{
      withCredentials: true
    })
  }
}
