import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NativefileuploadComponent } from './nativefileupload.component';

describe('NativefileuploadComponent', () => {
  let component: NativefileuploadComponent;
  let fixture: ComponentFixture<NativefileuploadComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NativefileuploadComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(NativefileuploadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
