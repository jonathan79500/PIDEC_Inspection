import { CommonModule, NgTemplateOutlet } from '@angular/common';
import { Component,computed,effect,input, Input, OnInit, Signal, signal, TemplateRef } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LoginComponent } from '../../Pages/login/login.component';
import { BrowserModule } from '@angular/platform-browser';
import { sign } from 'node:crypto';
import { CheckComponent } from '../../Pages/check/check.component';
import { NotFoundComponent } from '../../Pages/not-found/not-found.component';
import { AgGridAngular } from 'ag-grid-angular';
import { RouterOutlet,RouterLink ,RouterLinkActive, RouterModule} from '@angular/router';
import { Router } from '@angular/router';
import { BasicInfoService } from '../../Core/Services/basic-info.service';
import { Project } from '../../Core/Models/Project.model';
import { Order } from '../../Core/Models/Order.model';

@Component({
  selector: 'app-vendor-master',
  standalone: true,
  imports: [RouterOutlet,RouterModule],
  templateUrl: './vendor-master.component.html',
  styleUrl: './vendor-master.component.css'
})
export class VendorMasterComponent implements OnInit{

  public CurrentProjectName :string = "Almas Project";
  public currentOrders : Order[] =[] ;

  /**
   *
   */
  constructor(private router: Router,private basicInfoService: BasicInfoService) {
     
  }

  ngOnInit(): void {

    console.log("in init orders");

    this.initOrders();

    console.log("After init orders",this.currentOrders);
   
  }

  initOrders(): void
  {

    let order1 : Order = {
      id: '1',
      projectId: '1',
      no: '242424',
      title: 'Order Title 1',
      name: 'Order Name 1',
      createdDate: undefined,
      modifiedDate: undefined,
      createdBy: undefined,
      modifiedBy: undefined
    }

    let order2 : Order = {
      id: '2',
      projectId: '2',
      no: '3453535',
      title: 'Order Title 2',
      name: 'Order Name 2',
      createdDate: undefined,
      modifiedDate: undefined,
      createdBy: undefined,
      modifiedBy: undefined
    }

    this.currentOrders.push(order1);
    this.currentOrders.push(order2);
  }

  

  goToInspectionList(){
    this.router.navigate(['/inspectionList']);
  }
  
  gotoRejectedInspections()
  {
    this.router.navigate(['/inspectionList']);
  }
  
  CreateNewInspectionNotification() {
  this.router.navigate(['/newinspection'])
    }

}
