import { FormUser } from './../../Core/FormTypes/InspectionFormTypes';
import { User } from './../../Core/Models/User.model';
import { Subject } from 'rxjs';
import { QCInfoStore } from './../../Stores/QCInfo.store';
import { userInfo } from 'node:os';
import { BasicInfoService } from './../../Core/Services/basic-info.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, Inject } from '@angular/core';
import { FormBuilder, FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FormContact} from '../../Core/FormTypes/InspectionFormTypes';
import { UserService } from '../../Core/Services/user.service';
import { currentstateStore } from '../../Stores/currentstate.store';

@Component({
  selector: 'app-contact-person-list',
  imports: [CommonModule,FormsModule,ReactiveFormsModule],
  templateUrl: './contact-person-list.component.html',
  styleUrl: './contact-person-list.component.css'
})
export class ContactPersonListComponent implements OnInit{

  formGroupUser!:FormUser ;
  QCStore = inject(QCInfoStore);
  currentAppState = inject(currentstateStore);
  existedUsers : [];

  constructor(private userInfoService : UserService,private fb: FormBuilder) {

  }

  ngOnInit(): void {

     this.formGroupUser = this.fb.group({
      id: new FormControl(''),
      contactType: new FormControl(0),
      email: new FormControl(''),
      attention: new FormControl(''),
      name: new FormControl(''),
      phone: new FormControl(''),
      mobile: new FormControl(''),
      fax: new FormControl(''),
      cc: new FormControl(''),
      homePage: new FormControl(''),
      address: this.fb.group({
            id: new FormControl(''),
            country: new FormControl(''),
            state: new FormControl(''),
            city: new FormControl(''),
            streetLine1: new FormControl(''),
            streetLine2: new FormControl(''),
            zipCode: new FormControl('')
    }) ,
      userId: new FormControl(''),
      isVisible: new FormControl(true),
      userName: new FormControl(''),
      password: new FormControl('')
    });
      let currentProjectId = this.currentAppState.currentproject().id;
        this.userInfoService.getDefinedUsersByProjectId(currentProjectId).subscribe((usersData)=>{
        console.log(usersData);
        this.existedUsers = usersData;
        }
        )
}

onSaveNewUser(){

}


}
