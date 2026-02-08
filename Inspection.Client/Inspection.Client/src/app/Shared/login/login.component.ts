import { InspectorInfoStore } from './../../Stores/InspectorInfo.store';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { User } from '../../Core/Models/User.model';
import { Router } from '@angular/router';
import { UserService } from '../../Core/Services/user.service';
import { InspectionService } from '../../Core/Services/inspection.service';
import { UserTypes } from '../../Core/Models/UserTypes.enum';
import { CommonModule } from '@angular/common';
import {currentstateStore} from '../../Stores/currentstate.store'
import {JwtPayload,JwtDecodeOptions, JwtHeader,jwtDecode,InvalidTokenError} from 'jwt-decode'
import { vendorInfoStore } from '../../Stores/vendorInfo.store';
import { QCInfoStore } from '../../Stores/QCInfo.store';
import { BasicInfoService } from '../../Core/Services/basic-info.service';
import { PIDECDeptInfoStore } from '../../Stores/PIDECDeptInfo.store';

type userForm = FormGroup<{
  userName:FormControl<string>
  password :FormControl<string>,
  domainId : FormControl<number>
}>;
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule,CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent  implements OnInit{


  currentAppState = inject(currentstateStore);
  VendorState = inject(vendorInfoStore);
  PIDECDeptState = inject(PIDECDeptInfoStore);
  InspectorState = inject(InspectorInfoStore);
  QCState = inject(QCInfoStore);
  isSubmitted :boolean= false;
  public isTrustedUser :boolean = false;
  form : userForm;

  constructor(private builder: FormBuilder,private router : Router,private userService: UserService,private inspectionService:InspectionService,private basicService:BasicInfoService) {

    this.form = this.generateUserForm();
  }

  hasDisplayableError(controlName :string):boolean
  {
    const control = this.form.get(controlName);
    return Boolean(control?.invalid ) && (this.isSubmitted && Boolean(control?.touched));
  }

 generateUserForm():userForm{
  return this.builder.group({
    //   userName:'1235-EL-509A-1345',
    //  password:'49gpa',
    userName:'akbari.elahe',
     password:'@2050926Rr',
    // //   userName:'monavari.ali',

    //password:'Z#6b^Fkd3#^g',

    //  userName: 'pInspector',
    //  password:'CJL&zx&P2nJA',
    domainId:0
  })
 }

 ngOnInit(): void {
}

login() {


    if(this.form.valid)
    {

      this.userService.IsValidUser(this.form.getRawValue()).subscribe((userData)=>{

        if(userData.user.token != ''){

          this.currentAppState.loggedIn(userData.user);

          localStorage.setItem("token",userData.user.token);
          localStorage.setItem("password",this.form.getRawValue().password);

          const decodedToken = jwtDecode(userData.user.token);

          let userRoles =  decodedToken["role"] ||  decodedToken["roles"];

          if (!Array.isArray(userRoles))
          {
              userRoles = [userRoles];
          }


          const lowerCaseUserRoles = userRoles.map(element => element.toLowerCase());


        if(localStorage.getItem('token') !='')
        {
          this.isTrustedUser= true;

          if(lowerCaseUserRoles.includes('vendor'))
          {
            this.VendorState.SetBasicVendorInfo(userData.vendorInfo);
            this.router.navigate(['vendor']);
          }
          else if(lowerCaseUserRoles.includes('qc_lead') || lowerCaseUserRoles.includes('qc_manager'))
          {
            this.QCState.SetBasicQCInfo(userData.qcMemberInfo);
            console.log("in login page_QC Codition",lowerCaseUserRoles);
            //this.userService.ActivateOrderInInspectionSystem1(this.form.getRawValue()).subscribe((result)=>{console.log(result)});
            this.router.navigate(['pidec']);
          }
          else if(lowerCaseUserRoles.includes('pidecdepartment_originator') || lowerCaseUserRoles.includes('pidecdepartment_lead'))
          {
            this.PIDECDeptState.SetBasicPIDECDeptInfo(userData.pidecLeadsInfo);
            console.log("in login page_PIDECDeptInfo Codition",lowerCaseUserRoles);
            //this.userService.ActivateOrderInInspectionSystem1(this.form.getRawValue()).subscribe((result)=>{console.log(result)});
            this.router.navigate(['pidecDept']);
          }
           else if(lowerCaseUserRoles.includes('pidec_inspector') || lowerCaseUserRoles.includes('thirdpart_inspector'))
          {
            this.InspectorState.SetBasicInspectorInfo(userData.inspectorInfo);
            console.log("in login page_PIDECDeptInfo Codition",lowerCaseUserRoles);
            //this.userService.ActivateOrderInInspectionSystem1(this.form.getRawValue()).subscribe((result)=>{console.log(result)});
            this.router.navigate(['inspector']);
          }

        }

      }
      })
    }

  }

}
