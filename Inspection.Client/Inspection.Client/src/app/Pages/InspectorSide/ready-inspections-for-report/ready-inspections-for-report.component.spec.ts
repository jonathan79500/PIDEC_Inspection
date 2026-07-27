import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReadyInspectionsForReportComponent } from './ready-inspections-for-report.component';

describe('ReadyInspectionsForReportComponent', () => {
  let component: ReadyInspectionsForReportComponent;
  let fixture: ComponentFixture<ReadyInspectionsForReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReadyInspectionsForReportComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ReadyInspectionsForReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
