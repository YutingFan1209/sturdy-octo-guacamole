import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { AuthSession, LoginRequest, LoginResponse, RegisterRequest, UserInfo } from './auth.models';
import { apiUrl } from '../api-url';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly storageKey = 'movieshop_auth';
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly session = signal<AuthSession | null>(this.loadSession());

  readonly currentUser = computed(() => this.validSession()?.user ?? null);
  readonly isAuthenticated = computed(() => this.validSession() !== null);

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(apiUrl('/api/accounts/login'), request).pipe(
      tap(response => this.saveSession({
        token: response.token,
        expiresAtUtc: response.expiresAtUtc,
        user: {
          id: response.id,
          email: response.email,
          firstName: response.firstName,
          lastName: response.lastName,
          dateOfBirth: response.dateOfBirth
        }
      }))
    );
  }

  register(request: RegisterRequest): Observable<UserInfo> {
    return this.http.post<UserInfo>(apiUrl('/api/accounts/register'), request);
  }

  getToken(): string | null {
    return this.validSession()?.token ?? null;
  }

  logout(): void {
    this.storage()?.removeItem(this.storageKey);
    this.session.set(null);
    void this.router.navigate(['/']);
  }

  private validSession(): AuthSession | null {
    const session = this.session();
    if (session && new Date(session.expiresAtUtc).getTime() > Date.now()) return session;
    if (session) {
      this.storage()?.removeItem(this.storageKey);
    }
    return null;
  }

  private saveSession(session: AuthSession): void {
    this.storage()?.setItem(this.storageKey, JSON.stringify(session));
    this.session.set(session);
  }

  private loadSession(): AuthSession | null {
    try {
      const storage = this.storage();
      if (!storage) return null;
      const value = storage.getItem(this.storageKey);
      if (!value) return null;
      const session = JSON.parse(value) as AuthSession;
      if (!session.token || new Date(session.expiresAtUtc).getTime() <= Date.now()) {
        storage.removeItem(this.storageKey);
        return null;
      }
      return session;
    } catch {
      this.storage()?.removeItem(this.storageKey);
      return null;
    }
  }

  private storage(): Storage | null {
    return typeof localStorage === 'undefined' ? null : localStorage;
  }
}
