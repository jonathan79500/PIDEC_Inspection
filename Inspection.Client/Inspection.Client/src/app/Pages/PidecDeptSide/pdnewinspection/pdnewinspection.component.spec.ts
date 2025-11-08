import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PdnewinspectionComponent } from './pdnewinspection.component';

describe('PdnewinspectionComponent', () => {
  let component: PdnewinspectionComponent;
  let fixture: ComponentFixture<PdnewinspectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PdnewinspectionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PdnewinspectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
