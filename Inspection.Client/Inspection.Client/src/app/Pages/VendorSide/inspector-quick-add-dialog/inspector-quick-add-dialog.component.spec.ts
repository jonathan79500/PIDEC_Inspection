import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InspectorQuickAddDialogComponent } from './inspector-quick-add-dialog.component';

describe('InspectorQuickAddDialogComponent', () => {
  let component: InspectorQuickAddDialogComponent;
  let fixture: ComponentFixture<InspectorQuickAddDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InspectorQuickAddDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InspectorQuickAddDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
