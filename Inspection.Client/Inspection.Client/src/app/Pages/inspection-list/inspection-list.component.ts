import { AfterRenderPhase, Component, OnInit } from '@angular/core';
import { AgGridAngular } from "ag-grid-angular";
import {    ColDef,ColGroupDef,  FirstDataRenderedEvent,  GridApi,
  GridOptions,
  PaginationNumberFormatterParams,
  createGrid,} from "ag-grid-community";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";

import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";
import { InspectionService } from '../../Core/Services/inspection.service';


interface IRow {
  make: string;
  model: string;
  price: number;
  electric: boolean;
} 

export interface IOlympicData {
  athlete: string,
  age: number,
  country: string,
  year: number,
  date: string,
  sport: string,
  gold: number,
  silver: number,
  bronze: number,
  total: number
}


function fillLarge() {
  setWidthAndHeight("100%");
}

function fillMedium() {
  setWidthAndHeight("60%");
}

function fillExact() {
  setWidthAndHeight("400px");
}

function setWidthAndHeight(size: string) {
  var eGridDiv = document.querySelector<HTMLElement>("#myGrid")! as any;
  eGridDiv.style.setProperty("width", size);
  eGridDiv.style.setProperty("height", size);
}

let gridApi: GridApi<IOlympicData> ;
const gridOptions: GridOptions<IOlympicData> = {
  columnDefs: [
    { field: "athlete", width: 150 },
    { field: "age", width: 90 },
    { field: "country", width: 150 },
    { field: "year", width: 90 },
    { field: "date", width: 150 },
    { field: "sport", width: 150 },
    { field: "gold", width: 100 },
    { field: "silver", width: 100 },
    { field: "bronze", width: 100 },
    { field: "total", width: 100 },
  ],
  domLayout:'normal'
};
@Component({
    selector: 'app-inspection-list',   
    templateUrl: './inspection-list.component.html',
    styleUrl: './inspection-list.component.css'
})
export class InspectionListComponent implements OnInit{
  
  
  /**
   *
   */
  constructor(private inspectionService : InspectionService) {
    
    
  }
  
  ngOnInit(): void {

  let gridDiv = document.querySelector<HTMLElement>("#myGrid")!;
  gridApi = createGrid(gridDiv, gridOptions);

  
    fetch("https://www.ag-grid.com/example-assets/olympic-winners.json")
    .then((response) => response.json())
    .then((data: IOlympicData[]) => gridApi!.setGridOption("rowData", data));
  
  if (typeof window !== "undefined") {
    // Attach external event handlers to window so they can be called from index.html
    (<any>window).fillLarge = fillLarge;
    (<any>window).fillMedium = fillMedium;
    (<any>window).fillExact = fillExact;
  }
  }

  // rowData: IRow[] = [
  //   { make: "Tesla", model: "Model Y", price: 64950, electric: true },
  //   { make: "Ford", model: "F-Series", price: 33850, electric: false },
  //   { make: "Toyota", model: "Corolla", price: 29600, electric: false },
  //   { make: "Mercedes", model: "EQA", price: 48890, electric: true },
  //   { make: "Fiat", model: "500", price: 15774, electric: false },
  //   { make: "Nissan", model: "Juke", price: 20675, electric: false },
  // ];
 
  

  
  // defaultColDef: ColDef = {
  //   flex: 1,
  // };
  // colDefs = colDefs;
  // constructor() {
    
    
  // }
  // ngOnInit(): void {
  //   fetch("https://www.ag-grid.com/example-assets/olympic-winners.json")
  //   .then((response) => response.json())
  //   .then(function (data) {
  //     gridApi!.setGridOption("rowData", data);
  //   });
  // }
  
  









}
