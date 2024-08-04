import { Routes } from '@angular/router';
import path from 'path';
import { LoginComponent } from './Pages/login/login.component';
import { authGuard } from './Core/guards/auth.guard';
import { CheckComponent } from './Pages/check/check.component';
import { NotFoundComponent } from './Pages/not-found/not-found.component';
import { InspectionListComponent } from './Pages/inspection-list/inspection-list.component';
import { MasterComponent } from './Shared/master/master.component';
import { InspectionlistComponent } from './Pages/inspectionlist/inspectionlist.component';
import { NewinspectionComponent } from './Pages/newinspection/newinspection.component';


export const routes: Routes = [
  // {
  //   path:'',
  //   component:MasterComponent
  // },
  {
    path:'notfound',
    component: NotFoundComponent   
   },
 {
  path:'check',
  component: CheckComponent,
  canActivate:[authGuard]
 },
 {
  path:'inspectionList',
  component:InspectionlistComponent
 },
 {
  path:'newinspection',
  component:NewinspectionComponent
 },
 {
  path:'login',
  loadComponent:()=>{
    return import('./Pages/login/login.component').then(m=>m.LoginComponent);    
  }
  
 }

];
