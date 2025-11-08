import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotStartedtasksComponent } from './not-startedtasks.component';

describe('NotStartedtasksComponent', () => {
  let component: NotStartedtasksComponent;
  let fixture: ComponentFixture<NotStartedtasksComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotStartedtasksComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NotStartedtasksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
