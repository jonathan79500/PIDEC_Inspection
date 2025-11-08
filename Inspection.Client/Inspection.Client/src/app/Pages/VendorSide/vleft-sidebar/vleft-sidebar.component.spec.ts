import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VleftSidebarComponent } from './vleft-sidebar.component';

describe('VleftSidebarComponent', () => {
  let component: VleftSidebarComponent;
  let fixture: ComponentFixture<VleftSidebarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VleftSidebarComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(VleftSidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
