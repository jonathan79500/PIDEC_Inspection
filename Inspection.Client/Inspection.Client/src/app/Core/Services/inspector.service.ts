import { Injectable, inject } from '@angular/core';
import { HttpClient,HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ContactOption, Inspector, InspectorSave, SkillOption } from '../Models/Inspector.model';

export interface InspectorFilter {
  inspectionCompanyContactId?: string;
  qcApprovalState?: number;
}

@Injectable({ providedIn: 'root' })
export class InspectorService {
  private http = inject(HttpClient);

  // TODO: change to your API base url
  private readonly baseUrl = '/api/inspectors';
  private readonly contactsUrl = '/api/contacts';
  private readonly skillsUrl = '/api/skills';

  // ----- inspectors -----
  getAll(filter?: InspectorFilter): Observable<Inspector[]> {
  let params = new HttpParams();
  if (filter?.inspectionCompanyContactId) {
    params = params.set('inspectionCompanyContactId', filter.inspectionCompanyContactId);
  }
  if (filter?.qcApprovalState !== undefined && filter?.qcApprovalState !== null) {
    params = params.set('qcApprovalState', String(filter.qcApprovalState));
  }
  return this.http.get<Inspector[]>(this.baseUrl, { params });
}

  create(inspector: InspectorSave): Observable<Inspector> {
    return this.http.post<Inspector>(this.baseUrl, inspector);
  }

  update(id: string, inspector: InspectorSave): Observable<Inspector> {
    return this.http.put<Inspector>(`${this.baseUrl}/${id}`, inspector);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  // ----- lookups -----
  getContacts(): Observable<ContactOption[]> {
    return this.http.get<ContactOption[]>(this.contactsUrl);
  }

  getSkills(): Observable<SkillOption[]> {
    return this.http.get<SkillOption[]>(this.skillsUrl);
  }

  getActiveSkills(): Observable<SkillOption[]> {
  return this.http.get<SkillOption[]>(this.skillsUrl+'/active');
}
}
