import { InspectionNotificationDataSummary } from './../../../Core/Models/InspectionNotificationDataSummary.model';
import { Component, ElementRef, inject, OnInit, runInInjectionContext, ViewChild, Injector, effect } from '@angular/core';
import { InspectionService } from '../../../Core/Services/inspection.service';
import { Inspection } from '../../../Core/Models/Inspection.model';
import { ColDef, GridApi, GridOptions } from 'ag-grid-community';
import { MatSort } from '@angular/material/sort';
import { MatTable, MatTableDataSource, MatTableModule } from '@angular/material/table';
import { currentstateStore } from '../../../Stores/currentstate.store';
import { UserTypes } from '../../../Core/Models/UserTypes.enum';
import { Project } from '../../../Core/Models/Project.model';
import { User } from '../../../Core/Models/User.model';
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

type InspectionGridData =
{
  name :string,
  //orderName:string,
  notificationNumber :string,
 // inspectionType :string,
  inspectionSubject :string,
  // contractShippingDueDate:string,
  // actualShippingDueDate:string,
 // country:string,
  //city:string
}


@Component({
    selector: 'app-inspection-management',
    imports: [MatTableModule],
    templateUrl: './inspection-management.component.html',
    styleUrl: './inspection-management.component.css'
})
export class InspectionManagementComponent implements OnInit{

 @ViewChild('inspectionTable', { static: true }) inspectionTable: MatTable<any>;
  @ViewChild(MatSort) sort !: MatSort;

  clickedRows = new Set<Inspection>();
   displayedColumns: string[] = ['no','relatedProjectName', 'notificationNumber','relatedOrderName','notificationTypes', 'notificationSubjects','issueDate','persianIssueDate'];
  dataSource: MatTableDataSource<Inspection> = new MatTableDataSource<Inspection>();

  public gridOptions: GridOptions = {
    domLayout: 'autoHeight'
      };

   themeClass0: string =
    "ag-theme-quartz";

 // @ViewChild('agInspections',{static:true}) agInspections :ElementRef;
public mainGrid:GridApi<Inspection>;

  public themeClass: string = "ag-theme-quartz";
  public inspections:Inspection[];
  public reorgData :InspectionGridData[]=[];
public agInspection :GridApi;
currentAppState = inject(currentstateStore);
currentProject : Project;
currentUser :User;
 injector = inject(Injector);

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

  constructor(private inspectionService : InspectionService, private router: Router) {

  }
  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
  }

  ngOnInit(): void {

    let currentProjectId = '';
    this.currentUser =  this.currentAppState.currentuser();
     if(this.currentUser.userType == UserTypes.PIDEC_TO || this.currentUser.userType == UserTypes.PIDEC_CC)
    {
       this.currentProject = this.currentAppState.currentproject();
       currentProjectId = this.currentProject.id;
    }


    this.inspectionService.getInspectionListByProjectIdUserId(currentProjectId,this.currentUser.id).subscribe((result)=>
    {
      this.inspections = result;
      let data: InspectionGridData ={
        inspectionSubject:'',
        name:'',
        notificationNumber:''
      };

      this.dataSource.data = result;
      this.inspectionTable.dataSource = this.dataSource;
    }
    )

    if (typeof window !== "undefined") {
      // Attach external event handlers to window so they can be called from index.html
      (<any>window).fillLarge = fillLarge;
      (<any>window).fillMedium = fillMedium;
      (<any>window).fillExact = fillExact;
    }


 runInInjectionContext(this.injector,()=>{
      effect(()=>{
      this.currentProject = this.currentAppState.currentproject();
       currentProjectId = this.currentProject.id;
    this.inspectionService.getInspectionListByProjectIdUserId(currentProjectId,this.currentUser.id).subscribe((result)=>
    {
      this.inspections = result;
      let data: InspectionGridData ={
        inspectionSubject:'',
        name:'',
        notificationNumber:''
      };
      this.dataSource.data = result;
      this.inspectionTable.dataSource = this.dataSource;
    })

      });
 });

 }

     clickInspectionRow(cRow: InspectionNotificationDataSummary): void {
          if (Request != null) {
           this.router.navigate(["pidec/editInspection/" + cRow.inspectionNotificationId]);
          }

        }
}
