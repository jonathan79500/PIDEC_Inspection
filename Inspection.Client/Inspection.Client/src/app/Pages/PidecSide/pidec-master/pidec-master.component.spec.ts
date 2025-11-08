import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PidecMasterComponent } from './pidec-master.component';

describe('PidecMasterComponent', () => {
  let component: PidecMasterComponent;
  let fixture: ComponentFixture<PidecMasterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PidecMasterComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(PidecMasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
