import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TimeSessionComponent } from './time-session.component';

describe('TimeSessionComponent', () => {
  let component: TimeSessionComponent;
  let fixture: ComponentFixture<TimeSessionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TimeSessionComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TimeSessionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
