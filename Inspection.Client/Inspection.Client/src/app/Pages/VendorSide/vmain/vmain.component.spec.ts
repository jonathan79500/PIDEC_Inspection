import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VmainComponent } from './vmain.component';

describe('VmainComponent', () => {
  let component: VmainComponent;
  let fixture: ComponentFixture<VmainComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VmainComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(VmainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
