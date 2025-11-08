import { Component, HostListener, signal } from '@angular/core';
import { InsleftSidebarComponent } from "../insleft-sidebar/insleft-sidebar.component";
import { InstopMenuComponent } from "../instop-menu/instop-menu.component";
import { InsMainComponent } from "../ins-main/ins-main.component";
import { Order } from '../../../Core/Models/Order.model';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { BasicInfoService } from '../../../Core/Services/basic-info.service';
import { UserService } from '../../../Core/Services/user.service';
import { BasicInspectorInfo } from '../../../Core/Models/BasicInspectorInfo.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-inspector-master',
  imports: [InsleftSidebarComponent, InstopMenuComponent, InsMainComponent, RouterModule, InsMainComponent, CommonModule],
  templateUrl: './inspector-master.component.html',
  styleUrl: './inspector-master.component.css'
})
export class InspectorMasterComponent {
public CurrentProjectName :string = "Almas Project";
  public currentOrders : Order[] =[] ;
  public basicInspectorInfo :any;
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



    this.route.data.subscribe((data: { inspectorBasicInfo: BasicInspectorInfo }) => {
      console.log('in onInit PIDEC-Master:',data.inspectorBasicInfo);
      this.basicInspectorInfo = data.inspectorBasicInfo;
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
