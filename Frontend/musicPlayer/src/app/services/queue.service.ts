import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PlaylistSongModel, QueueSongModel } from '../models/playlist-models';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { APIROUTES } from '../constants/api-routes';
import { QueueModel } from '../models/storage';

@Injectable({
  providedIn: 'root'
})
export class QueueService {

  constructor(private httpClient: HttpClient) { }

  public CreateQueueFromAlbum(albumId: string, randomize: boolean, loopMode: string, playFromIndex: number): Observable<PlaylistSongModel>{
    return this.httpClient.get<PlaylistSongModel>(`${environment.apiUrl}/${APIROUTES.queue}/create/album/${albumId}?randomize=${randomize}&loopMode=${loopMode}&playFromIndex=${playFromIndex}`,{
      withCredentials: true
    })
  }

  public CreateQueueFromSingleSong(songId: string, randomize: boolean, loopMode: string): Observable<PlaylistSongModel>{
    return this.httpClient.get<PlaylistSongModel>(`${environment.apiUrl}/${APIROUTES.queue}/create/song/${songId}?randomize=${randomize}&loopMode=${loopMode}`,{
      withCredentials: true
    })
  }

  public CreateQueueFromPlaylist(playlistId: string, randomize: boolean,loopMode: string, sortAfter: string, asc: boolean, playFromOrder: number): Observable<PlaylistSongModel>{
    return this.httpClient.get<PlaylistSongModel>(`${environment.apiUrl}/${APIROUTES.queue}/create/playlist/${playlistId}?randomize=${randomize}&loopMode=${loopMode}&sortAfter=${sortAfter}&asc=${asc}&playFromOrder=${playFromOrder}`,{
      withCredentials: true
    })
  }

  public CreateQueueFromFavorites(randomize: boolean,loopMode: string, sortAfter: string, asc: boolean, playFromOrder: number): Observable<PlaylistSongModel>{
    return this.httpClient.get<PlaylistSongModel>(`${environment.apiUrl}/${APIROUTES.queue}/create/favorites?randomize=${randomize}&loopMode=${loopMode}&sortAfter=${sortAfter}&asc=${asc}&playFromOrder=${playFromOrder}`,{
      withCredentials: true
    })
  }

  public AddAlbumToQueue(albumId: string): Observable<object>{
    return this.httpClient.get<object>(`${environment.apiUrl}/${APIROUTES.queue}/add/album/${albumId}`,{
      withCredentials: true
    })
  }

  public AddPlaylistToQueue(playlistId: string): Observable<object>{
    return this.httpClient.get<object>(`${environment.apiUrl}/${APIROUTES.queue}/add/playlist/${playlistId}`,{
      withCredentials: true
    })
  }

  public ChangeQueue(randomize: boolean): Observable<PlaylistSongModel>{
    return this.httpClient.get<PlaylistSongModel>(`${environment.apiUrl}/${APIROUTES.queue}/change?randomize=${randomize}`,{
      withCredentials: true
    })
  }

  public GetCurrentQueue(): Observable<QueueSongModel[]>{
    return this.httpClient.get<QueueSongModel[]>(`${environment.apiUrl}/${APIROUTES.queue}`,{
      withCredentials: true
    })
  }

  public GetCurrentSong(): Observable<PlaylistSongModel>{
    return this.httpClient.get<PlaylistSongModel>(`${environment.apiUrl}/${APIROUTES.queue}/playing-item`,{
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

  public RemoveSongsFromQueue(indexList: number[]): Observable<object>{
    return this.httpClient.post<object>(`${environment.apiUrl}/${APIROUTES.queue}/remove`, {
      OrderIds: indexList
    },{
      withCredentials: true
    })
  }

  public AddSongsToQueue(ids: string[]): Observable<object>{
    return this.httpClient.post<object>(`${environment.apiUrl}/${APIROUTES.queue}/add`, {
      SongIds: ids
    },{
      withCredentials: true
    })
  }

  public PushSongInQueue(srcIndex: number, targetIndex: number, markAsManuallyAdded: number): Observable<QueueSongModel[]>{
    return this.httpClient.get<QueueSongModel[]>(`${environment.apiUrl}/${APIROUTES.queue}/push?srcIndex=${srcIndex}&targetIndex=${targetIndex}&markAsManuallyAdded=${markAsManuallyAdded}`,{
      withCredentials: true
    })
  }

  public GetQueueData(): Observable<QueueModel>{
    return this.httpClient.get<QueueModel>(`${environment.apiUrl}/${APIROUTES.queue}/data`,{
      withCredentials: true
    })
  }

  public UpdateLoopMode(loopMode: string): Observable<object>{
    return this.httpClient.get<object>(`${environment.apiUrl}/${APIROUTES.queue}/loop?loopMode=${loopMode}`,
    {
      withCredentials: true
    })
  }
}
