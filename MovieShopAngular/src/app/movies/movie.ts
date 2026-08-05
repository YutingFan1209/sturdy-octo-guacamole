export interface MovieSummary {
  id: number;
  title: string;
  releaseDate: string;
  price: number;
  posterUrl: string;
  revenue: number;
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface MovieCast { name: string; character: string; profileUrl: string | null; }
export interface MovieTrailer { name: string; url: string; }

export interface MovieDetails extends MovieSummary {
  backdropUrl: string;
  overview: string;
  tagline: string;
  rating: number;
  genre: string;
  genres: string[];
  runtime: number;
  budget: number;
  casts: MovieCast[];
  trailers: MovieTrailer[];
}
