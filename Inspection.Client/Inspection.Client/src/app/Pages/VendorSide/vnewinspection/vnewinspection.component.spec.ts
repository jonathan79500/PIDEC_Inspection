import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewinspectionComponent } from './vnewinspection.component';

describe('NewinspectionComponent', () => {
  let component: NewinspectionComponent;
  let fixture: ComponentFixture<NewinspectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NewinspectionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NewinspectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
