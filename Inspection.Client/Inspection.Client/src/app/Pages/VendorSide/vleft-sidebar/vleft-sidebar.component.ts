import { CommonModule } from '@angular/common';
import { Component, effect, inject, Injector, input, OnInit, output, runInInjectionContext } from '@angular/core';
import { RouterModule } from '@angular/router';


@Component({
    selector: 'app-vleft-sidebar',
    imports: [RouterModule, CommonModule],
    templateUrl: './vleft-sidebar.component.html',
    styleUrls: ['./vleft-sidebar.component.css']
})
export class VleftSidebarComponent implements OnInit {


  injector = inject(Injector);

  /**
   *
   */
  constructor() {


  }
  ngOnInit(): void {
   runInInjectionContext(this.injector,()=>{
effect(()=>{
    //console.log(this.vendorSharedData.getSelectedRequisition());
})

   })
  }
  isLeftSidebarCollapsed = input.required<boolean>();
  changeIsLeftSidebarCollapsed = output<boolean>();
  items = [
    {
      routeLink: 'sentinspections',
      icon: 'fal fa-home',
      label: 'Sent Inspection Notifications',
      imageName:'../../../../assets/list02.png'
    },
    {
      routeLink: 'notstartedinspections',
      icon: 'fal fa-box-open',
      label: 'NotStarted Inspection Notifications',
      imageName:'../../../../assets/list01.png'
    },
    {
      routeLink: 'draftedNotifications',
      icon: 'fal fa-box-open',
      label: 'Drafted Inspection Notifications',
      imageName:'../../../../assets/list01.png'
    },
    {
      routeLink: 'pages',
      icon: 'fal fa-file',
      label: 'Pages',
      imageName:'../../../../assets/green-add-button-12023.png'
    },
    {
      routeLink: 'newInspection',
      icon: 'fal fa-cog',
      label: 'Create New Inspection',
      imageName:'../../../../assets/orange-plus-11974.svg'
    },
  ];

  toggleCollapse(): void {
    this.changeIsLeftSidebarCollapsed.emit(!this.isLeftSidebarCollapsed());
  }

  closeSidenav(): void {
    this.changeIsLeftSidebarCollapsed.emit(true);
  }


}
