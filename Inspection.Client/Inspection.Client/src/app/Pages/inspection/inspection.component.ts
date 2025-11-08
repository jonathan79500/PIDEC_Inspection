import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { VnewinspectionComponent } from '../VendorSide/vnewinspection/vnewinspection.component';

@Component({
  selector: 'app-inspection',
  imports: [CommonModule,ReactiveFormsModule,FormsModule],
  templateUrl: './inspection.component.html',
  styleUrl: './inspection.component.css'
})
export class InspectionComponent {

  maxSections = 2;
  inspectionNotifications: VnewinspectionComponent[];

}
