import { Routes } from '@angular/router';
import { MovieDetails } from './movies/movie-details';
import { MovieList } from './movies/movie-list';

export const routes: Routes = [
  { path: '', component: MovieList, title: 'MovieShop' },
  { path: 'movies/:id', component: MovieDetails, title: 'Movie details' },
  { path: '**', redirectTo: '' }
];
