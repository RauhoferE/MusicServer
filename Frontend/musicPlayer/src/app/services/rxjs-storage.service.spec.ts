import { TestBed } from '@angular/core/testing';

import { RxjsStorageService } from './rxjs-storage.service';

describe('RxjsStorageService', () => {
  let service: RxjsStorageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RxjsStorageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
