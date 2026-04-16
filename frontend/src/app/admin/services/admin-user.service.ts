import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AdminCreateUserRequest, AdminManagedUser, AdminUpdateUserRequest } from '../models/admin-user.model';

@Injectable({ providedIn: 'root' })
export class AdminUserService {
    private readonly http = inject(HttpClient);
    private readonly adminUsersApiBaseUrl = `${environment.apiBaseUrl}/api/admin/users`;

    getUsers(): Observable<AdminManagedUser[]> {
        return this.http.get<AdminManagedUser[]>(this.adminUsersApiBaseUrl);
    }

    createUser(payload: AdminCreateUserRequest): Observable<AdminManagedUser> {
        return this.http.post<AdminManagedUser>(this.adminUsersApiBaseUrl, payload);
    }

    updateUser(id: string, payload: AdminUpdateUserRequest): Observable<AdminManagedUser> {
        return this.http.put<AdminManagedUser>(`${this.adminUsersApiBaseUrl}/${id}`, payload);
    }

    deleteUser(id: string): Observable<{ id: string; deleted: boolean }> {
        return this.http.delete<{ id: string; deleted: boolean }>(`${this.adminUsersApiBaseUrl}/${id}`);
    }
}
