import { CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatDividerModule } from '@angular/material/divider';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Order } from '../../../Core/Models/Order.model';
import { Project } from '../../../Core/Models/Project.model';
import { currentstateStore } from '../../../Stores/currentstate.store';
import { Router, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { BasicQCMemberInfo } from '../../../Core/Models/BasicQCMemberInfo.model';
import { QCInfoStore } from '../../../Stores/QCInfo.store';
import { BasicInfoService } from '../../../Core/Services/basic-info.service';
import { UserService } from '../../../Core/Services/user.service';

@Component({
    selector: 'app-ptop-menu',
    imports: [CommonModule, MatToolbarModule, MatDividerModule, FormsModule],
    templateUrl: './ptop-menu.component.html',
    styleUrl: './ptop-menu.component.css'
})
export class PtopMenuComponent {

  vendorData : any;
  public currentProject:Project = {} as Project;
  public order:Order;
  public QCMemberProjects:Project[] =[];

  currentAppState = inject(currentstateStore);
  currentQCState = inject(QCInfoStore);
  @Input() basicQCInfo: BasicQCMemberInfo;


  constructor(private router:Router,private activeRoute: ActivatedRoute,private basicService:BasicInfoService,private userService:UserService) {


  }
  ngOnInit(): void {


    console.log('in ptop menu',this.currentQCState.projects());

    this.QCMemberProjects = this.basicQCInfo.projects;//this.currentVendorState.orders();//this.vendorData['orders'];

  }

  changeProject(event:Event):void{
   console.log('this.currentOrder',this.currentProject);

    this.currentAppState.ChangeProject(this.router.url,this.basicService,this.currentProject);
  }

  CreateNewInspection():void
  {

    console.log('in create New :',this.currentQCState.projects());
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

