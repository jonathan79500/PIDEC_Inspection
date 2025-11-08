import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PtopMenuComponent } from './ptop-menu.component';

describe('PtopMenuComponent', () => {
  let component: PtopMenuComponent;
  let fixture: ComponentFixture<PtopMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PtopMenuComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(PtopMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
