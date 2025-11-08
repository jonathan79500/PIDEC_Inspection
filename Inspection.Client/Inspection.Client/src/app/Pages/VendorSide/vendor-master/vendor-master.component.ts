import { CommonModule, NgTemplateOutlet } from '@angular/common';
import { Component,computed,effect,HostListener,input, Input, OnDestroy, OnInit, Signal, signal, TemplateRef, ViewEncapsulation } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { sign } from 'node:crypto';
import { CheckComponent } from '../../check/check.component';
import { NotFoundComponent } from '../../not-found/not-found.component';
import { AgGridAngular } from 'ag-grid-angular';
import { RouterOutlet,RouterLink ,RouterLinkActive, RouterModule, ActivatedRoute} from '@angular/router';
import { Router } from '@angular/router';
import { BasicInfoService } from '../../../Core/Services/basic-info.service';
import { Project } from '../../../Core/Models/Project.model';
import { Order } from '../../../Core/Models/Order.model';
import { UserService } from '../../../Core/Services/user.service';
import { VleftSidebarComponent } from "../vleft-sidebar/vleft-sidebar.component";
import { VmainComponent } from '../vmain/vmain.component';
import { VtopMenuComponent } from "../vtop-menu/vtop-menu.component";
import { BasicVendorInfo } from '../../../Core/Models/BasicVendorInfo.model';


@Component({
    selector: 'app-vendor-master',
    imports: [RouterModule, VleftSidebarComponent, VmainComponent, VtopMenuComponent, CommonModule],
    templateUrl: './vendor-master.component.html',
    styles: [],
    styleUrl: './vendor-master.component.css',
    encapsulation: ViewEncapsulation.None
})
export class VendorMasterComponent implements OnInit,OnDestroy{

  public CurrentProjectName :string = "Almas Project";
  public currentOrders : Order[] =[] ;
  public basicVendorInfo :any;
  isLoading = true;

  isLeftSidebarCollapsed = signal<boolean>(false);
  screenWidth = signal<number>(window.innerWidth);

  @HostListener('window:resize')
  onResize() {
    this.screenWidth.set(window.innerWidth);
    if (this.screenWidth() < 768) {
      this.isLeftSidebarCollapsed.set(true);
    }
  }

  

   
  ngOnDestroy() {
    console.log('VendorMasterComponent ngOnDestroy');
  }
  
  /**
   *
   */
  constructor(private route: ActivatedRoute,private router: Router,private basicInfoService: BasicInfoService,private vendorService :UserService) {
    console.log('VendorMasterComponent constructor');
  }

  ngOnInit(): void {
    console.log('VendorMasterComponent ngOnInit');
  
    this.isLeftSidebarCollapsed.set(this.screenWidth() < 768);

    
    
    this.route.data.subscribe((data: { vendorBasicInfo: BasicVendorInfo }) => {
      console.log('in onInit vendor-Master:',data.vendorBasicInfo);
      this.basicVendorInfo = data.vendorBasicInfo;
      this.isLoading = false;    
    });
  }

  changeIsLeftSidebarCollapsed(isLeftSidebarCollapsed1: boolean): void {
      
    this.isLeftSidebarCollapsed.set(isLeftSidebarCollapsed1);
  }

  

  goToInspectionList(){
    this.router.navigate(['/inspectionList']);
  }
  
  gotoRejectedInspections()
  {
    this.router.navigate(['/inspectionList']);
  }
  
  CreateNewInspectionNotification() {
  this.router.navigate(['/vendor/newInspection'])
    }

}
