import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChooseDeviceTypeDialogComponent } from './choose-device-type-dialog.component';

describe('ChooseDeviceTypeDialogComponent', () => {
  let component: ChooseDeviceTypeDialogComponent;
  let fixture: ComponentFixture<ChooseDeviceTypeDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChooseDeviceTypeDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChooseDeviceTypeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
