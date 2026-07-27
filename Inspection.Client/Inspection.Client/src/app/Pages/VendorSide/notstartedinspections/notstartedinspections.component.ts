
import { InspectionNotificationDataSummary } from './../../../Core/Models/InspectionNotificationDataSummary.model';
import { AfterViewInit, Component, ElementRef, OnInit, ViewChild, viewChild } from '@angular/core';
import {  inject} from '@angular/core';
import { currentstateStore } from '../../../Stores/currentstate.store';
import { MatTable, MatTableDataSource, MatTableModule} from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { Inspection } from '../../../Core/Models/Inspection.model';
import { InspectionService } from '../../../Core/Services/inspection.service';
import { Router } from '@angular/router';



@Component({
  selector: 'app-notstartedinspections',
  imports: [MatTableModule],
  templateUrl: './notstartedinspections.component.html',
  styleUrl: './notstartedinspections.component.css'
})
export class NotstartedinspectionsComponent implements OnInit{


  @ViewChild('inspectionTable', { static: true }) inspectionTable: MatTable<any>;
    @ViewChild(MatSort) sort !: MatSort;

    clickedRows = new Set<Inspection>();
    displayedColumns: string[] = ['no','relatedProjectName', 'notificationNumber','relatedOrderName','notificationTypes', 'notificationSubjects','issueDate','persianIssueDate'];
    dataSource: MatTableDataSource<InspectionNotificationDataSummary> = new MatTableDataSource<InspectionNotificationDataSummary>();


     themeClass0: string =
      "ag-theme-quartz";
    public themeClass: string = "ag-theme-quartz";
    public inspections:InspectionNotificationDataSummary[];


    @ViewChild('myGrid',{static:true})  gridDiv: ElementRef;


    columnDefs: [
      { field: "relatedProjectName", width: 150 },
      { field: "relatedOrderName", width: 150 },
      {field: "relatedOrderNumber",width:200},
      { field: "notificationNumber", width: 90 },
      { field: "issueDate", width: 90 },
      { field: "persianIssueDate", width: 90 },
      { field: "inspectionStartDate", width: 90 },
      { field: "persianInspectionStartDate", width: 90 },
      { field: "inspectionEndDate", width: 90 },
      { field: "persianInspectionEndDate", width: 90 },
      { field: "notificationTypes", width: 90 },
      { field: "notificationSubjects", width: 90 },
      { field: "notificationPIDECContacts", width: 90 },
       { field: "notificationOwnerContacts", width: 90 },
        { field: "notificationMCContacts", width: 90 },

    ]

  currentAppState = inject(currentstateStore);
  constructor(private inspectionService: InspectionService, private router: Router){

  }

  ngOnInit(): void {
    let currentUser_userName = this.currentAppState.currentuser().userName;
    this.inspectionService.getNotStartedInspectionsByUserName(currentUser_userName).subscribe((draftedInspections)=>{

    this.inspections = draftedInspections;
      this.dataSource.data = draftedInspections;
      this.inspectionTable.dataSource = this.dataSource;
    }

    );
  }

   clickInspectionRow(cRow: InspectionNotificationDataSummary): void {

      // console.log('in selected Row',cRow);
      // this.vendorSharedData.setInspection(cRow);

     // this.clickedRows.add(cRow);

      if (Request != null) {

        //let w = window.open("editInspection/" + cRow.inspectionNotificationId, "", "width:100px;height:200px;");
        //let w = window.open("newInspection/" + cRow.id, "", "width:100px;height:200px;");
       // w.opener.data = cRow;
       this.router.navigate(["vendor/editInspection/"+cRow.requestId+"/" + cRow.inspectionNotificationId]);
      }

    }
}
