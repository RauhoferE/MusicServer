import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AudioLoopComponent } from './audio-loop.component';

describe('AudioLoopComponent', () => {
  let component: AudioLoopComponent;
  let fixture: ComponentFixture<AudioLoopComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AudioLoopComponent]
    });
    fixture = TestBed.createComponent(AudioLoopComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
