import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { APIROUTES } from '../constants/api-routes';

@Injectable({
  providedIn: 'root'
})
export class FileService {

  constructor(private httpClient: HttpClient) { }

  public ChangeProfilePicture(file: File): Observable<object>{
    const formData = new FormData();
    formData.append('file', file);

    return this.httpClient.post<object>(`${environment.apiUrl}/${APIROUTES.file}/me`,
    formData,
    {
      withCredentials: true,
    })
  }
}
