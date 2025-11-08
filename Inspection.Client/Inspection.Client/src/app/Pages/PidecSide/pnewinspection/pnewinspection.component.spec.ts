import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PnewinspectionComponent } from './pnewinspection.component';

describe('PNewinspectionComponent', () => {
  let component: PnewinspectionComponent;
  let fixture: ComponentFixture<PnewinspectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PnewinspectionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PnewinspectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
