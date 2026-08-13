import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Purchase } from './auth.models';
import { apiUrl } from '../api-url';

@Injectable({ providedIn: 'root' })
export class PurchaseService {
  private readonly http = inject(HttpClient);
  getPurchases(): Observable<Purchase[]> { return this.http.get<Purchase[]>(apiUrl('/api/users/purchases')); }
}
