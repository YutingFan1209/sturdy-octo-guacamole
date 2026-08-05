import { AsyncPipe, DatePipe, DecimalPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { catchError, map, of, switchMap } from 'rxjs';
import { MovieService } from './movie.service';

@Component({
  selector: 'app-movie-details',
  imports: [AsyncPipe, DatePipe, DecimalPipe, RouterLink],
  templateUrl: './movie-details.html',
  styleUrl: './movie-details.css'
})
export class MovieDetails {
  private readonly route = inject(ActivatedRoute);
  private readonly movieService = inject(MovieService);
  error = false;
  movie$ = this.route.paramMap.pipe(
    map(params => Number(params.get('id'))),
    switchMap(id => this.movieService.getMovie(id)),
    catchError(() => { this.error = true; return of(null); })
  );
}
