import { Component, HostListener, signal, ViewEncapsulation } from '@angular/core';
import { PleftSidebarComponent } from "../pleft-sidebar/pleft-sidebar.component";
import { ActivatedRoute, RouterModule, RouterOutlet } from '@angular/router';
import { PtopMenuComponent } from '../ptop-menu/ptop-menu.component';
import { CommonModule } from '@angular/common';
import { PmainComponent } from '../pmain/pmain.component';
import { Order } from '../../../Core/Models/Order.model';
import { Router } from '@angular/router';
import { BasicInfoService } from '../../../Core/Services/basic-info.service';
import { UserService } from '../../../Core/Services/user.service';
import { BasicQCMemberInfo } from '../../../Core/Models/BasicQCMemberInfo.model';

@Component({
    selector: 'app-pidec-master',
    imports: [PleftSidebarComponent, RouterModule, PmainComponent, PtopMenuComponent, CommonModule],
    templateUrl: './pidec-master.component.html',
    styleUrl: './pidec-master.component.css',
    encapsulation: ViewEncapsulation.None
})
export class PidecMasterComponent {

  public CurrentProjectName :string = "Almas Project";
  public currentOrders : Order[] =[] ;
  public basicQCInfo :any;
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


  /**
   *
   */
  constructor(private route: ActivatedRoute,private router: Router,private basicInfoService: BasicInfoService,private vendorService :UserService) {
     
  }

  ngOnInit(): void {

    this.isLeftSidebarCollapsed.set(this.screenWidth() < 768);

    
    
    this.route.data.subscribe((data: { qcBasicInfo: BasicQCMemberInfo }) => {
      console.log('in onInit PIDEC-Master:',data.qcBasicInfo);
      this.basicQCInfo = data.qcBasicInfo;
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
