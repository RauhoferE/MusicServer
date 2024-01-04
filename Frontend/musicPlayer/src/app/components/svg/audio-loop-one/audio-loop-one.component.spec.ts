import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AudioLoopOneComponent } from './audio-loop-one.component';

describe('AudioLoopOneComponent', () => {
  let component: AudioLoopOneComponent;
  let fixture: ComponentFixture<AudioLoopOneComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AudioLoopOneComponent]
    });
    fixture = TestBed.createComponent(AudioLoopOneComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
