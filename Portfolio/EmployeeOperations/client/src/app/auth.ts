import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const token = sessionStorage.getItem('employee-operations-token');
  return next(
    token ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : request,
  );
};
export const authGuard: CanActivateFn = () =>
  sessionStorage.getItem('employee-operations-token')
    ? true
    : inject(Router).createUrlTree(['/sign-in']);
