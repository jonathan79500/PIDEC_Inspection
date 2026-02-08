import { Component, ElementRef, inject, ViewChild } from '@angular/core';
import { InspectionNotificationDataSummary } from '../../../Core/Models/InspectionNotificationDataSummary.model';
import { InspectionService } from '../../../Core/Services/inspection.service';
import { Router } from '@angular/router';


import { MatSort } from '@angular/material/sort';
import { Inspection } from '../../../Core/Models/Inspection.model';
import { currentstateStore } from '../../../Stores/currentstate.store';

import {    ColDef,ColGroupDef,  FirstDataRenderedEvent,  GridApi,
  GridOptions,
  PaginationNumberFormatterParams,
  createGrid,} from "ag-grid-community";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";
import { MatTable, MatTableDataSource, MatTableModule} from '@angular/material/table';

function fillLarge() {
  setWidthAndHeight("100%");
}

function fillMedium() {
  setWidthAndHeight("60%");
}

function fillExact(grid) {
  setSize("50%","850px",grid)
  //setWidthAndHeight("800px");
}

function setWidthAndHeight(size: string) {
  var eGridDiv = document.querySelector<HTMLElement>("#myGrid")! as any;
  eGridDiv.style.setProperty("width", size);
  eGridDiv.style.setProperty("height", size);
}
function setSize(width: string,height:string,eGridDiv:any) {
  //var eGridDiv = document.querySelector<HTMLElement>("#myGrid")! as any;
  eGridDiv.style.setProperty("width", width);
  eGridDiv.style.setProperty("height", height);
}
let gridApi: GridApi<Inspection> ;
const gridOptions: GridOptions<Inspection> = {
  columnDefs: [
    { field: "name", width: 150 },
    { field: "notificationNumber", width: 90 },
    { field: "requestInspectionDateStart", width: 150 },
    { field: "requestInspectionPersianDateStart", width: 150 },
    { field: "requestInspectionDateEnd", width: 150 },
    { field: "requestInspectionPersianDateEnd", width: 150 },
    { field: "requestInspectionDueDate", width: 150 },
    { field: "inspectionTimeComment", width: 150 },
    { field: "order.equipmentTagNo", width: 90 },
    { field: "contractShippingDueDate", width: 150 },
    { field: "actualShippingDueDate", width: 150 },
    // { field: "location.country", width: 100 },
    // { field: "location.city", width: 100 },
    { field:"issueDate",width:100},
    { field:"persianIssueDate",width:100},
    { field: "order.itP_NO", width: 100 }
  ],
  domLayout:'normal'
};
@Component({
  selector: 'app-not-startedtasks',
  imports: [MatTableModule],
  templateUrl: './not-startedtasks.component.html',
  styleUrl: './not-startedtasks.component.css'
})
export class NotStartedtasksComponent {
@ViewChild('inspectionTable', { static: true }) inspectionTable: MatTable<any>;
    @ViewChild(MatSort) sort !: MatSort;

    clickedRows = new Set<Inspection>();
    displayedColumns: string[] = ['no','relatedProjectName', 'notificationNumber','relatedOrderName','notificationTypes', 'notificationSubjects','issueDate','persianIssueDate'];
    dataSource: MatTableDataSource<InspectionNotificationDataSummary> = new MatTableDataSource<InspectionNotificationDataSummary>();

    public gridOptions: GridOptions = {
      domLayout: 'autoHeight'
        };

     themeClass0: string =
      "ag-theme-quartz";

   // @ViewChild('agInspections',{static:true}) agInspections :ElementRef;
  public mainGrid:GridApi<InspectionNotificationDataSummary>;

    public themeClass: string = "ag-theme-quartz";
    public inspections:InspectionNotificationDataSummary[];
    //public reorgData :InspectionGridData[]=[];
  public agInspection :GridApi;

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
       this.router.navigate(["pidec/editInspection/" + cRow.inspectionNotificationId]);
      }

    }
}
