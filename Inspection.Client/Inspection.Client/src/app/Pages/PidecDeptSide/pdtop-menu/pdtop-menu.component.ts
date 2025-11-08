import { CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatDividerModule } from '@angular/material/divider';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Project } from '../../../Core/Models/Project.model';
import { Order } from '../../../Core/Models/Order.model';
import { currentstateStore } from '../../../Stores/currentstate.store';
import { PIDECDeptInfoStore } from '../../../Stores/PIDECDeptInfo.store';
import { BasicPIDECDeptInfo } from '../../../Core/Models/BasicPIDECDeptInfo.model';

import { BasicInfoService } from '../../../Core/Services/basic-info.service';
import { UserService } from '../../../Core/Services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-pdtop-menu',
  imports: [CommonModule, MatToolbarModule, MatDividerModule, FormsModule],
  templateUrl: './pdtop-menu.component.html',
  styleUrl: './pdtop-menu.component.css'
})
export class PdtopMenuComponent {
vendorData : any;
  public currentProject:Project = {} as Project;
  public order:Order;
  public QCMemberProjects:Project[] =[];

  currentAppState = inject(currentstateStore);
  currentPIDECDeptState = inject(PIDECDeptInfoStore);
  @Input() basicPIDECDeptInfo: BasicPIDECDeptInfo;


  constructor(private router:Router,private basicService:BasicInfoService,private userService:UserService) {


  }
  ngOnInit(): void {


    console.log('in ptop menu',this.currentPIDECDeptState.projects());

    this.QCMemberProjects = this.basicPIDECDeptInfo.projects;//this.currentVendorState.orders();//this.vendorData['orders'];

  }

  changeProject(event:Event):void{
   console.log('this.currentOrder',this.currentProject);

    this.currentAppState.ChangeProject("pdepartment/",this.basicService,this.currentProject);
  }

  CreateNewInspection():void
  {

    console.log('in create New :',this.currentPIDECDeptState.projects());
    this.router.routeReuseStrategy.shouldReuseRoute = function () {
      return false;
  }
  this.router.onSameUrlNavigation = 'reload';
    this.router.navigate(['vendor/newInspection']);
  }

  Exit():void
  {
        this.userService.logOut();
    this.router.navigate(['logout']);
  }
}
