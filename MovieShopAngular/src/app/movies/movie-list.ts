import { AsyncPipe, CurrencyPipe, DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { catchError, map, of, switchMap } from 'rxjs';
import { PagedResult, MovieSummary } from './movie';
import { MovieService } from './movie.service';

@Component({
  selector: 'app-movie-list',
  imports: [AsyncPipe, CurrencyPipe, DatePipe, RouterLink],
  templateUrl: './movie-list.html',
  styleUrl: './movie-list.css'
})
export class MovieList {
  private readonly movieService = inject(MovieService);
  private readonly route = inject(ActivatedRoute);
  error = false;
  page$ = this.route.queryParamMap.pipe(
    map(params => Math.max(Number(params.get('page')) || 1, 1)),
    switchMap(page => {
      this.error = false;
      return this.movieService.getTopGrossingMovies(page, 10);
    }),
    catchError(() => {
      this.error = true;
      return of<PagedResult<MovieSummary> | null>(null);
    })
  );

  pageNumbers(totalPages: number): number[] {
    return Array.from({ length: totalPages }, (_, index) => index + 1);
  }
}
