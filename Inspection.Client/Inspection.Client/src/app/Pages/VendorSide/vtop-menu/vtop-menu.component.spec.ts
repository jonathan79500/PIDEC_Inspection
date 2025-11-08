import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VtopMenuComponent } from './vtop-menu.component';

describe('VtopMenuComponent', () => {
  let component: VtopMenuComponent;
  let fixture: ComponentFixture<VtopMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VtopMenuComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(VtopMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
