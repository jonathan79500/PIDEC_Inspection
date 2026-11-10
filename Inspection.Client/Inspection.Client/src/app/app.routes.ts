import { Inspection } from './Core/Models/Inspection.model';
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
import { VnewinspectionComponent } from './Pages/VendorSide/vnewinspection/vnewinspection.component';
import { NotStartedtasksComponent } from './Pages/PidecSide/not-startedtasks/not-startedtasks.component';
import { InspectorMasterComponent } from './Pages/InspectorSide/inspector-master/inspector-master.component';
import { inspectorbasicinfoResolver } from './Core/Resolvers/inspectorbasicinfo.resolver';
import { NotstartedtasksComponent } from './Pages/InspectorSide/notstartedtasks/notstartedtasks.component';
import { InspectionPrintComponent } from './Pages/VendorSide/inspection-print/inspection-print.component';
import { ReadyInspectionsForReportComponent } from './Pages/InspectorSide/ready-inspections-for-report/ready-inspections-for-report.component';
import { CompletedTasksComponent } from './Pages/PidecSide/completed-tasks/completed-tasks.component';
import { InspectorManagementComponent } from './Shared/inspector-management/inspector-management.component';
import { DocumentTypeManagementComponent } from './Shared/document-type-management/document-type-management.component';
import { InspectionHistoryComponent } from './Pages/VendorSide/inspection-history/inspection-history.component';


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
      path:'inspection-print',
      component:InspectionPrintComponent,

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
      resolve:{inspectionInfo: inspectionInfoResolver},
      runGuardsAndResolvers: 'always'
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
      path:'editInspection/:RequestId/:InspectionId',
      component:VnewinspectionComponent,
      data:{reuse:true}
    }
  ]
 },
 ,
{
  path: 'inspection-print',
  component: InspectionPrintComponent,
  canActivate: [AuthGuard]
},
    {
      path:'inspection-history/:requestId',
      component:InspectionHistoryComponent,

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
    path:'InspectorManager',
    component:InspectorManagementComponent
  },
  {
    path:'DocumentTypeManager',
    component:DocumentTypeManagementComponent
  },
   {
    path:'CompletedInspections',
   component:CompletedTasksComponent
  },
   {
      path:'editInspection/:RequestId/:InspectionId',
      component:VnewinspectionComponent,
      data:{reuse:true}
    }
    ,
    {
      path:'inspection-print',
      component:InspectionPrintComponent,

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
      path:'editInspection/:RequestId/:InspectionId',
      component:VnewinspectionComponent,
      data:{reuse:true}
    },
    {
      path:'inspection-print',
      component:InspectionPrintComponent,

    },
     {
  path:'newInspection/:id',
  component:VnewinspectionComponent,
  data:{reuse:true}
 }
]
 }
,
 {
  path :'inspector',
  component:InspectorMasterComponent,
  resolve:{inspectorBasicInfo:inspectorbasicinfoResolver},
  children:[
   {
      path:'ResetPassword',
      component:  ResetPasswordComponent

   },
  {
    path:'NotStartedTasks',
    component:NotstartedtasksComponent
  },
   {
    path:'InspectorManager',
    component:InspectorManagementComponent
  },
   {
    path:'CompletedInspections',
    component:InspectionManagementComponent
  },
   {
      path:'editInspection/:RequestId/:InspectionId',
      component:VnewinspectionComponent,
      data:{reuse:true}
    },

    {
      path:'inspection-print',
      component:InspectionPrintComponent,

    },
    {
      path:'ReadyInspectionsForReport',
      component:ReadyInspectionsForReportComponent,
      data:{reuse:true}
    }

]
 }

];
