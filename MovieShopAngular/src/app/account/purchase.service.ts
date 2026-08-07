import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Purchase } from './auth.models';

@Injectable({ providedIn: 'root' })
export class PurchaseService {
  private readonly http = inject(HttpClient);
  getPurchases(): Observable<Purchase[]> { return this.http.get<Purchase[]>('/api/users/purchases'); }
}
