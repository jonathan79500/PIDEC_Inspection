import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DocumentDraft, DocumentItem,  DocumentTypeOption } from '../Models/Document.model';
import { DocumentOwnerType } from '../Models/Document/DocumentOwnerType.model';
import { environment } from '../../../environments/environments';

@Injectable({ providedIn: 'root' })
export class DocumentService {
  private http = inject(HttpClient);

  // TODO: change to your API routes

    public HostName :string  = environment.apiUrl;//=  "https://pims.pidec.com/pims";
      public DocumentAPIName : string ="Documents";
      public DocumentTypeAPIName : string ="DocumentTypes";
      public DocumentAPIPath :string = this.HostName+'/'+this.DocumentAPIName;
      public DocumentTypeAPIPath :string = this.HostName+'/'+this.DocumentTypeAPIName;
      public headers = new HttpHeaders().set('Content-Type', 'application/json');




  /** Active document types; filtered by contact type when a code is given. */
  getTypes(contactTypeCode: number | null): Observable<DocumentTypeOption[]> {
    let params = new HttpParams();
    if (contactTypeCode !== null) params = params.set('contactTypeCode', String(contactTypeCode));
    return this.http.get<DocumentTypeOption[]>(this.DocumentTypeAPIPath, { params });
  }

  list(ownerType: DocumentOwnerType, ownerId: string): Observable<DocumentItem[]> {
    return this.http.get<DocumentItem[]>(this.DocumentAPIPath, { params: this.ownerParams(ownerType, ownerId) });
  }

  upload(ownerType: DocumentOwnerType, ownerId: string, draft: DocumentDraft): Observable<DocumentItem> {
    const body = new FormData();
    body.append('file', draft.file, draft.file.name);
    body.append('ownerType', String(ownerType));
    body.append('ownerId', ownerId);
    body.append('documentTypeId', draft.documentTypeId);
    if (draft.name) body.append('name', draft.name);
    if (draft.number) body.append('number', draft.number);
    if (draft.issueDate) body.append('issueDate', draft.issueDate);
    if (draft.expireDate) body.append('expireDate', draft.expireDate);
    return this.http.post<DocumentItem>(this.DocumentAPIPath, body);
  }

  download(ownerType: DocumentOwnerType, ownerId: string, documentId: string): Observable<Blob> {
    return this.http.get(`${this.DocumentAPIPath}/${documentId}/download`, {
      params: this.ownerParams(ownerType, ownerId),
      responseType: 'blob',
    });
  }

  delete(ownerType: DocumentOwnerType, ownerId: string, documentId: string): Observable<void> {
    return this.http.delete<void>(`${this.DocumentAPIPath}/${documentId}`, {
      params: this.ownerParams(ownerType, ownerId),
    });
  }

  private ownerParams(ownerType: DocumentOwnerType, ownerId: string): HttpParams {
    return new HttpParams().set('ownerType', ownerType).set('ownerId', ownerId);
  }
}
