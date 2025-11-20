import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotstartedtasksComponent } from './notstartedtasks.component';

describe('NotstartedtasksComponent', () => {
  let component: NotstartedtasksComponent;
  let fixture: ComponentFixture<NotstartedtasksComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotstartedtasksComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NotstartedtasksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
