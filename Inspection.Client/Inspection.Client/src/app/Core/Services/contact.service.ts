
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ContactOption, Inspector,InspectorReview } from '../Models/Inspector.model';
import { environment } from '../../../environments/environments';
import { currentstateStore } from '../../Stores/currentstate.store';
import { Contact } from '../Models/Contact.model';

@Injectable({ providedIn: 'root' })
export class ContactService {
    currentAppState = inject(currentstateStore);

    public HostName :string  = environment.apiUrl;//=  "https://pims.pidec.com/pims";
    public APIName : string ="contacts";
    public ContactAPIPath :string = this.HostName+'/'+this.APIName;
    public headers = new HttpHeaders().set('Content-Type', 'application/json');


      constructor(private http:HttpClient) { }

  getAll()
  {

    const headers = new HttpHeaders();
    return this.http.get<ContactOption[]>(this.ContactAPIPath+'/'+"GetAll" );
  }

  getByProject_Types(projectId:string,requestedContactTypes:[number])
  {
    const headers = new HttpHeaders();
    const body = { projectId, requestedContactTypes };

    return this.http.post<ContactOption[]>(this.ContactAPIPath+'/'+"GetByProject_Types",body );
  }

  getByProject(projectId:string)
  {
    const body={ projectId}
    const headers = new HttpHeaders();
    return this.http.post<Contact[]>(this.ContactAPIPath+'/'+"GetByProject", body);
  }

  getByTypes(requestedContactTypes:[number])
  {
    const body={ requestedContactTypes}
    const headers = new HttpHeaders();
    return this.http.post<Contact[]>(this.ContactAPIPath+'/'+"GetByTypes", body);
  }

}
