import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SongQueueComponent } from './song-queue.component';

describe('SongQueueComponent', () => {
  let component: SongQueueComponent;
  let fixture: ComponentFixture<SongQueueComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SongQueueComponent]
    });
    fixture = TestBed.createComponent(SongQueueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
