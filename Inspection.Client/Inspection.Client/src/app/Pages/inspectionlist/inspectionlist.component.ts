import { Component, OnInit } from '@angular/core';
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
import { Inspection } from '../../Core/Models/Inspection.model';

interface IRow {
  make: string;
  model: string;
  price: number;
  electric: boolean;
} 

export interface IInspectionData {
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
  setSize("100%","850px")
  //setWidthAndHeight("800px");
}

function setWidthAndHeight(size: string) {
  var eGridDiv = document.querySelector<HTMLElement>("#myGrid")! as any;
  eGridDiv.style.setProperty("width", size);
  eGridDiv.style.setProperty("height", size);
}
function setSize(width: string,height:string) {
  var eGridDiv = document.querySelector<HTMLElement>("#myGrid")! as any;
  eGridDiv.style.setProperty("width", width);
  eGridDiv.style.setProperty("height", height);
}
let gridApi: GridApi<Inspection> ;
const gridOptions: GridOptions<Inspection> = {
  columnDefs: [
    { field: "name", width: 150 },
    // { field: "city", width: 90 },
    // { field: "notificationNumber", width: 150 },
    { field: "inspectionSubject", width: 90 },
    // { field: "ActualShippingDueDate", width: 150 },
    // { field: "contractShippingDueDate", width: 150 },
    // { field: "locationId", width: 100 },
    // { field: "ownerId", width: 100 }    
  ],
  domLayout:'normal'
};

@Component({
  selector: 'app-inspectionlist',
  standalone: true,
  imports: [],
  templateUrl: './inspectionlist.component.html',
  styleUrl: './inspectionlist.component.css'
})
export class InspectionlistComponent implements OnInit{

  constructor(private inspectionService : InspectionService) {
  }

  ngOnInit(): void {

    let gridDiv = document.querySelector<HTMLElement>("#myGrid")!;
    gridApi = createGrid(gridDiv, gridOptions);
  
    this.inspectionService.getInspectionList().subscribe((result)=>
    {
     console.log(result);
       gridApi!.setGridOption("rowData", result);
       fillExact();
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

    
}
