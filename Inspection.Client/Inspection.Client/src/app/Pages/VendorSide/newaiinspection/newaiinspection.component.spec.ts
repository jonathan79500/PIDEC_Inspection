import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewaiinspectionComponent } from './newaiinspection.component';

describe('NewaiinspectionComponent', () => {
  let component: NewaiinspectionComponent;
  let fixture: ComponentFixture<NewaiinspectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NewaiinspectionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NewaiinspectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
