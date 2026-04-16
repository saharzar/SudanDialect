export interface AdminLoginRequest {
  username: string;
  password: string;
}

export type AdminRole = 'admin' | 'moderator';

export interface AdminLoginResponse {
  expiresAtUtc: string;
  username: string;
  roles: string[];
}

export interface AdminAuthSession {
  expiresAtUtc: string;
  username: string;
  roles: AdminRole[];
}
