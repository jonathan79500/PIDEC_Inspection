import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SentinspectionsComponent } from './sentinspections.component';

describe('SentinspectionsComponent', () => {
  let component: SentinspectionsComponent;
  let fixture: ComponentFixture<SentinspectionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SentinspectionsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SentinspectionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
