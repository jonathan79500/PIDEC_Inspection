import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PidecdeptMasterComponent } from './pidecdept-master.component';

describe('PidecdeptMasterComponent', () => {
  let component: PidecdeptMasterComponent;
  let fixture: ComponentFixture<PidecdeptMasterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PidecdeptMasterComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PidecdeptMasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
