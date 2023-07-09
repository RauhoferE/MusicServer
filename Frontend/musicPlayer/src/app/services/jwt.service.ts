import { Injectable } from '@angular/core';
import jwtDecode from 'jwt-decode';
import { CookieService } from 'ngx-cookie-service';
import { AUTH_COOKIE_NAME, CLAIMS_COOKIE_NAME } from '../constants/cookie-names';

@Injectable({
  providedIn: 'root'
})
export class JwtService {

  constructor(private cookieService: CookieService) { }

  getUserName(): string{
    if (!this.cookieService.check(CLAIMS_COOKIE_NAME)) {
      return 'NOT_FOUND';
    }

    var jwt: any = jwtDecode(this.cookieService.get(CLAIMS_COOKIE_NAME));
    return jwt['name'] as string
  }

  getUserEmail(): string{
    if (!this.cookieService.check(CLAIMS_COOKIE_NAME)) {
      return 'NOT_FOUND';
    }

    var jwt: any = jwtDecode(this.cookieService.get(CLAIMS_COOKIE_NAME));
    return jwt['email'] as string
  }

  getUserRoles(): string[] {
    if (!this.cookieService.check(CLAIMS_COOKIE_NAME)) {
      return [];
    }

    var jwt: any = jwtDecode(this.cookieService.get(CLAIMS_COOKIE_NAME));
    if (Array.isArray(jwt['roles'])) {
      return jwt['roles'] as string[];
    }

    return [jwt['roles'] as string];
  }
}
