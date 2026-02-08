import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InspectionPrintComponent } from './inspection-print.component';

describe('InspectionPrintComponent', () => {
  let component: InspectionPrintComponent;
  let fixture: ComponentFixture<InspectionPrintComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InspectionPrintComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InspectionPrintComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
