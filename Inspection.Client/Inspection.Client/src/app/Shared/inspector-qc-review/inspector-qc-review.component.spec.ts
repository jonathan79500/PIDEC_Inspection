import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InspectorQcReviewComponent } from './inspector-qc-review.component';

describe('InspectorQcReviewComponent', () => {
  let component: InspectorQcReviewComponent;
  let fixture: ComponentFixture<InspectorQcReviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InspectorQcReviewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InspectorQcReviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
