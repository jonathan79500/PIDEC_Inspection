import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Inspection } from '../Models/Inspection.model';

@Injectable({
  providedIn: 'root'
})
export class InspectionService {

  public HostName :string = "http://localhost:5196";
  public PortNo : string = "5196";
  public APIName : string ="InspectionNotification";
  public InspectionAPIPath :string = this.HostName+'/'+'api'+'/'+this.APIName;


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
    return this.http.post(this.InspectionAPIPath+'/'+"SaveNew",newInspection);
  }
}
