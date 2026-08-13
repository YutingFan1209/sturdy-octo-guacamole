import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { MovieDetails, MovieSummary, PagedResult } from './movie';
import { apiUrl } from '../api-url';

@Injectable({ providedIn: 'root' })
export class MovieService {
  private readonly http = inject(HttpClient);

  getMovies(): Observable<MovieSummary[]> {
    return this.http.get<MovieSummary[]>(apiUrl('/api/movies'));
  }

  getTopGrossingMovies(pageNumber = 1, pageSize = 10): Observable<PagedResult<MovieSummary>> {
    return this.http.get<PagedResult<MovieSummary>>(apiUrl('/api/movies/top-grossing'), {
      params: { pageNumber, pageSize }
    });
  }

  getMovie(id: number): Observable<MovieDetails> {
    return this.http.get<MovieDetails>(apiUrl(`/api/movies/${id}`));
  }
}
