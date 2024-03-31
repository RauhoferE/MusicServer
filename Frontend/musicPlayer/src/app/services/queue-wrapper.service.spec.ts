import { TestBed } from '@angular/core/testing';

import { QueueWrapperService } from './queue-wrapper.service';

describe('QueueWrapperService', () => {
  let service: QueueWrapperService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(QueueWrapperService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
