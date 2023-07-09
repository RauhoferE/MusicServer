import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'; 
import { APIROUTES } from '../constants/api-routes';
import { environment } from 'src/environments/environment';
import { RegisterModel } from '../models/user-models';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  constructor(private httpClient: HttpClient) { }

  async login(email: string, password: string): Promise<void>{
    this.httpClient.post(`${environment.apiUrl}/${APIROUTES.authentication}/login`, {
      Email: email,
      Password: password
    })
  };

  async logout(): Promise<void>{
    this.httpClient.get(`${environment.apiUrl}/${APIROUTES.authentication}/logout`,{
      withCredentials: true
    })
  };

  async register(register: RegisterModel): Promise<void>{
    this.httpClient.post(`${environment.apiUrl}/${APIROUTES.authentication}/register`, register)
  }


}
