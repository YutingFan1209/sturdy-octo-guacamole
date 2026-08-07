import { AsyncPipe, CurrencyPipe, DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { catchError, of } from 'rxjs';
import { PurchaseService } from './purchase.service';

@Component({ selector: 'app-purchases', imports: [AsyncPipe, CurrencyPipe, DatePipe, RouterLink], templateUrl: './purchases.html', styleUrl: './purchases.css' })
export class Purchases {
  private readonly purchaseService = inject(PurchaseService);
  error = false;
  purchases$ = this.purchaseService.getPurchases().pipe(catchError(() => { this.error = true; return of([]); }));
}
