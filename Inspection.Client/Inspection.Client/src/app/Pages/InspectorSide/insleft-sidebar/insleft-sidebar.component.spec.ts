import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InsleftSidebarComponent } from './insleft-sidebar.component';

describe('InsleftSidebarComponent', () => {
  let component: InsleftSidebarComponent;
  let fixture: ComponentFixture<InsleftSidebarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InsleftSidebarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InsleftSidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
