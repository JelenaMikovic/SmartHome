import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RejectPropertyDialogComponent } from './reject-property-dialog.component';

describe('RejectPropertyDialogComponent', () => {
  let component: RejectPropertyDialogComponent;
  let fixture: ComponentFixture<RejectPropertyDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RejectPropertyDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RejectPropertyDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
