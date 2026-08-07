export interface LoginRequest { email: string; password: string; }

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
}

export interface UserInfo {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string | null;
}

export interface LoginResponse extends UserInfo {
  token: string;
  expiresAtUtc: string;
}

export interface AuthSession { user: UserInfo; token: string; expiresAtUtc: string; }

export interface Purchase {
  id: number;
  purchaseNumber: string;
  totalPrice: number;
  purchaseDateTime: string;
  movieId: number;
  movieTitle: string;
}
