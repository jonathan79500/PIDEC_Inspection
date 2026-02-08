import { InspectionNotificationDataSummary } from './../../../Core/Models/InspectionNotificationDataSummary.model';
import { AfterViewInit, Component, ElementRef, OnInit, ViewChild, viewChild, ViewEncapsulation } from '@angular/core';
import {  inject} from '@angular/core';
import { currentstateStore } from '../../../Stores/currentstate.store';


import {    ColDef,ColGroupDef,  FirstDataRenderedEvent,  GridApi,
  GridOptions,
  PaginationNumberFormatterParams,
  createGrid,} from "ag-grid-community";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";



import { MatTable, MatTableDataSource, MatTableModule} from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { Inspection } from '../../../Core/Models/Inspection.model';
import { InspectionService } from '../../../Core/Services/inspection.service';
import { Router } from '@angular/router';


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
  selector: 'app-draftedinspectionslist',
  imports: [MatTableModule],
  templateUrl: './draftedinspectionslist.component.html',
  styleUrl: './draftedinspectionslist.component.css'

})
export class DraftedinspectionslistComponent implements OnInit{


  @ViewChild('inspectionTable', { static: true }) inspectionTable: MatTable<any>;
    @ViewChild(MatSort) sort !: MatSort;

    clickedRows = new Set<Inspection>();
    displayedColumns: string[] = ['relatedProjectName', 'notificationNumber','relatedOrderName','notificationTypes', 'notificationSubjects','issueDate','persianIssueDate'];
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
    this.inspectionService.getDraftedInspectionsByUserName(currentUser_userName).subscribe((draftedInspections)=>{

    this.inspections = draftedInspections;
      this.dataSource.data = draftedInspections;
      this.inspectionTable.dataSource = this.dataSource;
    }

    );
  }

   clickInspectionRow(cRow: InspectionNotificationDataSummary): void {

      if (Request != null) {
       this.router.navigate(["vendor/editInspection/" + cRow.inspectionNotificationId]);
      }

    }
}
