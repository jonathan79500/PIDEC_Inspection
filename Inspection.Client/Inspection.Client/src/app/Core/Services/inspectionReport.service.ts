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
import { InspectionReport } from '../Models/InspectionReport.model';


export interface queryNewInspectionReportNumberResult
{
  newNotificationNumber:string;
}

export interface NewInspectionReportNumberQueryParams
{
  InspectionNotificationNumber:string
}

@Injectable({
  providedIn: 'root'
})
export class InspectionReportService {

  public HostName :string = "http://localhost:5196";
  public PortNo : string = "5196";
  public APIName : string ="InspectionReport";
  public InspectionReportAPIPath :string = this.HostName+'/'+'api'+'/'+this.APIName;
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


  getNewInspectionReportNumber(inspectionNotificationNumber:string)
  {
    const token = localStorage.getItem('token');

    var param :NewInspectionReportNumberQueryParams={
      InspectionNotificationNumber : inspectionNotificationNumber
    }

    // const headers = new HttpHeaders({
    //   Authorization: `Bearer ${token}`,
    //   'Content-Type': `text/plain`
    // });

    console.log('params:',param);

    const headers = new HttpHeaders();

   // const params = new HttpParams() .set('projectNumber', projectNumber) .set('orderNumber', orderNumber);
    var responseResolver = this.http.post<queryNewInspectionReportNumberResult>(this.InspectionReportAPIPath+'/'+"GetNewNotificationNumber",param);
    responseResolver.subscribe((data)=>
    console.log('new not number:',data.newNotificationNumber))
    return responseResolver;
  }

  getInspectionList()
  {
    //console.log("in Inspection Service")
    return this.http.get<Inspection[]>(this.InspectionReportAPIPath+'/'+"GetAll");
  }

   getInspectionReportsByInspectionId(InspectionNotificationId:string)
  {
      const body = { InspectionNotificationId };
    //console.log("in Inspection Service")
    return this.http.post<Inspection[]>(this.InspectionReportAPIPath+'/'+"GetInspectionReportByInspectionNotificationId",body);
  }

  getInspectionReportById(inspectionId:string)
  {
    //console.log("in Inspection Service")
    const headers = new HttpHeaders();
    console.log('ins id',inspectionId);
    return this.http.post<InspectionReport>(this.InspectionReportAPIPath+'/'+"GetInspectionByInspectionNotificationId",{'inspectionNotificationId':inspectionId} );
  }


  getDraftedReportsByUserName(userName:string)
  {
    //console.log("in Inspection Service")
    const body={ userName}
    const headers = new HttpHeaders();
    return this.http.post<InspectionNotificationDataSummary[]>(this.InspectionReportAPIPath+'/'+"GetDraftedInspectionsByUserName", body);
  }

  getSavedtInspectionReportsByUserName(userName:string)
  {
    //console.log("in Inspection Service")
    const body={ userName}
    const headers = new HttpHeaders();
    return this.http.post<InspectionNotificationDataSummary[]>(this.InspectionReportAPIPath+'/'+"GetSentInspectionsByUserName", body);
  }

FetchFileFromServer(id:string)
{
  window.open(this.InspectionReportAPIPath + "/GetAttachmentById/" + id, "_blank");
  //return this.http.get<any>(this.InspectionAPIPath+"/"+"GetAttachmentById/"+id);
}
   saveCirculationData(formData: FormData)
  {
    const headers = new HttpHeaders();

     return this.http.post<Circulation>(this.InspectionReportAPIPath+'/'+"CompleteTask",formData);//,{ headers: headers });
  }

   saveNewInspectionReport(formData: FormData)
  {
    const headers = new HttpHeaders();
     return this.http.post<{inspectionReport: InspectionReport,currentCirculation:Circulation}>(this.InspectionReportAPIPath+'/'+"SaveNewInspectionReport",formData,{ headers: headers });
  }


  UpdateInspectionReport(updatedInspectionReport:InspectionReport)
  {

  }
  draftNewInspectionReport(formData: FormData)
  {
     const headers = new HttpHeaders();
     return this.http.post<{inspectionReport: InspectionReport,currentCirculation:Circulation}>(this.InspectionReportAPIPath+'/'+"DraftInspectionNotification",formData,{ headers: headers });

    //return await lastValueFrom()
  }


}
