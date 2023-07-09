import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { isUnknownGuard } from './is-unknown.guard';

describe('isUnknownGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => isUnknownGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
