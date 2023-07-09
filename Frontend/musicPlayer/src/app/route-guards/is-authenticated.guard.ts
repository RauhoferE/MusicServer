import { Injectable, inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateFn, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { Observable } from 'rxjs';
import { AUTH_COOKIE_NAME } from '../constants/cookie-names';

export const isAuthenticatedGuard: CanActivateFn = (route, state) => {
  if (inject(CookieService).check(AUTH_COOKIE_NAME)) {
    return true;
  }

  return inject(Router).parseUrl('/login');
};
  

