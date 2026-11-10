import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentTypeManagementComponent } from './document-type-management.component';

describe('DocumentTypeManagementComponent', () => {
  let component: DocumentTypeManagementComponent;
  let fixture: ComponentFixture<DocumentTypeManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DocumentTypeManagementComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DocumentTypeManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
