import { Component, HostListener, signal } from '@angular/core';
import { PdleftSidebarComponent } from '../pdleft-sidebar/pdleft-sidebar.component';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { PdMainComponent } from '../pd-main/pd-main.component';
import { PdtopMenuComponent } from '../pdtop-menu/pdtop-menu.component';
import { CommonModule } from '@angular/common';
import { BasicPIDECDeptInfo } from '../../../Core/Models/BasicPIDECDeptInfo.model';
import { BasicInfoService } from '../../../Core/Services/basic-info.service';
import { UserService } from '../../../Core/Services/user.service';
import { Order } from '../../../Core/Models/Order.model';

@Component({
  selector: 'app-pidecdept-master',
  imports: [PdleftSidebarComponent, RouterModule, PdMainComponent, PdtopMenuComponent, CommonModule],
  templateUrl: './pidecdept-master.component.html',
  styleUrl: './pidecdept-master.component.css'
})
export class PidecdeptMasterComponent {
 public CurrentProjectName :string = "Almas Project";
  public currentOrders : Order[] =[] ;
  public basicPidecDeptInfo :any;
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



    this.route.data.subscribe((data: { pidecdeptBasicInfo: BasicPIDECDeptInfo }) => {
      console.log('in onInit PIDEC-Master:',data.pidecdeptBasicInfo);
      this.basicPidecDeptInfo = data.pidecdeptBasicInfo;
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
