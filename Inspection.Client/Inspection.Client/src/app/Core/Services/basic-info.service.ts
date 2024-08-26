import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import {Order} from '../Models/Order.model';
import { Vendor } from '../Models/Vendor.model';
import { Project } from '../Models/Project.model';
import { json } from 'stream/consumers';


@Injectable({
  providedIn: 'root'
})
export class BasicInfoService {

  public HostName :string = "http://localhost:5196";
  public PortNo : string = "5196";
  public APIName : string ="BasicInfo";
  public BasicAPIPath :string = this.HostName+'/'+'api'+'/'+this.APIName;
  public headers = new HttpHeaders().set('Content-Type', 'application/json');
 
  constructor(private http:HttpClient) { }

  getVendorProjects(vendorId : string)
  { 
    return this.http.get<Project[]>(this.BasicAPIPath+'/'+"GetProjectsByVendorId/"+vendorId );
  }


}
