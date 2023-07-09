import { CanActivateFn } from '@angular/router';

export const isUnknownGuard: CanActivateFn = (route, state) => {
  return true;
};
