import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Inspector,InspectorReview } from '../Models/Inspector.model';

@Injectable({ providedIn: 'root' })
export class InspectorQcService {
  private http = inject(HttpClient);

  // TODO: change to your API base url
  private readonly baseUrl = '/api/inspectors';

  /** Every inspector still awaiting a QC decision (QCApprovalState = NoAction). */
  getPending(): Observable<Inspector[]> {
    return this.http.get<Inspector[]>(`${this.baseUrl}/pending`);
  }

  approve(inspectorId: string, comment?: string): Observable<Inspector> {
    const body: InspectorReview = { comment: comment || null };
    return this.http.post<Inspector>(`${this.baseUrl}/${inspectorId}/approve`, body);
  }

  reject(inspectorId: string, comment: string): Observable<Inspector> {
    const body: InspectorReview = { comment };
    return this.http.post<Inspector>(`${this.baseUrl}/${inspectorId}/reject`, body);
  }
}
