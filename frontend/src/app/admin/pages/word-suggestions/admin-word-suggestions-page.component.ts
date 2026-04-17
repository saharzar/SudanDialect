import { DatePipe } from '@angular/common';
import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Subject, catchError, debounceTime, distinctUntilChanged, finalize, of, startWith, switchMap, tap } from 'rxjs';
import {
    AdminWordSuggestionPage,
    AdminWordSuggestionQuery
} from '../../models/admin-word-suggestion.model';
import { AdminToastService } from '../../services/admin-toast.service';
import { AdminWordSuggestionService } from '../../services/admin-word-suggestion.service';

@Component({
    selector: 'app-admin-word-suggestions-page',
    imports: [DatePipe, ReactiveFormsModule],
    templateUrl: './admin-word-suggestions-page.component.html',
    styleUrl: './admin-word-suggestions-page.component.css'
})
export class AdminWordSuggestionsPageComponent {
    private readonly adminWordSuggestionService = inject(AdminWordSuggestionService);
    private readonly toastService = inject(AdminToastService);
    private readonly destroyRef = inject(DestroyRef);

    private readonly loadTrigger$ = new Subject<void>();

    protected readonly queryControl = new FormControl('', { nonNullable: true });
    protected readonly page = signal(1);
    protected readonly pageSize = signal(20);
    protected readonly sortDirection = signal<'asc' | 'desc'>('desc');
    protected readonly resolvedFilter = signal<'all' | 'resolved' | 'unresolved'>('unresolved');

    protected readonly pageResponse = signal<AdminWordSuggestionPage>(this.createEmptyPage());
    protected readonly isLoading = signal(false);
    protected readonly errorMessage = signal('');

    protected readonly suggestionItems = computed(() => this.pageResponse().items);
    protected readonly totalCount = computed(() => this.pageResponse().totalCount);
    protected readonly totalPages = computed(() => this.pageResponse().totalPages);
    protected readonly canGoPrevious = computed(() => this.page() > 1);
    protected readonly canGoNext = computed(() => this.totalPages() > 0 && this.page() < this.totalPages());

    constructor() {
        this.queryControl.valueChanges
            .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
            .subscribe(() => {
                this.page.set(1);
                this.loadTrigger$.next();
            });

        this.loadTrigger$
            .pipe(
                startWith(void 0),
                tap(() => {
                    this.isLoading.set(true);
                    this.errorMessage.set('');
                }),
                switchMap(() =>
                    this.adminWordSuggestionService.getSuggestions(this.buildQuery()).pipe(
                        catchError(() => {
                            this.errorMessage.set('تعذر تحميل اقتراحات الكلمات.');
                            return of(this.createEmptyPage());
                        }),
                        finalize(() => this.isLoading.set(false))
                    )
                ),
                takeUntilDestroyed(this.destroyRef)
            )
            .subscribe((response) => {
                this.pageResponse.set(response);
                this.page.set(response.page);
            });
    }

    protected setResolvedFilter(filter: 'all' | 'resolved' | 'unresolved'): void {
        this.resolvedFilter.set(filter);
        this.page.set(1);
        this.loadTrigger$.next();
    }

    protected toggleSortDirection(): void {
        this.sortDirection.set(this.sortDirection() === 'asc' ? 'desc' : 'asc');
        this.page.set(1);
        this.loadTrigger$.next();
    }

    protected previousPage(): void {
        if (!this.canGoPrevious()) {
            return;
        }

        this.page.update((current) => current - 1);
        this.loadTrigger$.next();
    }

    protected nextPage(): void {
        if (!this.canGoNext()) {
            return;
        }

        this.page.update((current) => current + 1);
        this.loadTrigger$.next();
    }

    protected refresh(): void {
        this.loadTrigger$.next();
    }

    protected toggleResolved(id: number, resolved: boolean): void {
        this.adminWordSuggestionService
            .setResolved(id, !resolved)
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({
                next: () => {
                    this.toastService.showSuccess(!resolved ? 'تم تمييز الاقتراح كمراجع.' : 'تمت إعادة فتح الاقتراح.');
                    this.loadTrigger$.next();
                },
                error: () => {
                    this.toastService.showError('تعذر تحديث حالة الاقتراح.');
                }
            });
    }

    private buildQuery(): AdminWordSuggestionQuery {
        const queryText = this.queryControl.value.trim();

        return {
            page: this.page(),
            pageSize: this.pageSize(),
            query: queryText || undefined,
            resolved: this.resolvedFilter() === 'all'
                ? undefined
                : this.resolvedFilter() === 'resolved',
            sortDirection: this.sortDirection()
        };
    }

    private createEmptyPage(): AdminWordSuggestionPage {
        return {
            items: [],
            page: this.page(),
            pageSize: this.pageSize(),
            totalCount: 0,
            totalPages: 0
        };
    }
}
