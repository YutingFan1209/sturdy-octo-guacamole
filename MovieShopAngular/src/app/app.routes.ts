import { Routes } from '@angular/router';
import { MovieDetails } from './movies/movie-details';
import { MovieList } from './movies/movie-list';
import { Login } from './account/login';
import { Register } from './account/register';
import { Purchases } from './account/purchases';
import { authGuard } from './account/auth.guard';

export const routes: Routes = [
  { path: '', component: MovieList, title: 'MovieShop' },
  { path: 'movies/:id', component: MovieDetails, title: 'Movie details' },
  { path: 'login', component: Login, title: 'Log in | MovieShop' },
  { path: 'register', component: Register, title: 'Register | MovieShop' },
  { path: 'purchases', component: Purchases, canActivate: [authGuard], title: 'My purchases | MovieShop' },
  { path: '**', redirectTo: '' }
];
