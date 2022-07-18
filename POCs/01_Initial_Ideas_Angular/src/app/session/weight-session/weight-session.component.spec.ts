import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WeightSessionComponent } from './weight-session.component';

describe('WeightSessionComponent', () => {
  let component: WeightSessionComponent;
  let fixture: ComponentFixture<WeightSessionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WeightSessionComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WeightSessionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
