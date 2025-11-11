import { CommonModule } from '@angular/common';
import { Component, effect, inject, Injector, input, OnInit, output, runInInjectionContext } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-insleft-sidebar',
  imports: [RouterModule, CommonModule],
  templateUrl: './insleft-sidebar.component.html',
  styleUrl: './insleft-sidebar.component.css'
})
export class InsleftSidebarComponent {
injector = inject(Injector);

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
    // {
    //   routeLink: 'projectSettings',
    //   icon: 'fal fa-home',
    //   label: 'Project Settings',
    //   imageName:'../../../../assets/list02.png'
    // },
    // {
    //   routeLink: 'users',
    //   icon: 'fal fa-home',
    //   label: 'User Management',
    //   imageName:'../../../../assets/list02.png'
    // },
    // {
    //   routeLink: 'DefaultSettings',
    //   icon: 'fal fa-home',
    //   label: 'Default Settings',
    //   imageName:'../../../../assets/list02.png'
    // },
    {
      routeLink: 'ResetPassword',
      icon: 'fal fa-box-open',
      label: 'Reset Password',
      imageName:'../../../../assets/list01.png'
    },
    {
      routeLink: 'NotStartedTasks',
      icon: 'fal fa-file',
      label: 'Received Inspections',
      imageName:'../../../../assets/green-add-button-12023.png'
    } ,
    {
      routeLink: 'ConditionalInspectionReports',
      icon: 'fal fa-file',
      label: 'Conditional Inspections',
      imageName:'../../../../assets/green-add-button-12023.png'
    } ,
    {
      routeLink: 'CompletedInspections',
      icon: 'fal fa-file',
      label: 'Completed Inspections',
      imageName:'../../../../assets/green-add-button-12023.png'
    }
  ];

  toggleCollapse(): void {
    this.changeIsLeftSidebarCollapsed.emit(!this.isLeftSidebarCollapsed());
  }

  closeSidenav(): void {
    this.changeIsLeftSidebarCollapsed.emit(true);
  }
}
