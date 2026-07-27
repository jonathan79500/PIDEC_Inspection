import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InspectionnotificationHistoryComponent } from './inspectionnotification-history.component';

describe('InspectionnotificationHistoryComponent', () => {
  let component: InspectionnotificationHistoryComponent;
  let fixture: ComponentFixture<InspectionnotificationHistoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InspectionnotificationHistoryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InspectionnotificationHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
