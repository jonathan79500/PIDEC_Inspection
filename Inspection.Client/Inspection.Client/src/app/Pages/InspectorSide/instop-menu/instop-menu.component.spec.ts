import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstopMenuComponent } from './instop-menu.component';

describe('InstopMenuComponent', () => {
  let component: InstopMenuComponent;
  let fixture: ComponentFixture<InstopMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InstopMenuComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InstopMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
