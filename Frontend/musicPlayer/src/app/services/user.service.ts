import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AllFollowedEntitiesModel } from '../models/user-models';
import { environment } from 'src/environments/environment';
import { APIROUTES } from '../constants/api-routes';

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
}
