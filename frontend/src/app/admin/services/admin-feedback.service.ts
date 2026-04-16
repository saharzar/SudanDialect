import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AdminFeedbackPage, AdminFeedbackQuery } from '../models/admin-feedback.model';

@Injectable({ providedIn: 'root' })
export class AdminFeedbackService {
    private readonly http = inject(HttpClient);
    private readonly adminFeedbackApiBaseUrl = `${environment.apiBaseUrl}/api/admin/feedback`;

    getFeedback(query: AdminFeedbackQuery): Observable<AdminFeedbackPage> {
        let params = new HttpParams()
            .set('page', query.page)
            .set('pageSize', query.pageSize)
            .set('sortDirection', query.sortDirection);

        if (typeof query.resolved === 'boolean') {
            params = params.set('resolved', query.resolved);
        }

        if (typeof query.wordId === 'number') {
            params = params.set('wordId', query.wordId);
        }

        return this.http.get<AdminFeedbackPage>(this.adminFeedbackApiBaseUrl, { params });
    }

    setResolved(id: number, resolved: boolean): Observable<{ id: number; resolved: boolean }> {
        return this.http.patch<{ id: number; resolved: boolean }>(
            `${this.adminFeedbackApiBaseUrl}/${id}/resolved`,
            { resolved }
        );
    }
}
