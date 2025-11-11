import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

@Component({
  selector: 'app-inspection-report',
  imports: [FormsModule,ReactiveFormsModule,CommonModule],
  templateUrl: './inspection-report.component.html',
  styleUrl: './inspection-report.component.css'
})
export class InspectionReportComponent implements OnInit{



  ngOnInit(): void {

  }

}
