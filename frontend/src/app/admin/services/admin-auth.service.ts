import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, catchError, map, of, switchMap, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AdminAuthSession, AdminLoginRequest, AdminLoginResponse, AdminRole } from '../models/admin-auth.model';

@Injectable({ providedIn: 'root' })
export class AdminAuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly authApiBaseUrl = `${environment.apiBaseUrl}/api/admin/auth`;
  private readonly sessionState = signal<AdminAuthSession | null>(null);

  readonly session = this.sessionState.asReadonly();
  readonly isAuthenticated = computed(() => {
    const session = this.sessionState();
    return !!session && !this.isExpired(session.expiresAtUtc);
  });
  readonly roles = computed(() => this.sessionState()?.roles ?? []);
  readonly primaryRole = computed<AdminRole | null>(() => this.roles()[0] ?? null);

  login(payload: AdminLoginRequest): Observable<AdminAuthSession> {
    return this.http
      .post<AdminLoginResponse>(`${this.authApiBaseUrl}/login`, payload, { withCredentials: true })
      .pipe(
        map((response) => this.toSession(response)),
        tap((session) => this.persistSession(session))
      );
  }

  refreshSession(): Observable<AdminAuthSession> {
    return this.http
      .post<AdminAuthSession>(`${this.authApiBaseUrl}/refresh`, {}, { withCredentials: true })
      .pipe(
        map((session) => this.toSession(session)),
        tap((session) => this.persistSession(session))
      );
  }

  ensureAuthenticated(): Observable<boolean> {
    const session = this.sessionState();
    if (session && !this.isExpired(session.expiresAtUtc)) {
      return of(true);
    }

    return this.fetchSession().pipe(
      map(() => true),
      catchError(() =>
        this.refreshSession().pipe(
          map(() => true),
          catchError(() => {
            this.clearSession();
            return of(false);
          })
        )
      )
    );
  }

  logout(): void {
    this.http
      .post<void>(`${this.authApiBaseUrl}/logout`, {}, { withCredentials: true })
      .pipe(catchError(() => of(void 0)))
      .subscribe(() => {
        this.clearSession();
        void this.router.navigate(['/admin/login']);
      });
  }

  clearLocalSession(): void {
    this.clearSession();
  }

  private persistSession(session: AdminAuthSession): void {
    this.sessionState.set(session);
  }

  private clearSession(): void {
    this.sessionState.set(null);
  }

  private fetchSession(): Observable<AdminAuthSession> {
    return this.http
      .get<AdminAuthSession>(`${this.authApiBaseUrl}/session`, { withCredentials: true })
      .pipe(
        map((session) => this.toSession(session)),
        tap((session) => this.persistSession(session)),
        switchMap((session) => {
          if (this.isExpired(session.expiresAtUtc)) {
            return this.refreshSession();
          }

          return of(session);
        })
      );
  }

  private toSession(source: AdminAuthSession | AdminLoginResponse): AdminAuthSession {
    return {
      expiresAtUtc: source.expiresAtUtc,
      username: source.username,
      roles: this.normalizeRoles(source.roles)
    };
  }

  private normalizeRoles(roles: readonly string[] | null | undefined): AdminRole[] {
    if (!roles || roles.length === 0) {
      return [];
    }

    return roles
      .map((role) => role.trim().toLowerCase())
      .filter((role): role is AdminRole => role === 'admin' || role === 'moderator');
  }

  private isExpired(expiresAtUtc: string): boolean {
    const expiresAt = Date.parse(expiresAtUtc);
    if (!Number.isFinite(expiresAt)) {
      return true;
    }

    return expiresAt <= Date.now();
  }
}
