import { TestBed } from '@angular/core/testing';

import { StreamingClientService } from './streaming-client.service';

describe('StreamingClientService', () => {
  let service: StreamingClientService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StreamingClientService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
