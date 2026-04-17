import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
    AdminWordSuggestionPage,
    AdminWordSuggestionQuery
} from '../models/admin-word-suggestion.model';

@Injectable({ providedIn: 'root' })
export class AdminWordSuggestionService {
    private readonly http = inject(HttpClient);
    private readonly adminWordSuggestionApiBaseUrl = `${environment.apiBaseUrl}/api/admin/word-suggestions`;

    getSuggestions(query: AdminWordSuggestionQuery): Observable<AdminWordSuggestionPage> {
        let params = new HttpParams()
            .set('page', query.page)
            .set('pageSize', query.pageSize)
            .set('sortDirection', query.sortDirection);

        if (query.query?.trim()) {
            params = params.set('query', query.query.trim());
        }

        if (typeof query.resolved === 'boolean') {
            params = params.set('resolved', query.resolved);
        }

        return this.http.get<AdminWordSuggestionPage>(this.adminWordSuggestionApiBaseUrl, { params });
    }

    setResolved(id: number, resolved: boolean): Observable<{ id: number; resolved: boolean }> {
        return this.http.patch<{ id: number; resolved: boolean }>(
            `${this.adminWordSuggestionApiBaseUrl}/${id}/resolved`,
            { resolved }
        );
    }
}
