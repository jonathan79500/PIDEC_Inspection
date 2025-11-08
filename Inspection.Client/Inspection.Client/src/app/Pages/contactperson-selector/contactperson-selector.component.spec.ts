import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ContactpersonSelectorComponent } from './contactperson-selector.component';

describe('ContactpersonSelectorComponent', () => {
  let component: ContactpersonSelectorComponent;
  let fixture: ComponentFixture<ContactpersonSelectorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContactpersonSelectorComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ContactpersonSelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
