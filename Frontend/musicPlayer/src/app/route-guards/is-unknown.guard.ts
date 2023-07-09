import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { AUTH_COOKIE_NAME } from '../constants/cookie-names';

export const isUnknownGuard: CanActivateFn = (route, state) => {
  if (inject(CookieService).check(AUTH_COOKIE_NAME)) {
    return inject(Router).parseUrl('/playlists');
  }

  return true;
};
