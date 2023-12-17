import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AmbientalSensorComponent } from './ambiental-sensor.component';

describe('AmbientalSensorComponent', () => {
  let component: AmbientalSensorComponent;
  let fixture: ComponentFixture<AmbientalSensorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AmbientalSensorComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AmbientalSensorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
