import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, throwError } from 'rxjs';
export type RequestStatus =
  'Draft' | 'Submitted' | 'Approved' | 'Rejected' | 'Completed' | 'Cancelled';
export interface Transition {
  previousStatus: RequestStatus;
  newStatus: RequestStatus;
  actorId: string;
  occurredAt: string;
  reason?: string;
}
export interface EquipmentRequest {
  id: string;
  employeeId: string;
  item: string;
  justification: string;
  status: RequestStatus;
  version: number;
  history: Transition[];
}
@Injectable({ providedIn: 'root' })
export class EquipmentRequestService {
  private http = inject(HttpClient);
  private base = '/api/equipment-requests';
  create(input: { item: string; justification: string }) {
    return this.http.post<EquipmentRequest>(this.base, input).pipe(catchError(this.failure));
  }
  get(id: string) {
    return this.http.get<EquipmentRequest>(`${this.base}/${id}`).pipe(catchError(this.failure));
  }
  transition(id: string, action: string, expectedVersion: number, reason?: string) {
    return this.http
      .post<EquipmentRequest>(`${this.base}/${id}/${action}`, { expectedVersion, reason })
      .pipe(catchError(this.failure));
  }
  private failure(e: HttpErrorResponse) {
    return throwError(() => new Error(e.error?.detail ?? 'The request could not be completed.'));
  }
}
