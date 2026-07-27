import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CirculationModalComponent } from './circulation-modal.component';

describe('CirculationModalComponent', () => {
  let component: CirculationModalComponent;
  let fixture: ComponentFixture<CirculationModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CirculationModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CirculationModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
