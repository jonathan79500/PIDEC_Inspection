import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotstartedinspectionsComponent } from './notstartedinspections.component';

describe('NotstartedinspectionsComponent', () => {
  let component: NotstartedinspectionsComponent;
  let fixture: ComponentFixture<NotstartedinspectionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotstartedinspectionsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NotstartedinspectionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
