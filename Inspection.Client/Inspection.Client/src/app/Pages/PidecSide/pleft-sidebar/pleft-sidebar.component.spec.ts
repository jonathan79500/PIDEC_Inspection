import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PleftSidebarComponent } from './pleft-sidebar.component';

describe('PleftSidebarComponent', () => {
  let component: PleftSidebarComponent;
  let fixture: ComponentFixture<PleftSidebarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PleftSidebarComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(PleftSidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
