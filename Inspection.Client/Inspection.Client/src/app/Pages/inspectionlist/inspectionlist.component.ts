import { AfterViewInit, Component, ElementRef, OnInit, ViewChild, viewChild } from '@angular/core';
// import { AgGridAngular } from "ag-grid-angular";
//import { AgGridAngular } from '@ag-grid-community/angular';

import {    ColDef,ColGroupDef,  FirstDataRenderedEvent,  GridApi,
  GridOptions,
  PaginationNumberFormatterParams,
  createGrid,} from "ag-grid-community";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";

import { InspectionService } from '../../Core/Services/inspection.service';
import { Inspection } from '../../Core/Models/Inspection.model';

import { MatTable, MatTableDataSource, MatTableModule} from '@angular/material/table';
import { MatSort } from '@angular/material/sort';


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
    { field: "requestInspectionTimesInPeriod", width: 150 },
    { field: "requestInspectionPeriodDays", width: 150 },
    { field: "order.equipmentTagNo", width: 90 },
    { field: "contractShippingDueDate", width: 150 },
    { field: "actualShippingDueDate", width: 150 },
    { field: "location.country", width: 100 },
    { field: "location.city", width: 100 },
    // { field: "ownerId", width: 100 }    
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
    selector: 'app-inspectionlist',
    imports: [ MatTableModule],
    templateUrl: './inspectionlist.component.html',
    styleUrl: './inspectionlist.component.css'
})
export class InspectionlistComponent implements OnInit,AfterViewInit {

  @ViewChild('inspectionTable', { static: true }) inspectionTable: MatTable<any>;
  @ViewChild(MatSort) sort !: MatSort;

  clickedRows = new Set<Inspection>();
  displayedColumns: string[] = ['name', 'notificationNumber','inspectionType.name', 'inspectionSubject','order.name'];
  dataSource: MatTableDataSource<Inspection> = new MatTableDataSource<Inspection>();

  public gridOptions: GridOptions = {
    domLayout: 'autoHeight'
      };
   rowData0: any[] | null = [
    { make: "Toyota", model: "Celica", price: 35000 },
    { make: "Ford", model: "Mondeo", price: 32000 },
    { make: "Porsche", model: "Boxster", price: 72000 },
    { make: "BMW", model: "M50", price: 60000 },
    { make: "Aston Martin", model: "DBX", price: 190000 },
  ];
   columnDefs0: ColDef[] = [
    { field: "make" ,width:200 },
    { field: "model" ,width:200},
    { field: "price" ,width:200},
  ];
   themeClass0: string =
    "ag-theme-quartz";

 // @ViewChild('agInspections',{static:true}) agInspections :ElementRef;
public mainGrid:GridApi<Inspection>;

  public themeClass: string = "ag-theme-quartz";
  public inspections:Inspection[];
  public reorgData :InspectionGridData[]=[];
public agInspection :GridApi;

  @ViewChild('myGrid',{static:true})  gridDiv: ElementRef;


  columnDefs: [
    { field: "name", width: 150 },
   // {field:"order.name",width:200},
    { field: "notificationNumber", width: 90 },
  //   { field: "inspectionType.name", width: 150 },
    { field: "inspectionSubject", width: 90 },
    // { field: "contractShippingDueDate", width: 150 },
    // { field: "actualShippingDueDate", width: 150 },
  //  { field: "location.country", width: 100 },
   // { field: "location.city", width: 100 },
    // { field: "ownerId", width: 100 }    
  ]

  constructor(private inspectionService : InspectionService) {
    //console.log('inspectionList Constructor');
  }
  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
  }

  ngOnInit(): void {

    
    //let gridDiv = document.querySelector<HTMLElement>("#myGrid")!;
    // console.log('inspectionList oninit',document,document.querySelector,this.gridDiv);
    // console.log('sadjhjd');
    // gridApi = createGrid(this.gridDiv.nativeElement, gridOptions);
  
    this.inspectionService.getInspectionList().subscribe((result)=>
    {
      this.inspections = result;
     // console.log(result);
      let data: InspectionGridData ={
        inspectionSubject:'',
        name:'',
        notificationNumber:''
      };
      // result.forEach(ins => {
      //     // data.actualShippingDueDate = inspection.actualShippingDueDate.toString();
      //     // data.contractShippingDueDate = inspection.contractShippingDueDate.toString();
      //     data.inspectionSubject = ins.inspectionSubject;
      //     data.name = ins.name!;
      //     data.notificationNumber = ins.notificationNumber;
      //     this.reorgData.push(data);
      // });
      this.dataSource.data = result;
      this.inspectionTable.dataSource = this.dataSource;
      
     // this.agInspection.setGridOption("rowData",this.reorgData);
    //  console.log(result);
    //    gridApi!.setGridOption("rowData", result);
      // fillExact(this.agInspection);
    }
    )
      // fetch("https://www.ag-grid.com/example-assets/olympic-winners.json")
      // .then((response) => response.json())
      // .then((data: IOlympicData[]) => gridApi!.setGridOption("rowData", data));
    
    if (typeof window !== "undefined") {
      // Attach external event handlers to window so they can be called from index.html
      (<any>window).fillLarge = fillLarge;
      (<any>window).fillMedium = fillMedium;
      (<any>window).fillExact = fillExact;
    }
    }
    clickInspectionRow(cRow: Inspection): void {

      // console.log('in selected Row',cRow);
      // this.vendorSharedData.setInspection(cRow);

      this.clickedRows.add(cRow);
  
      if (Request != null) {
  
        let w = window.open("newInspection/" + cRow.notificationNumber, "", "width:100px;height:200px;");
        //let w = window.open("newInspection/" + cRow.id, "", "width:100px;height:200px;");
       // w.opener.data = cRow;
      }
  
    }
    
}
