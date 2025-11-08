import { Component, inject, Inject, Input, OnInit } from '@angular/core';

import {MatToolbarModule} from '@angular/material/toolbar';
import {MatDividerModule} from '@angular/material/divider';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../Core/Services/user.service';
import { Order } from '../../../Core/Models/Order.model';
import { Project } from '../../../Core/Models/Project.model';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { currentstateStore } from '../../../Stores/currentstate.store';
import { vendorInfoStore } from '../../../Stores/vendorInfo.store';
import { BasicVendorInfo } from '../../../Core/Models/BasicVendorInfo.model';



@Component({
    selector: 'app-vtop-menu',
    imports: [CommonModule, MatToolbarModule, MatDividerModule, FormsModule],
    templateUrl: './vtop-menu.component.html',
    styleUrl: './vtop-menu.component.css'
})
export class VtopMenuComponent implements OnInit{

  vendorData : any;
  public currentOrder:Order = {} as Order;
  public order:Order;
  public vendorOrders:Order[] =[];
  public vendorProject :Project;
  currentAppState = inject(currentstateStore);
  currentVendorState = inject(vendorInfoStore);
  @Input() basicVendorInfo: BasicVendorInfo;


  constructor(private router:Router,private userService:UserService) {


  }
  ngOnInit(): void {

    console.log('in vtop ,',this.basicVendorInfo);
   // this.vendorData = this.currentVendorData.//this.currentAppState.vendorData();
    console.log('in vtop menu',this.currentVendorState.orders());


    this.vendorOrders = this.basicVendorInfo.orders;//this.currentVendorState.orders();//this.vendorData['orders'];

    console.log('in vtop menu orders',this.vendorOrders);

    this.vendorProject = this.basicVendorInfo.projects[0];

    console.log('in vtop menu project',this.vendorProject);
  }

  changeRequisition(event:Event):void{
   console.log('this.currentOrder',this.currentOrder);

    this.currentAppState.ChangeProject_Order(this.vendorProject,this.currentOrder);
  }

  CreateNewInspection():void
  {

    console.log('in create New :',this.currentVendorState.orders());
    this.router.routeReuseStrategy.shouldReuseRoute = function () {
      return false;
  }
  if(this.currentAppState.currentorder().name !='')
  {
    this.router.onSameUrlNavigation = 'reload';
    this.router.navigate(['vendor/newInspection']);
  }
  }

  Exit():void
  {
    this.userService.logOut();
    this.router.navigate(['logout']);
  }

}
