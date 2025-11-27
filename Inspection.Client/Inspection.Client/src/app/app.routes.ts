import { NotstartedinspectionsComponent } from './Pages/VendorSide/notstartedinspections/notstartedinspections.component';
import { SentinspectionsComponent } from './Pages/VendorSide/sentinspections/sentinspections.component';
import { Routes } from '@angular/router';
import path from 'path';
import { LoginComponent } from './Shared/login/login.component';
import { AuthGuard } from './Core/guards/auth.guard';
import { CheckComponent } from './Pages/check/check.component';
import { NotFoundComponent } from './Pages/not-found/not-found.component';
import { InspectionListComponent } from './Pages/inspection-list/inspection-list.component';
import { MasterComponent } from './Shared/master/master.component';
import { InspectionlistComponent } from './Pages/inspectionlist/inspectionlist.component';
import { VendorMasterComponent } from './Pages/VendorSide/vendor-master/vendor-master.component';
import { PidecMasterComponent } from './Pages/PidecSide/pidec-master/pidec-master.component';
import { vendorbasicinfoResolver } from './Core/Resolvers/vendorbasicinfo.resolver';
import { InspectionService } from './Core/Services/inspection.service';
import { RegisterComponent } from './Shared/register/register.component';
import {MainComponent } from './Shared/main/main.component'
import { inspectionInfoResolver } from './Core/Resolvers/inspection-info.resolver';
import { SettingsComponent } from './Pages/PidecSide/settings/settings.component';
import { InspectionManagementComponent } from './Pages/PidecSide/inspection-management/inspection-management.component';
import { OrderManagementComponent } from './Pages/PidecSide/order-management/order-management.component';
import { qCMembersbasicinfoResolver } from './Core/Resolvers/qcmembersbasicinfo.resolver';
import { DraftedinspectionslistComponent } from './Pages/VendorSide/draftedinspectionslist/draftedinspectionslist.component';
import { InspectionComponent } from './Pages/inspection/inspection.component';
import { ProjectSettingsComponent } from './Pages/PidecSide/project-settings/project-settings.component';
import { ContactPersonListComponent } from './Pages/contact-person-list/contact-person-list.component';
import { ResetPasswordComponent } from './Shared/reset-password/reset-password.component';
import { PidecdeptMasterComponent } from './Pages/PidecDeptSide/pidecdept-master/pidecdept-master.component';
import { pidecdeptbasicinfoResolver } from './Core/Resolvers/pidecdeptbasicinfo.resolver';
import { PdnotStartedtasksComponent } from './Pages/PidecDeptSide/pdnot-startedtasks/pdnot-startedtasks.component';
import { PnewinspectionComponent } from './Pages/PidecSide/pnewinspection/pnewinspection.component';
import { PdnewinspectionComponent } from './Pages/PidecDeptSide/pdnewinspection/pdnewinspection.component';
import { VnewinspectionComponent } from './Pages/VendorSide/vnewinspection/vnewinspection.component';
import { NotStartedtasksComponent } from './Pages/PidecSide/not-startedtasks/not-startedtasks.component';
import { NewinspectionComponent } from './Shared/newinspection/newinspection.component';


export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },

  {
    path:'',
    component: MainComponent,
     children:[
      {
        path:'login',
        component:LoginComponent
      }
      ,{
        path:'signup',
        component:RegisterComponent
      },
      {
        path:'logout',
        component:LoginComponent
      }
     ]
  },
  // {
  //   path:'login',
  //   component:LoginComponent
  // }
  ,
  {
    path:'notfound',
    component: NotFoundComponent
   },
 {
  path:'check',
  component: CheckComponent,
  canActivate:[AuthGuard]
 },
 {
  path:'resetpassword',
  component:ResetPasswordComponent
 },
 {
  path:'vendor',
  component:VendorMasterComponent,
  canActivate:[AuthGuard],
  resolve:{vendorBasicInfo:vendorbasicinfoResolver},
  children:[

    {
      path:'newInspection',
      component:VnewinspectionComponent,
      resolve:{inspectionInfo: inspectionInfoResolver}
    },
    {
      path:'inspectionList',
      component:InspectionlistComponent
    },
    {
      path:'draftedNotifications',
      component: DraftedinspectionslistComponent
    },
     {
      path:'sentinspections',
      component: SentinspectionsComponent
    },
     {
      path:'notstartedinspections',
      component: NotstartedinspectionsComponent
    },
    {
      path:'editInspection/:id',
      component:VnewinspectionComponent,
      data:{reuse:true}
    }
  ]
 },
 {
  path :'pidec',
  component:PidecMasterComponent,
  resolve:{qcBasicInfo:qCMembersbasicinfoResolver},
  children:[
    {
      path:'users',
      component:ContactPersonListComponent
    },
   {
      path:'projectSettings',
      component:  ProjectSettingsComponent

   },
   {
    path:'Orders',
    component:OrderManagementComponent
   },
    {
    path:'inspectionManagement',
    component:InspectionManagementComponent
  },
  {
    path:'projectManagement',
    component:OrderManagementComponent
  },
  {
    path:'NotStartedTasks',
    component:NotStartedtasksComponent
  },
   {
    path:'CompletedInspections',
    component:InspectionManagementComponent
  },
   {
      path:'editInspection/:id',
      component:NewinspectionComponent,
      data:{reuse:true}
    }
]
 },
{
  path :'pidecDept',
  component:PidecdeptMasterComponent,
  resolve:{pidecdeptBasicInfo:pidecdeptbasicinfoResolver},
  children:[
    {
      path:'users',
      component:ContactPersonListComponent
    },
   {
      path:'ResetPassword',
      component:  ResetPasswordComponent

   },
   {
    path:'Orders',
    component:OrderManagementComponent
   },
    {
    path:'inspectionManagement',
    component:InspectionManagementComponent
  },
  {
    path:'projectManagement',
    component:OrderManagementComponent
  },
  {
    path:'NotStartedTasks',
    component:PdnotStartedtasksComponent
  },
   {
    path:'CompletedInspections',
    component:InspectionManagementComponent
  },
   {
      path:'editInspection/:id',
      component:NewinspectionComponent,
      data:{reuse:true}
    },
     {
  path:'newInspection/:id',
  component:PdnewinspectionComponent,
  data:{reuse:true}
 }
]
 }
,
 {
  path :'inspector',
  component:PidecdeptMasterComponent,
  resolve:{pidecdeptBasicInfo:pidecdeptbasicinfoResolver},
  children:[
    {
      path:'users',
      component:ContactPersonListComponent
    },
   {
      path:'ResetPassword',
      component:  ResetPasswordComponent

   },
   {
    path:'Orders',
    component:OrderManagementComponent
   },
    {
    path:'inspectionManagement',
    component:InspectionManagementComponent
  },
  {
    path:'projectManagement',
    component:OrderManagementComponent
  },
  {
    path:'NotStartedTasks',
    component:PdnotStartedtasksComponent
  },
   {
    path:'CompletedInspections',
    component:InspectionManagementComponent
  },
   {
      path:'editInspection/:id',
      component:NewinspectionComponent,
      data:{reuse:true}
    },
     {
  path:'newInspection/:id',
  component:PdnewinspectionComponent,
  data:{reuse:true}
 }
]
 }

//  {
//   path:'login',
//   loadComponent:()=>{
//     return import('./Pages/login/login.component').then(m=>m.LoginComponent);
//   }

 //}

];
