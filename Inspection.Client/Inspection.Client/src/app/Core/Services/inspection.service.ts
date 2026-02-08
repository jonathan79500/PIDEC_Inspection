import { InspectionReport } from './../Models/InspectionReport.model';
import { Attachment } from './../Models/Attachment.model';
import { Circulation } from './../Models/Circulation.model';
import { inspect } from 'util';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Inspection } from '../Models/Inspection.model';
import { lastValueFrom } from 'rxjs';
import { json } from 'stream/consumers';
import { InspectionNotificationFlow } from '../Models/InspectionNotificationFlow.model';
import { Project } from '../Models/Project.model';
import { InspectionNotificationDataSummary } from '../Models/InspectionNotificationDataSummary.model';


export interface queryNewNotificationNumberResult
{
  newNotificationNumber:string;
}

export interface NewNotificationNumberQueryParams
{
  projectNumber:string,
  orderNumber:string
}

@Injectable({
  providedIn: 'root'
})
export class InspectionService {

  public HostName :string = "http://localhost:5196";
  public PortNo : string = "5196";
  public APIName : string ="InspectionNotification";
  public InspectionAPIPath :string = this.HostName+'/'+'api'+'/'+this.APIName;
  public headers = new HttpHeaders().set('Content-Type', 'application/json');

  public RestAPIActions : object[]=[
    {
      Name : "GetAll",
      Value:"GetAll"
    },
    {
      Name : "GetById",
      Value:"GetById"
    },
    {
      Name : "Post",
      Value:"Post"
    },
    {
      Name : "Put",
      Value:"Put"
    },
    {
      Name : "Delete",
      Value:"Delete"
    }
  ]
  constructor(private http:HttpClient) { }


  getNewNotificationNumber(projectNumber:string,orderNumber:string)
  {
    console.log('in get newNotificationNumber',projectNumber,orderNumber);
    const token = localStorage.getItem('token');

    var param :NewNotificationNumberQueryParams={
      projectNumber:projectNumber,
      orderNumber: orderNumber

    }

    // const headers = new HttpHeaders({
    //   Authorization: `Bearer ${token}`,
    //   'Content-Type': `text/plain`
    // });

    console.log('params:',param);

    const headers = new HttpHeaders();

   // const params = new HttpParams() .set('projectNumber', projectNumber) .set('orderNumber', orderNumber);
    var responseResolver = this.http.post<queryNewNotificationNumberResult>(this.InspectionAPIPath+'/'+"GetNewNotificationNumber",param);
    responseResolver.subscribe((data)=>
    console.log('new not number:',data.newNotificationNumber))
    return responseResolver;
  }

  getInspectionList()
  {
    //console.log("in Inspection Service")
    return this.http.get<Inspection[]>(this.InspectionAPIPath+'/'+"GetAll");
  }

   getInspectionListByProjectIdUserId(projectId:string,userId:string)
  {
    if(projectId == undefined)
      projectId = '';
      const body = { projectId, userId };
    //console.log("in Inspection Service")
    return this.http.post<Inspection[]>(this.InspectionAPIPath+'/'+"GetReceivedInspectionsByProjectId_UserId",body);
  }

  getInspectionById(inspectionId:string)
  {
    //console.log("in Inspection Service")
    const headers = new HttpHeaders();
    console.log('ins id',inspectionId);
    return this.http.post<{inspection:Inspection,relatedReports :InspectionReport[]}>(this.InspectionAPIPath+'/'+"GetInspectionByInspectionNotificationId",{'inspectionNotificationId':inspectionId} );
  }


  getDraftedInspectionsByUserName(userName:string)
  {
    //console.log("in Inspection Service")
    const body={ userName}
    const headers = new HttpHeaders();
    return this.http.post<InspectionNotificationDataSummary[]>(this.InspectionAPIPath+'/'+"GetDraftedInspectionsByUserName", body);
  }

  getSentInspectionsByUserName(userName:string)
  {
    //console.log("in Inspection Service")
    const body={ userName}
    const headers = new HttpHeaders();
    return this.http.post<InspectionNotificationDataSummary[]>(this.InspectionAPIPath+'/'+"GetSentInspectionsByUserName", body);
  }

  getNotStartedInspectionsByUserName(userName:string)
  {
    //console.log("in Inspection Service")
    const body={ userName}
    const headers = new HttpHeaders();
    return this.http.post<InspectionNotificationDataSummary[]>(this.InspectionAPIPath+'/'+"GetNotStartedInspectionsByUserName", body);
  }


  getInspectionByNotificationNumber(notificationNumber:string)
  {
    //console.log("in Inspection Service")
    const headers = new HttpHeaders();
    console.log('ins id',notificationNumber);
    return this.http.post<InspectionNotificationFlow>(this.InspectionAPIPath+'/'+"GetInspectionByNumber",{'notificationNumber':notificationNumber} );
  }

  saveNewInspection(newInspection: Inspection)
  {
    const headers = new HttpHeaders();

    return this.http.post<any>(this.InspectionAPIPath+'/'+"SaveNew", newInspection,{ headers: headers } );
  }

   saveCirculationData(formData: FormData)
  {
    const headers = new HttpHeaders();

     return this.http.post<Circulation>(this.InspectionAPIPath+'/'+"CompleteTask",formData);//,{ headers: headers });
  }

  // async saveNewInspectionFull(formData: FormData)
  // {
  //   return await lastValueFrom(this.http.post(this.InspectionAPIPath+'/'+"SendInspectionNotification",formData))
  // }
FetchFileFromServer(id:string)
{
  window.open(this.InspectionAPIPath + "/GetAttachmentById/" + id, "_blank");
  //return this.http.get<any>(this.InspectionAPIPath+"/"+"GetAttachmentById/"+id);
}

   saveNewInspectionFull(formData: FormData)
  {
    const headers = new HttpHeaders();
     return this.http.post<{inspection:Inspection,currentCirculation:Circulation}>(this.InspectionAPIPath+'/'+"SendInspectionNotification",formData);//,{ headers: headers });
  }

  draftNewInspectionFull(formData: FormData)
  {
     const headers = new HttpHeaders();
     return this.http.post<{inspection:Inspection,currentCirculation:Circulation}>(this.InspectionAPIPath+'/'+"DraftInspectionNotification",formData);

    //return await lastValueFrom()
  }

  saveNeww()
  {
    const headers = new HttpHeaders();


  var str = {
    id :'a5dcc11b-846a-4d16-91e1-1dfb9b86c4e2',
    name :'',
         userName:'',
         domainId:1,
         Email :'',
         Password :''
  };

    return this.http.post<any>(this.InspectionAPIPath+'/'+"SaveNeww", str,{ headers: headers } );
  }
}
