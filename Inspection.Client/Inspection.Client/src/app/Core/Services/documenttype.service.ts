import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DocumentTypeAdmin, DocumentTypeSave } from '../Models/DocumentType.model';
import { environment } from '../../../environments/environments';

@Injectable({ providedIn: 'root' })
export class DocumentTypeService {
  private http = inject(HttpClient);

  // TODO: change to your API base url

   // TODO: change to your API base url
     public HostName :string  = environment.apiUrl;//=  "https://pims.pidec.com/pims";
     public DocumentTypeAPIName : string ="DocumentTypes/admin";
     public DocumentTypeAPIPath :string = this.HostName+'/'+this.DocumentTypeAPIName;
     public headers = new HttpHeaders().set('Content-Type', 'application/json');

  getAll(): Observable<DocumentTypeAdmin[]> {
    return this.http.get<DocumentTypeAdmin[]>(this.DocumentTypeAPIPath);
  }

  getById(id: string): Observable<DocumentTypeAdmin> {
    return this.http.get<DocumentTypeAdmin>(`${this.DocumentTypeAPIPath}/${id}`);
  }

  create(data: DocumentTypeSave): Observable<DocumentTypeAdmin> {
    return this.http.post<DocumentTypeAdmin>(this.DocumentTypeAPIPath, data);
  }

  update(id: string, data: DocumentTypeSave): Observable<DocumentTypeAdmin> {
    return this.http.put<DocumentTypeAdmin>(`${this.DocumentTypeAPIPath}/${id}`, data);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.DocumentTypeAPIPath}/${id}`);
  }
}
