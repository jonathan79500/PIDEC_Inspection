import { Component } from '@angular/core';
import {MatListModule} from '@angular/material/list'
import { User } from '../../../Core/Models/User.model';
import {MatTableModule} from '@angular/material/table';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatDatepickerModule} from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';

type ExtendedEDMSUser = User & {
selected:boolean,
NotAdded:boolean
}
type ExtendedQCUser = User & {
to:boolean,
cc:boolean,
from: Date,
until:Date
}

@Component({
  selector: 'app-usersettings',
  imports: [FormsModule,ReactiveFormsModule,MatListModule,MatTableModule,MatCheckboxModule,MatDatepickerModule,MatFormFieldModule],
  templateUrl: './usersettings.component.html',
  styleUrl: './usersettings.component.css'
})
export class UsersettingsComponent {

  InspectionQCsColumns :string[]=['userName','CC','TO','From','Until']

  EDMSQCMembers : ExtendedEDMSUser[]= [
    {userName: 'person1',name:'person1',domainId:1,email:'',id:'0',isActive:true,isAuthenticated:true,password:'',sAMAccountName:'',token:'',use_Id:1,userType:1,selected:false,NotAdded:true},
    {userName: 'person2',name:'person2',domainId:1,email:'',id:'0',isActive:true,isAuthenticated:true,password:'',sAMAccountName:'',token:'',use_Id:2,userType:1,selected:false,NotAdded:true},
    {userName: 'person3',name:'person3',domainId:1,email:'',id:'0',isActive:true,isAuthenticated:true,password:'',sAMAccountName:'',token:'',use_Id:3,userType:1,selected:false,NotAdded:true}
  ];

  InspectionQCMembers: ExtendedQCUser[]=[];

  AddToQCUsers():void{
    let selectedEdmsUser :ExtendedEDMSUser= this.EDMSQCMembers.find(member=>member.selected==true && member.NotAdded==true);
    let newQCUser :ExtendedQCUser = {...selectedEdmsUser,cc:false,to:false,from:new Date(Date.now()),until:new Date(Date.now())}
    this.InspectionQCMembers.push(newQCUser);
  }
}
