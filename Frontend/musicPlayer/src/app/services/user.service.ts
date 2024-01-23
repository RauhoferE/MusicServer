import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AllFollowedEntitiesModel, UserModel } from '../models/user-models';
import { environment } from 'src/environments/environment';
import { APIROUTES } from '../constants/api-routes';
import { GuidNameModel } from '../models/playlist-models';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private httpClient: HttpClient) { }

  public GetFollowedEntities(filter:string, searchTerm:string) : Observable<AllFollowedEntitiesModel>{
    return this.httpClient.get<AllFollowedEntitiesModel>(`${environment.apiUrl}/${APIROUTES.user}/followed/all?filter=${filter}&searchTerm=${searchTerm}`,{
      withCredentials: true
    })
  }

  public SuscribeToArtist(artistId: string): Observable<object>{
    return this.httpClient.get<object>(`${environment.apiUrl}/${APIROUTES.user}/subscribe/artist/${artistId}`,{
      withCredentials: true
    })
  }

  public UnSuscribeFromArtist(artistId: string): Observable<object>{
    return this.httpClient.delete<object>(`${environment.apiUrl}/${APIROUTES.user}/subscribe/artist/${artistId}`,{
      withCredentials: true
    })
  }

  public SuscribeToUser(userId: string): Observable<object>{
    return this.httpClient.get<object>(`${environment.apiUrl}/${APIROUTES.user}/subscribe/user/${userId}`,{
      withCredentials: true
    })
  }

  public UnSuscribeFromUser(userId: string): Observable<object>{
    return this.httpClient.delete<object>(`${environment.apiUrl}/${APIROUTES.user}/subscribe/user/${userId}`,{
      withCredentials: true
    })
  }

  public ReceiveNotficationsFromArtist(artistId: string): Observable<object>{
    return this.httpClient.get<object>(`${environment.apiUrl}/${APIROUTES.user}/notifications/artist/${artistId}`,{
      withCredentials: true
    })
  }

  public RemoveNotficationsFromArtist(artistId: string): Observable<object>{
    return this.httpClient.delete<object>(`${environment.apiUrl}/${APIROUTES.user}/notifications/artist/${artistId}`,{
      withCredentials: true
    })
  }

  public GetSubscribedArtists(userId: string, query: string): Observable<GuidNameModel[]>{
    return this.httpClient.get<GuidNameModel[]>(`${environment.apiUrl}/${APIROUTES.user}/followed/artists/${userId}?query=${query}`,{
      withCredentials: true
    })
  }

  public GetSubscribedUsers(userId: string, query: string): Observable<GuidNameModel[]>{
    return this.httpClient.get<GuidNameModel[]>(`${environment.apiUrl}/${APIROUTES.user}/followed/users/${userId}?query=${query}`,{
      withCredentials: true
    })
  }

  public SubscribeToUser(userId: string): Observable<object>{
    return this.httpClient.get<object>(`${environment.apiUrl}/${APIROUTES.user}/subscribe/user/${userId}`,{
      withCredentials: true
    })
  }

  public UnSubscribeFromUser(userId: string): Observable<object>{
    return this.httpClient.delete<object>(`${environment.apiUrl}/${APIROUTES.user}/subscribe/user/${userId}`,{
      withCredentials: true
    })
  }

  public ReceiveNotficationsFromUser(userId: string): Observable<object>{
    return this.httpClient.get<object>(`${environment.apiUrl}/${APIROUTES.user}/notifications/user/${userId}`,{
      withCredentials: true
    })
  }

  public RemoveNotficationsFromUser(userId: string): Observable<object>{
    return this.httpClient.delete<object>(`${environment.apiUrl}/${APIROUTES.user}/notifications/user/${userId}`,{
      withCredentials: true
    })
  }

  public SubscribeUserInfo(userId: string): Observable<UserModel>{
    return this.httpClient.get<UserModel>(`${environment.apiUrl}/${APIROUTES.user}/subscribe/user/info/${userId}`,{
      withCredentials: true
    })
  }


}
