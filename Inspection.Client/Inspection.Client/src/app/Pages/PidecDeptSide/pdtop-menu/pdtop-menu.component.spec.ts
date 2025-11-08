import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PdtopMenuComponent } from './pdtop-menu.component';

describe('PdtopMenuComponent', () => {
  let component: PdtopMenuComponent;
  let fixture: ComponentFixture<PdtopMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PdtopMenuComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PdtopMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
