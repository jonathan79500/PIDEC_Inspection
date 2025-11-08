import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PmainComponent } from './pmain.component';

describe('PmainComponent', () => {
  let component: PmainComponent;
  let fixture: ComponentFixture<PmainComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PmainComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(PmainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
