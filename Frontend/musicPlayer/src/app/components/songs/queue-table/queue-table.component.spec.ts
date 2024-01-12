import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QueueTableComponent } from './queue-table.component';

describe('QueueTableComponent', () => {
  let component: QueueTableComponent;
  let fixture: ComponentFixture<QueueTableComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [QueueTableComponent]
    });
    fixture = TestBed.createComponent(QueueTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
