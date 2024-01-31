import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SharedDevicesComponent } from './shared-devices.component';

describe('SharedDevicesComponent', () => {
  let component: SharedDevicesComponent;
  let fixture: ComponentFixture<SharedDevicesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SharedDevicesComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SharedDevicesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
