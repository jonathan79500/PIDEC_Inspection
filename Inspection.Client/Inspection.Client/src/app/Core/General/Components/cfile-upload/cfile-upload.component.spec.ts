import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CFileUploadComponent } from './cfile-upload.component';

describe('CFileUploadComponent', () => {
  let component: CFileUploadComponent;
  let fixture: ComponentFixture<CFileUploadComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CFileUploadComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CFileUploadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
