import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PdMainComponent } from './pd-main.component';

describe('PdMainComponent', () => {
  let component: PdMainComponent;
  let fixture: ComponentFixture<PdMainComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PdMainComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PdMainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
