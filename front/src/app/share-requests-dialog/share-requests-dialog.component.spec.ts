import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShareRequestsDialogComponent } from './share-requests-dialog.component';

describe('ShareRequestsDialogComponent', () => {
  let component: ShareRequestsDialogComponent;
  let fixture: ComponentFixture<ShareRequestsDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ShareRequestsDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShareRequestsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
