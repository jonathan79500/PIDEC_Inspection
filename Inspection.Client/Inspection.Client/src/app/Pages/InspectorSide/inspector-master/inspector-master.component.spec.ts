import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InspectorMasterComponent } from './inspector-master.component';

describe('InspectorMasterComponent', () => {
  let component: InspectorMasterComponent;
  let fixture: ComponentFixture<InspectorMasterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InspectorMasterComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InspectorMasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
