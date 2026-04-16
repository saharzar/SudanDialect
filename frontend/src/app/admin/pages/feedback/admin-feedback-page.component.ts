import { DatePipe } from '@angular/common';
import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject, catchError, debounceTime, distinctUntilChanged, finalize, of, startWith, switchMap, tap } from 'rxjs';
import { AdminFeedbackPage, AdminFeedbackQuery } from '../../models/admin-feedback.model';
import { AdminFeedbackService } from '../../services/admin-feedback.service';
import { AdminToastService } from '../../services/admin-toast.service';

@Component({
    selector: 'app-admin-feedback-page',
    imports: [DatePipe, ReactiveFormsModule],
    templateUrl: './admin-feedback-page.component.html',
    styleUrl: './admin-feedback-page.component.css'
})
export class AdminFeedbackPageComponent {
    private readonly adminFeedbackService = inject(AdminFeedbackService);
    private readonly toastService = inject(AdminToastService);
    private readonly router = inject(Router);
    private readonly destroyRef = inject(DestroyRef);

    private readonly loadTrigger$ = new Subject<void>();

    protected readonly wordIdControl = new FormControl('', { nonNullable: true });
    protected readonly page = signal(1);
    protected readonly pageSize = signal(20);
    protected readonly sortDirection = signal<'asc' | 'desc'>('desc');
    protected readonly resolvedFilter = signal<'all' | 'resolved' | 'unresolved'>('all');

    protected readonly pageResponse = signal<AdminFeedbackPage>(this.createEmptyPage());
    protected readonly isLoading = signal(false);
    protected readonly errorMessage = signal('');

    protected readonly feedbackItems = computed(() => this.pageResponse().items);
    protected readonly totalCount = computed(() => this.pageResponse().totalCount);
    protected readonly totalPages = computed(() => this.pageResponse().totalPages);
    protected readonly canGoPrevious = computed(() => this.page() > 1);
    protected readonly canGoNext = computed(() => this.totalPages() > 0 && this.page() < this.totalPages());

    constructor() {
        this.wordIdControl.valueChanges
            .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
            .subscribe((value) => {
                const normalized = value.replace(/[^0-9]/g, '');
                if (normalized !== value) {
                    this.wordIdControl.setValue(normalized, { emitEvent: false });
                }

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
                    this.adminFeedbackService.getFeedback(this.buildQuery()).pipe(
                        catchError(() => {
                            this.errorMessage.set('تعذر تحميل البلاغات.');
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

    protected toggleSortDirection(): void {
        this.sortDirection.set(this.sortDirection() === 'asc' ? 'desc' : 'asc');
        this.page.set(1);
        this.loadTrigger$.next();
    }

    protected setResolvedFilter(filter: 'all' | 'resolved' | 'unresolved'): void {
        this.resolvedFilter.set(filter);
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

    protected openWordEditor(wordId: number): void {
        void this.router.navigate(['/admin/words', wordId, 'edit']);
    }

    protected toggleResolved(feedbackId: number, resolved: boolean): void {
        this.adminFeedbackService
            .setResolved(feedbackId, !resolved)
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({
                next: () => {
                    this.toastService.showSuccess(!resolved ? 'تم تمييز البلاغ كمحلول.' : 'تم إعادة فتح البلاغ.');
                    this.loadTrigger$.next();
                },
                error: () => {
                    this.toastService.showError('تعذر تحديث حالة البلاغ.');
                }
            });
    }

    private buildQuery(): AdminFeedbackQuery {
        const wordIdRaw = this.wordIdControl.value.trim();
        const parsedWordId = wordIdRaw ? Number(wordIdRaw) : undefined;

        return {
            page: this.page(),
            pageSize: this.pageSize(),
            resolved: this.resolvedFilter() === 'all'
                ? undefined
                : this.resolvedFilter() === 'resolved',
            wordId: typeof parsedWordId === 'number' && Number.isInteger(parsedWordId) && parsedWordId > 0
                ? parsedWordId
                : undefined,
            sortDirection: this.sortDirection()
        };
    }

    private createEmptyPage(): AdminFeedbackPage {
        return {
            items: [],
            page: this.page(),
            pageSize: this.pageSize(),
            totalCount: 0,
            totalPages: 0
        };
    }
}
