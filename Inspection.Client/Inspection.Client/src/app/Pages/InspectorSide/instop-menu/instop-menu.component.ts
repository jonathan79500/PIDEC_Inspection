import { CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatDividerModule } from '@angular/material/divider';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Project } from '../../../Core/Models/Project.model';
import { Order } from '../../../Core/Models/Order.model';
import { BasicInspectorInfo } from '../../../Core/Models/BasicInspectorInfo.model';
import { currentstateStore } from '../../../Stores/currentstate.store';
import { InspectorInfoStore } from '../../../Stores/InspectorInfo.store';
import { BasicInfoService } from '../../../Core/Services/basic-info.service';
import { UserService } from '../../../Core/Services/user.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-instop-menu',
  imports: [CommonModule, MatToolbarModule, MatDividerModule, FormsModule],
  templateUrl: './instop-menu.component.html',
  styleUrl: './instop-menu.component.css'
})
export class InstopMenuComponent {
vendorData : any;
  public currentProject:Project = {} as Project;
  public order:Order;
  public InspectorProjects:Project[] =[];

  currentAppState = inject(currentstateStore);
  currentInspectorState = inject(InspectorInfoStore);
  @Input() basicInspectorInfo: BasicInspectorInfo;


  constructor(private router:Router,private basicService:BasicInfoService,private userService:UserService) {


  }
  ngOnInit(): void {


    console.log('in ptop menu',this.currentInspectorState.projects());

    this.InspectorProjects = this.basicInspectorInfo.projects;//this.currentVendorState.orders();//this.vendorData['orders'];

  }

  changeProject(event:Event):void{
   console.log('this.currentOrder',this.currentProject);

    this.currentAppState.ChangeProject("pdepartment/",this.basicService,this.currentProject);
  }

  CreateNewInspection():void
  {

    console.log('in create New :',this.currentInspectorState.projects());
    this.router.routeReuseStrategy.shouldReuseRoute = function () {
      return false;
  }
  this.router.onSameUrlNavigation = 'reload';
    this.router.navigate(['vendor/newInspection']);
  }

  ShowWarnings():void
  {

  }

  Exit():void
  {
        this.userService.logOut();
    this.router.navigate(['logout']);
  }
}
