import { AdminRole } from './admin-auth.model';

export interface AdminManagedUser {
    id: string;
    username: string;
    roles: AdminRole[];
}

export interface AdminCreateUserRequest {
    username: string;
    password: string;
}

export interface AdminUpdateUserRequest {
    username: string;
    password: string;
}
