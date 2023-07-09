import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'; 

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  constructor(private httpClient: HttpClient) { }

  async login(email: string, password: string): Promise<void>{
    this.httpClient.post('', {
      Email: email,
      Password: password
    })
  };
}
