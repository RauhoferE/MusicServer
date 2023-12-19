import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AudioOffSvgComponent } from './audio-off-svg.component';

describe('AudioOffSvgComponent', () => {
  let component: AudioOffSvgComponent;
  let fixture: ComponentFixture<AudioOffSvgComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AudioOffSvgComponent]
    });
    fixture = TestBed.createComponent(AudioOffSvgComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
