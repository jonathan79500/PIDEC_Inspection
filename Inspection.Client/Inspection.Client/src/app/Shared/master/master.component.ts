import { CommonModule, NgTemplateOutlet } from '@angular/common';
import { Component,computed,effect,input, Input, Signal, signal, TemplateRef } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LoginComponent } from '../../Pages/login/login.component';
import { BrowserModule } from '@angular/platform-browser';
import { sign } from 'node:crypto';
import { CheckComponent } from '../../Pages/check/check.component';
import { NotFoundComponent } from '../../Pages/not-found/not-found.component';
import { AgGridAngular } from 'ag-grid-angular';
import { RouterOutlet,RouterLink ,RouterLinkActive} from '@angular/router';
import { Router } from '@angular/router';


 enum NavbarLinkList {
  waitingInspectionsTitle = "بازرسی های در لیست انتظار",
  RejectedInspectionList = "بازرسی های برگشتی"
}

@Component({
  selector: 'app-master',
  standalone: true,
  imports: [ RouterOutlet,LoginComponent,FormsModule,CheckComponent,NotFoundComponent],
  templateUrl: './master.component.html',
  styleUrl: './master.component.css'
})



export class MasterComponent {


  constructor(private router: Router) {  
 
    effect(()=>{
    console.log(`the count is ${this.count}`);
  })  
  }

size = input<string>();
count = signal<number>(0);
count2 = computed(()=>this.count()*2);
count3 = signal<number>(this.count2());
title =signal<string>("Angular Tot");

CountChange(){
  //this.count.set(10);
  this.count.update(v=>v+1);
  this.count3.update(v=>v*3);
}

goToInspectionList(){
  this.router.navigate(['/inspectionList']);
}

gotoRejectedInspections()
{
  this.router.navigate(['/inspectionList']);
}

CreateNewInspectionNotification() {
this.router.navigate(['/newinspection'])
  }

}
