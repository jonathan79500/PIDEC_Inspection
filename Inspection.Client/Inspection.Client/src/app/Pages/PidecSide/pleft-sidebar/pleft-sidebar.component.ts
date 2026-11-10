import { CommonModule } from '@angular/common';
import { Component, effect, inject, Injector, input, output, runInInjectionContext } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
    selector: 'app-pleft-sidebar',
    imports: [RouterModule, CommonModule],
    templateUrl: './pleft-sidebar.component.html',
    styleUrl: './pleft-sidebar.component.css'
})
export class PleftSidebarComponent {

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
      routeLink: 'projectSettings',
      icon: 'fal fa-home',
      label: 'Project Settings',
      imageName:'assets/list02.png'
    },
    {
      routeLink: 'users',
      icon: 'fal fa-home',
      label: 'User Management',
      imageName:'assets/list02.png'
    },
    {
      routeLink: 'DefaultSettings',
      icon: 'fal fa-home',
      label: 'Default Settings',
      imageName:'assets/list02.png'
    },
    {
      routeLink: 'Orders',
      icon: 'fal fa-box-open',
      label: 'Orders',
      imageName:'assets/list01.png'
    },
    {
      routeLink: 'InspectorManager',
      icon: 'fal fa-file',
      label: 'Manage Inspectors',
      imageName:'assets/green-add-button-12023.png'
    },
    {
      routeLink: 'DocumentTypeManager',
      icon: 'fal fa-file',
      label: 'Manage Document Types',
      imageName:'assets/green-add-button-12023.png'
    },
    {
      routeLink: 'NotStartedTasks',
      icon: 'fal fa-file',
      label: 'Received Inspections',
      imageName:'assets/green-add-button-12023.png'
    } ,
    {
      routeLink: 'CompletedInspections',
      icon: 'fal fa-file',
      label: 'Completed Inspections',
      imageName:'assets/green-add-button-12023.png'
    }
  ];

  toggleCollapse(): void {
    this.changeIsLeftSidebarCollapsed.emit(!this.isLeftSidebarCollapsed());
  }

  closeSidenav(): void {
    this.changeIsLeftSidebarCollapsed.emit(true);
  }


}
