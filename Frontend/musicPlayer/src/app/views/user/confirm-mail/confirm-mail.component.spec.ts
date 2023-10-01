import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmMailComponent } from './confirm-mail.component';

describe('ConfirmMailComponent', () => {
  let component: ConfirmMailComponent;
  let fixture: ComponentFixture<ConfirmMailComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ConfirmMailComponent]
    });
    fixture = TestBed.createComponent(ConfirmMailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
