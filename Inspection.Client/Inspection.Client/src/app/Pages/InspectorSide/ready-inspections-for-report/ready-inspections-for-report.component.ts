import { InspectionNotificationDataSummary } from './../../../Core/Models/InspectionNotificationDataSummary.model';
import { AfterViewInit, Component, ViewChild } from '@angular/core';
import {  inject} from '@angular/core';
import { currentstateStore } from '../../../Stores/currentstate.store';
import { MatTable, MatTableDataSource, MatTableModule} from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { Inspection } from '../../../Core/Models/Inspection.model';
import { InspectionService } from '../../../Core/Services/inspection.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-ready-inspections-for-report',
  imports: [MatTableModule],
  templateUrl: './ready-inspections-for-report.component.html',
  styleUrl: './ready-inspections-for-report.component.css'
})
export class ReadyInspectionsForReportComponent {
@ViewChild('inspectionTable', { static: true }) inspectionTable: MatTable<any>;
    @ViewChild(MatSort) sort !: MatSort;

    clickedRows = new Set<Inspection>();
    displayedColumns: string[] = ['no','relatedProjectName', 'notificationNumber','relatedOrderName','notificationTypes', 'notificationSubjects','issueDate','persianIssueDate'];
    dataSource: MatTableDataSource<InspectionNotificationDataSummary> = new MatTableDataSource<InspectionNotificationDataSummary>();
     public inspections:InspectionNotificationDataSummary[];


  currentAppState = inject(currentstateStore);
  constructor(private inspectionService: InspectionService, private router: Router){

  }

  ngOnInit(): void {
    let currentUser_userName = this.currentAppState.currentuser().userName;
    let currentProjectId = this.currentAppState.currentproject().id;
    this.inspectionService.getReadyInspectionsForReportByUserName(currentProjectId).subscribe((draftedInspections)=>{

    this.inspections = draftedInspections;
      this.dataSource.data = draftedInspections;
      this.inspectionTable.dataSource = this.dataSource;
    }

    );
  }

   clickInspectionRow(cRow: InspectionNotificationDataSummary): void {
      if (Request != null) {
       //this.router.navigate(["inspector/editInspection/" + cRow.inspectionNotificationId]);
        this.router.navigate(["inspector/editInspection/"+cRow.requestId+"/" + cRow.inspectionNotificationId]);
      }

    }
}
