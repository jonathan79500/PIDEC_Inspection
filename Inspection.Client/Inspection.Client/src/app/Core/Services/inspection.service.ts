import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inspection } from '../Models/Inspection.model';

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


  getInspectionList()
  {
    return this.http.get<Inspection[]>(this.InspectionAPIPath+'/'+"GetAll");
  }

  saveNewInspection(newInspection: Inspection)
  {
    const headers = new HttpHeaders();
   
    return this.http.post<any>(this.InspectionAPIPath+'/'+"SaveNew", newInspection,{ headers: headers } );
  }
}
