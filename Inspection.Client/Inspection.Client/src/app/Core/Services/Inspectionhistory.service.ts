import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RevisionHistory } from '../Models/Inspectionhistory.model';
import { environment } from '../../../environments/environments';

@Injectable({ providedIn: 'root' })
export class InspectionHistoryService {
  private http = inject(HttpClient);

  // TODO: change to your API base url
   public HostName :string  = environment.apiUrl;//=  "https://pims.pidec.com/pims";
    public APIName : string ="inspection-history";
    public InspectionHistoryAPIPath :string = this.HostName+'/'+this.APIName;
    public headers = new HttpHeaders().set('Content-Type', 'application/json');


  getByRequestId(requestId: string): Observable<RevisionHistory[]> {
    return this.http.get<RevisionHistory[]>(`${this.InspectionHistoryAPIPath}/${requestId}`);
  }
}
