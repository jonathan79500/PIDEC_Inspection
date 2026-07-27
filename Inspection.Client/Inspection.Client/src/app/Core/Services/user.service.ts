import { computed, inject, Injectable,  signal,Signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inspection } from '../Models/Inspection.model';
import { lastValueFrom } from 'rxjs';
import { User } from '../Models/User.model';
import { Vendor } from '../Models/Vendor.model';
import { UserTypes } from '../Models/UserTypes.enum';
import { sign } from 'crypto';
import { JwtHelperService } from '@auth0/angular-jwt';
import { currentstateStore } from '../../Stores/currentstate.store';
import { environment } from '../../../environments/environments';


@Injectable({
  providedIn: 'root'
})
export class UserService {

  public
  public HostName :string  = environment.apiUrl; //=  "https://pims.pidec.com/pims";
  public APIName : string ="Authentication";
  public LoginAPIPath :string = this.HostName+'/'+this.APIName;
  public headers = new HttpHeaders().set('Content-Type', 'application/json');

  //public vendorData : Signal<{}>;
private vendorData = signal<{}>({});
private jwtHelper = new JwtHelperService()
private currentAppState = inject(currentstateStore);

  constructor(private http:HttpClient) {


  }

  CreateUser(userData:any)
  {
    const headers = new HttpHeaders();
    var MappeduserData :User
    ={
      id:'00000000-0000-0000-0000-000000000000',
      name:userData.fullName,
      email:userData.email,
      userName :userData.fullName,
      domainId :0,
      password :userData.password,
      isActive:false,
      isAuthenticated:false,
      sAMAccountName:'',
      use_Id:0,
      userType:0,
      token:'',
      oldPassword:'',
      newPassword:'',
      confirmPassword:''
    }

    return this.http.post<any>(this.LoginAPIPath+'/'+"register",MappeduserData,{  headers: headers });
  }


  SignIn(userData:any)
  {
    const headers = new HttpHeaders();
    var MappeduserData :User
    ={
      id:'00000000-0000-0000-0000-000000000000',
      name:userData.fullName,
      email:userData.email,
      userName :userData.fullName,
      domainId :0,
      password :userData.password,
      isActive:false,
      isAuthenticated:false,
      sAMAccountName:'',
      use_Id:0,
      userType:0,
      token:'',
      oldPassword:'',
        newPassword:'',
      confirmPassword:''
    }

    return this.http.post<any>(this.LoginAPIPath+'/'+"login",MappeduserData,{  headers: headers });
  }


  IsValidUser(userData)
  {

    var MappeduserData :User
    ={
      id:'00000000-0000-0000-0000-000000000000',
      name:'',
      email:'',
      userName :userData.userName,
      domainId :1,
      password :userData.password,
      isActive:false,
      isAuthenticated:false,
      sAMAccountName:'',
      use_Id:0,
      userType:0,
      token:'',
      oldPassword:'',
        newPassword:'',
      confirmPassword:''
    }


    console.log(MappeduserData);
    const headers = new HttpHeaders();
    return this.http.post<any>(this.LoginAPIPath+'/'+"IsValidUser",MappeduserData,{  headers: headers });
  }

  ActivateOrderInInspectionSystem1(userData){

    // const headers = new HttpHeaders({
    //   'Content-Type': 'application/json',
    // });

    var MappeduserData :User = {
          id:'00000000-0000-0000-0000-000000000000',
          name:'',
          email:'',
          userName :userData.userName,
          domainId :userData.domainId,
          password :userData.password,
          isActive:false,
          isAuthenticated:false,
          sAMAccountName:'',
          use_Id:0,
          userType:UserTypes.Vendor,
          token:'',
          oldPassword:'',
            newPassword:'',
      confirmPassword:''
        }
        const headers = new HttpHeaders();
    // const token = localStorage.getItem('token');
    // const headers = { Authorization: `Bearer ${token}` };
   return this.http.post<any>(this.LoginAPIPath+'/'+"ActivateOrderInInspectionSystem",MappeduserData,{headers:headers});

  }

  GetVendorData()
  {

    var userData = this.currentAppState.currentuser();
    console.log(userData);
    var MappeduserData :User
    ={
      id:'00000000-0000-0000-0000-000000000000',
      name:'',
      email:'',
      userName :userData.userName,
      domainId :userData.domainId,
      password :userData.password,
      isActive:false,
      isAuthenticated:false,
      sAMAccountName:'',
      use_Id:0,
      userType:UserTypes.Vendor,
      token:'',
      oldPassword:'',
        newPassword:'',
      confirmPassword:''
    }

    const token = localStorage.getItem('token');
    const headers = { Authorization: `Bearer ${token}` };
    var query = this.http.post<any>(this.LoginAPIPath+'/'+"GetVendorDataByVendorInfo_Remotely",MappeduserData,{  headers: headers });
    return query;

  }

  GetQCMemberData()
  {

    var userData = this.currentAppState.currentuser();
    console.log(userData);
    var MappeduserData :User = {
      id:'00000000-0000-0000-0000-000000000000',
      name:'',
      email:'',
      userName :userData.userName,
      domainId :userData.domainId,
      password :userData.password,
      isActive:false,
      isAuthenticated:false,
      sAMAccountName:'',
      use_Id:0,
      userType:UserTypes.Vendor,
      token:'',
        newPassword:'',
        oldPassword:'',
      confirmPassword:''
    }

    const token = localStorage.getItem('token');
    const headers = { Authorization: `Bearer ${token}` };

    console.log('In GetQCMemberData :',token);
    console.log('In GetQCMemberData :', MappeduserData);
    var query = this.http.post<any>(this.LoginAPIPath+'/'+"GetQCDataByQCMemberInfo_Remotely",MappeduserData,{  headers: headers });
    return query;

  }

  GetInspectorData()
  {

    var userData = this.currentAppState.currentuser();
    console.log(userData);
    var MappeduserData :User = {
      id:'00000000-0000-0000-0000-000000000000',
      name:'',
      email:'',
      userName :userData.userName,
      domainId :userData.domainId,
      password :userData.password,
      isActive:false,
      isAuthenticated:false,
      sAMAccountName:'',
      use_Id:0,
      userType:UserTypes.PIDEC_Inspector,
      token:'',
        newPassword:'',
        oldPassword:'',
      confirmPassword:''
    }

    const token = localStorage.getItem('token');
    const headers = { Authorization: `Bearer ${token}` };

    console.log('In GetInspectorData :',token);
    console.log('In GetInspectorData :', MappeduserData);
    var query = this.http.post<any>(this.LoginAPIPath+'/'+"GetInspectorDataByInspectorInfo_Remotely",MappeduserData,{  headers: headers });
    return query;

  }

GetPIDECDeptMemberData()
  {

    var userData = this.currentAppState.currentuser();
    console.log(userData);
    var MappeduserData :User = {
      id:'00000000-0000-0000-0000-000000000000',
      name:'',
      email:'',
      userName :userData.userName,
      domainId :userData.domainId,
      password :userData.password,
      isActive:false,
      isAuthenticated:false,
      sAMAccountName:'',
      use_Id:0,
      userType:UserTypes.Vendor,
      token:'',
      oldPassword:'',
        newPassword:'',
      confirmPassword:''
    }

    const token = localStorage.getItem('token');
    const headers = { Authorization: `Bearer ${token}` };

    console.log('In GetPIDECLeadMemberData :',token);
    console.log('In GetPIDECLeadMemberData :', MappeduserData);
    var query = this.http.post<any>(this.LoginAPIPath+'/'+"GetPIDECLeadDataByPIDECLeadInfo_Remotely",MappeduserData,{  headers: headers });
    return query;

  }

  // isTokenExpired():boolean
  // {
  //   const token= localStorage.getItem('token');
  //   return token ? !this.jwtHelper.isTokenExpired(token) : false;
  // }
 getDefinedUsersByProjectId(ProjectId:string)
  {
      let usersInfo = this.http.get<any>(this.LoginAPIPath+'/'+'GetUserDataByProjectId'+'/'+ProjectId);
      return usersInfo;
  }

  isTokenExpired(): boolean {
    const token = localStorage.getItem('token'); // Assuming token is stored in localStorage
    if (!token) {
      console.log('Token not found');
      return false;
    }
    // Implement your token expiration logic here
    const isExpired = false; // Replace with actual expiration check
    console.log('Token expired:', isExpired);
    return !isExpired;
  }

  logOut():void
  {
     localStorage.setItem('token',null);
  }

  resetPassword(token:string,userId:string,newpass:string,newPassConfirm:string)
  {
    let Receivedtoken = "";
  if(token == undefined || token =='' || token==null)
    Receivedtoken = localStorage.getItem('token');
  else
    Receivedtoken= token;

  var userData = this.currentAppState.currentuser();
   if(userId == undefined || userId==null || userId=='')
     userId = userData.id;

  const oldPass =localStorage.getItem("password")
  const headers = { Authorization: `Bearer ${token}` };
  var query = this.http.post<any>(this.LoginAPIPath+'/'+"SetPassword",{
  id: userId,
  token: token,
  newPassword: newpass,
  oldPassword:oldPass,
  confirmPassword:newPassConfirm
},{  headers: headers });
    return query;
  }
}
