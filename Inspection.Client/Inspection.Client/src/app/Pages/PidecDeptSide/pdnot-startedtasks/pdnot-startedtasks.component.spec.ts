import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PdnotStartedtasksComponent } from './pdnot-startedtasks.component';

describe('PdnotStartedtasksComponent', () => {
  let component: PdnotStartedtasksComponent;
  let fixture: ComponentFixture<PdnotStartedtasksComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PdnotStartedtasksComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PdnotStartedtasksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
