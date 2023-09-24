import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'; 
import { APIROUTES } from '../constants/api-routes';
import { environment } from 'src/environments/environment';
import { RegisterModel } from '../models/user-models';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  constructor(private httpClient: HttpClient) { }

  login(email: string, password: string): Observable<Object>{
    return this.httpClient.post(`${environment.apiUrl}/${APIROUTES.authentication}/login`, {
      Email: email,
      Password: password
    },{
      withCredentials: true
    });
  };

  logout(): Observable<Object>{
    return this.httpClient.get(`${environment.apiUrl}/${APIROUTES.authentication}/logout`,{
      withCredentials: true
    });
  };

  register(register: RegisterModel): Observable<Object>{
    return this.httpClient.post(`${environment.apiUrl}/${APIROUTES.authentication}/register`, register);
  }


}
