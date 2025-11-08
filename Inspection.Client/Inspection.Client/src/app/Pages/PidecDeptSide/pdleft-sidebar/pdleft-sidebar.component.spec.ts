import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PdleftSidebarComponent } from './pdleft-sidebar.component';

describe('PdleftSidebarComponent', () => {
  let component: PdleftSidebarComponent;
  let fixture: ComponentFixture<PdleftSidebarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PdleftSidebarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PdleftSidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
