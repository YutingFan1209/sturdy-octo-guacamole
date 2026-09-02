import { Routes } from '@angular/router';
import { authGuard } from './auth';
import { CreateRequestPage, OpenRequestPage, RequestPage, SignInPage } from './request-pages';
export const routes: Routes = [
  { path: 'sign-in', component: SignInPage },
  { path: 'requests/new', component: CreateRequestPage, canActivate: [authGuard] },
  { path: 'requests/open', component: OpenRequestPage, canActivate: [authGuard] },
  { path: 'requests/:id', component: RequestPage, canActivate: [authGuard] },
  { path: '', pathMatch: 'full', redirectTo: 'requests/new' },
];
