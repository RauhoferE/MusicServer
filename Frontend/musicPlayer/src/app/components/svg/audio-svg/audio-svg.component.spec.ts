import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AudioSvgComponent } from './audio-svg.component';

describe('AudioSvgComponent', () => {
  let component: AudioSvgComponent;
  let fixture: ComponentFixture<AudioSvgComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AudioSvgComponent]
    });
    fixture = TestBed.createComponent(AudioSvgComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
