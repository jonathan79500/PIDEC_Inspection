import { inject, Injectable, Signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import {Order} from '../Models/Order.model';
import { Vendor } from '../Models/Vendor.model';
import { Project, Project11 } from '../Models/Project.model';
import { json } from 'stream/consumers';
import { Observable } from 'rxjs';
import { BasicVendorInfo } from '../Models/BasicVendorInfo.model';
import { BasicQCMemberInfo } from '../Models/BasicQCMemberInfo.model';

import { currentstateStore } from '../../Stores/currentstate.store';
import { UserTypes } from '../Models/UserTypes.enum';
import { ProjectContact } from '../Models/ProjectContact.model';
import { environment } from '../../../environments/environments';

@Injectable({
  providedIn: 'root'
})
export class BasicInfoService {

  public HostName :string = environment.apiUrl; //= "https://pims.pidec.com/pims";

  public APIName : string ="BasicInfo";
  public BasicAPIPath :string = this.HostName+'/'+this.APIName;
  public headers = new HttpHeaders().set('Content-Type', 'application/json');
 private currentAppState = inject(currentstateStore);

  constructor(private http:HttpClient) { }

  getVendorProjects(vendorId : string)
  {
    return this.http.get<Project[]>(this.BasicAPIPath+'/'+"GetProjectsByVendorId/"+vendorId );
  }

  getEdmsOrdersByProjectId(ProjectId:string)
  {
    return this.http.get<Order[]>(this.BasicAPIPath+'/'+"GetEdmsOrdersByProjectId/"+ProjectId);
  }


   getProjectDetailByProjectId(ProjectId:string)
  {
    return this.http.get<Project>(this.BasicAPIPath+'/'+"GetProjectDetailByProjectId/"+ProjectId);
  }


  getEdmsOrderDataIfExist(EdmsOrderId:string,ProjectId:string)
  {
    return this.http.get<Order>(this.BasicAPIPath+'/'+"GetEdmsOrderDataIfExist/"+EdmsOrderId+"/"+ProjectId);
  }

   getQCMemberDataByUserNameQuery():Observable<BasicQCMemberInfo>
  {
    let resultObservable =  this.http.get<any>(this.BasicAPIPath+'/'+"GetQCDataByQCMemberInfo/" );
  // resultObservable.subscribe((result)=>{
  //     this.vendorSharedService.setVendorBasicInfo(result)
  //     return this.vendorSharedService.getVendorBasicInfo();
  //   })
    return resultObservable;
  }



  ActivateOrderInInspectionSystem(order:Order){//:Observable<boolean>{

    const token = localStorage.getItem('token');
    const headers = new HttpHeaders();
    //const headers = { Authorization: `Bearer ${token}` };
    if(order.id =='' || order.id== undefined)
      order.id='00000000-0000-0000-0000-000000000000';
    return this.http.post<any>(this.BasicAPIPath+'/'+"ActivateOrderInInspectionSystem/",order,{headers:headers});

  }

  SaveProjectSettings(project:Project)
  {

//     const sampleProject: Project11 = {
//   id: "b3f9c1a2-4d5e-4c3b-9f6e-123456789abc",
//   title: "New Construction Project",
//   description: "This is a sample project for testing.",
//   number: "PRJ-2025-001",
//   fsid: "FS-7890",
//   ownerContacts: [
//   ],
//   mcContacts: [
//   ],
//   pidecInspectors: [

//   ],
//   tpInspectors: [
//   ],
//   createdDate: new Date(),
//   modifiedDate: new Date(),
//   createdBy: "admin",
//   modifiedBy: "admin"
// };

//     const project1 = {
//   id: "04e7b439-253c-4e71-ad1b-d8832654aa1b",
//   fSId: "68",
//   createdBy: "Admin",
//   createdDate: "2025-07-22T15:24:43.2605474",
//   description: "AGHAJARI 1,3,4,5 COMPRESSION STATIONS",
//   mCContacts: [
//   ],
//   modifiedBy: "Admin1",
//   modifiedDate: "2025-07-22T15:24:48.2605474",
//   number: "1235",
//   ownerContacts: [
//     {
//       id: "00000000-0000-0000-0000-000000000000",
//       attention: "",
//       cc: "",
//       email: "test.test@pidec.com",
//       fax: "",
//       name: "Owner 1",
//       phone: "",
//       contactType: 1,
//       mobile: "",
//       homePage: "",
//       isVisible: true,
//       address: {
//         id: "00000000-0000-0000-0000-000000000000",
//         city: "",
//         country: "",
//         state: "",
//         streetLine1: "",
//         streetLine2: "",
//         zipCode: "",
//       },
//       userId: "",
//     },
//   ],
//   pidecInspectors: [
//   ],
//   tPInspectors: [
//   ],
//   title: "ایستگاه های فشار افزائی و نم زدائی آغاجاری",
// }
    const headers = new HttpHeaders();
    return this.http.post<any>(this.BasicAPIPath+'/'+"SaveProjectSettings/", project,{headers:headers});
  }

}
