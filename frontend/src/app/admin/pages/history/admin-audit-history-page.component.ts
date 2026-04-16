import { DatePipe } from '@angular/common';
import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject, catchError, finalize, of, startWith, switchMap, tap } from 'rxjs';
import { AdminAuditActionType, AdminAuditEntry, AdminAuditPage, AdminAuditQuery } from '../../models/admin-audit.model';
import { AdminWordService } from '../../services/admin-word.service';

type DiffLineKind = 'context' | 'removed' | 'added';
type DiffSegmentKind = 'context' | 'removed' | 'added';

interface DiffLine {
  kind: DiffLineKind;
  content: string;
}

interface DiffSegment {
  kind: DiffSegmentKind;
  content: string;
}

interface DiffViewLine {
  kind: DiffLineKind;
  segments: DiffSegment[];
}

@Component({
  selector: 'app-admin-audit-history-page',
  imports: [DatePipe, ReactiveFormsModule],
  templateUrl: './admin-audit-history-page.component.html',
  styleUrl: './admin-audit-history-page.component.css'
})
export class AdminAuditHistoryPageComponent {
  private readonly adminWordService = inject(AdminWordService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  private readonly loadTrigger$ = new Subject<void>();

  protected readonly actionTypeControl = new FormControl<'all' | AdminAuditActionType>('all', { nonNullable: true });
  protected readonly page = signal(1);
  protected readonly pageSize = signal(20);
  protected readonly sortDirection = signal<'asc' | 'desc'>('desc');
  protected readonly wordId = signal<number | null>(null);

  protected readonly pageResponse = signal<AdminAuditPage>(this.createEmptyPage());
  protected readonly isLoading = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly selectedEntry = signal<AdminAuditEntry | null>(null);

  protected readonly items = computed(() => this.pageResponse().items);
  protected readonly totalCount = computed(() => this.pageResponse().totalCount);
  protected readonly totalPages = computed(() => this.pageResponse().totalPages);
  protected readonly canGoPrevious = computed(() => this.page() > 1);
  protected readonly canGoNext = computed(() => this.totalPages() > 0 && this.page() < this.totalPages());
  protected readonly isDiffDialogOpen = computed(() => this.selectedEntry() !== null);
  protected readonly pageTitle = computed(() =>
    this.wordId() ? `سجل تعديلات الكلمة #${this.wordId()}` : 'سجل تعديلات الكلمات'
  );
  protected readonly headwordDiffRows = computed(() => {
    const entry = this.selectedEntry();
    if (!entry) {
      return [];
    }

    return this.buildDiffRows(entry.oldHeadword, entry.newHeadword);
  });
  protected readonly definitionDiffRows = computed(() => {
    const entry = this.selectedEntry();
    if (!entry) {
      return [];
    }

    return this.buildDiffRows(entry.oldDefinition, entry.newDefinition);
  });

  constructor() {
    this.route.paramMap
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((params) => {
        const idParam = params.get('id');

        if (!idParam) {
          this.wordId.set(null);
          this.errorMessage.set('');
          this.page.set(1);
          this.loadTrigger$.next();
          return;
        }

        const parsedId = Number(idParam);
        if (!Number.isInteger(parsedId) || parsedId <= 0) {
          this.wordId.set(null);
          this.errorMessage.set('معرّف الكلمة غير صالح.');
          this.pageResponse.set(this.createEmptyPage());
          return;
        }

        this.wordId.set(parsedId);
        this.errorMessage.set('');
        this.page.set(1);
        this.loadTrigger$.next();
      });

    this.actionTypeControl.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
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
          this.fetchAuditPage().pipe(
            catchError(() => {
              this.errorMessage.set('تعذر تحميل سجل التعديلات.');
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

  protected nextPage(): void {
    if (!this.canGoNext()) {
      return;
    }

    this.page.update((current) => current + 1);
    this.loadTrigger$.next();
  }

  protected previousPage(): void {
    if (!this.canGoPrevious()) {
      return;
    }

    this.page.update((current) => current - 1);
    this.loadTrigger$.next();
  }

  protected refresh(): void {
    this.loadTrigger$.next();
  }

  protected openGlobalHistory(): void {
    void this.router.navigate(['/admin/history']);
  }

  protected openWordHistory(wordId: number): void {
    void this.router.navigate(['/admin/words', wordId, 'history']);
  }

  protected openWordEditor(wordId: number): void {
    void this.router.navigate(['/admin/words', wordId, 'edit']);
  }

  protected openChangesDialog(item: AdminAuditEntry): void {
    this.selectedEntry.set(item);
  }

  protected closeChangesDialog(): void {
    this.selectedEntry.set(null);
  }

  protected preventDialogClose(event: MouseEvent): void {
    event.stopPropagation();
  }

  protected actionTypeLabel(actionType: AdminAuditActionType): string {
    if (actionType === 'deactivate') {
      return 'تعطيل';
    }

    if (actionType === 'reactivate') {
      return 'إعادة تفعيل';
    }

    return 'تعديل';
  }

  protected activeStatusLabel(item: AdminAuditEntry): string {
    if (item.oldIsActive === item.newIsActive) {
      return item.newIsActive ? 'نشطة ← نشطة' : 'غير نشطة ← غير نشطة';
    }

    return item.newIsActive ? 'غير نشطة ← نشطة' : 'نشطة ← غير نشطة';
  }

  protected diffLinePrefix(kind: DiffLineKind): string {
    if (kind === 'added') {
      return '+';
    }

    if (kind === 'removed') {
      return '-';
    }

    return ' ';
  }

  private fetchAuditPage() {
    const query = this.buildQuery();
    const currentWordId = this.wordId();

    if (currentWordId) {
      return this.adminWordService.getAuditHistoryByWordId(currentWordId, query);
    }

    return this.adminWordService.getAuditHistory(query);
  }

  private buildQuery(): AdminAuditQuery {
    const selectedActionType = this.actionTypeControl.value;

    return {
      page: this.page(),
      pageSize: this.pageSize(),
      actionType: selectedActionType === 'all' ? undefined : selectedActionType,
      sortDirection: this.sortDirection()
    };
  }

  private createEmptyPage(): AdminAuditPage {
    return {
      items: [],
      page: this.page(),
      pageSize: this.pageSize(),
      totalCount: 0,
      totalPages: 0
    };
  }

  private buildDiffLines(oldValue: string, newValue: string): DiffLine[] {
    const oldLines = this.splitLines(oldValue);
    const newLines = this.splitLines(newValue);

    const n = oldLines.length;
    const m = newLines.length;
    const lcs: number[][] = Array.from({ length: n + 1 }, () => Array<number>(m + 1).fill(0));

    for (let i = n - 1; i >= 0; i -= 1) {
      for (let j = m - 1; j >= 0; j -= 1) {
        if (oldLines[i] === newLines[j]) {
          lcs[i][j] = lcs[i + 1][j + 1] + 1;
        } else {
          lcs[i][j] = Math.max(lcs[i + 1][j], lcs[i][j + 1]);
        }
      }
    }

    const diff: DiffLine[] = [];
    let i = 0;
    let j = 0;

    while (i < n && j < m) {
      if (oldLines[i] === newLines[j]) {
        diff.push({ kind: 'context', content: oldLines[i] });
        i += 1;
        j += 1;
        continue;
      }

      if (lcs[i + 1][j] >= lcs[i][j + 1]) {
        diff.push({ kind: 'removed', content: oldLines[i] });
        i += 1;
        continue;
      }

      diff.push({ kind: 'added', content: newLines[j] });
      j += 1;
    }

    while (i < n) {
      diff.push({ kind: 'removed', content: oldLines[i] });
      i += 1;
    }

    while (j < m) {
      diff.push({ kind: 'added', content: newLines[j] });
      j += 1;
    }

    return diff;
  }

  private buildDiffRows(oldValue: string, newValue: string): DiffViewLine[] {
    const lines = this.buildDiffLines(oldValue, newValue);
    const rows: DiffViewLine[] = [];

    for (let i = 0; i < lines.length; i += 1) {
      const current = lines[i];
      const next = i + 1 < lines.length ? lines[i + 1] : null;

      if (current.kind === 'removed' && next?.kind === 'added') {
        const paired = this.buildCharacterSegments(current.content, next.content);
        rows.push({ kind: 'removed', segments: this.ensureRenderableSegments(paired.oldSegments) });
        rows.push({ kind: 'added', segments: this.ensureRenderableSegments(paired.newSegments) });
        i += 1;
        continue;
      }

      rows.push({
        kind: current.kind,
        segments: this.ensureRenderableSegments([{ kind: current.kind, content: current.content }])
      });
    }

    return rows;
  }

  private buildCharacterSegments(oldValue: string, newValue: string): { oldSegments: DiffSegment[]; newSegments: DiffSegment[] } {
    const oldChars = Array.from(oldValue);
    const newChars = Array.from(newValue);

    let prefixLength = 0;
    while (prefixLength < oldChars.length && prefixLength < newChars.length && oldChars[prefixLength] === newChars[prefixLength]) {
      prefixLength += 1;
    }

    let suffixLength = 0;
    while (
      suffixLength < oldChars.length - prefixLength &&
      suffixLength < newChars.length - prefixLength &&
      oldChars[oldChars.length - 1 - suffixLength] === newChars[newChars.length - 1 - suffixLength]
    ) {
      suffixLength += 1;
    }

    const prefix = oldChars.slice(0, prefixLength).join('');
    const suffix = suffixLength > 0 ? oldChars.slice(oldChars.length - suffixLength).join('') : '';
    const oldMiddle = oldChars.slice(prefixLength, oldChars.length - suffixLength);
    const newMiddle = newChars.slice(prefixLength, newChars.length - suffixLength);

    const oldSegments: DiffSegment[] = [];
    const newSegments: DiffSegment[] = [];

    if (prefix) {
      this.appendSegment(oldSegments, 'context', prefix);
      this.appendSegment(newSegments, 'context', prefix);
    }

    if (oldMiddle.length * newMiddle.length <= 360000) {
      this.appendCharacterLcsSegments(oldSegments, newSegments, oldMiddle, newMiddle);
    } else {
      if (oldMiddle.length > 0) {
        this.appendSegment(oldSegments, 'removed', oldMiddle.join(''));
      }

      if (newMiddle.length > 0) {
        this.appendSegment(newSegments, 'added', newMiddle.join(''));
      }
    }

    if (suffix) {
      this.appendSegment(oldSegments, 'context', suffix);
      this.appendSegment(newSegments, 'context', suffix);
    }

    return { oldSegments, newSegments };
  }

  private appendCharacterLcsSegments(
    oldSegments: DiffSegment[],
    newSegments: DiffSegment[],
    oldChars: string[],
    newChars: string[]
  ): void {
    const n = oldChars.length;
    const m = newChars.length;
    const lcs: number[][] = Array.from({ length: n + 1 }, () => Array<number>(m + 1).fill(0));

    for (let i = n - 1; i >= 0; i -= 1) {
      for (let j = m - 1; j >= 0; j -= 1) {
        if (oldChars[i] === newChars[j]) {
          lcs[i][j] = lcs[i + 1][j + 1] + 1;
        } else {
          lcs[i][j] = Math.max(lcs[i + 1][j], lcs[i][j + 1]);
        }
      }
    }

    let i = 0;
    let j = 0;

    while (i < n && j < m) {
      if (oldChars[i] === newChars[j]) {
        this.appendSegment(oldSegments, 'context', oldChars[i]);
        this.appendSegment(newSegments, 'context', newChars[j]);
        i += 1;
        j += 1;
        continue;
      }

      if (lcs[i + 1][j] >= lcs[i][j + 1]) {
        this.appendSegment(oldSegments, 'removed', oldChars[i]);
        i += 1;
      } else {
        this.appendSegment(newSegments, 'added', newChars[j]);
        j += 1;
      }
    }

    while (i < n) {
      this.appendSegment(oldSegments, 'removed', oldChars[i]);
      i += 1;
    }

    while (j < m) {
      this.appendSegment(newSegments, 'added', newChars[j]);
      j += 1;
    }
  }

  private appendSegment(segments: DiffSegment[], kind: DiffSegmentKind, content: string): void {
    if (!content) {
      return;
    }

    const lastIndex = segments.length - 1;
    if (lastIndex >= 0 && segments[lastIndex].kind === kind) {
      segments[lastIndex].content += content;
      return;
    }

    segments.push({ kind, content });
  }

  private ensureRenderableSegments(segments: DiffSegment[]): DiffSegment[] {
    if (segments.length > 0) {
      return segments;
    }

    return [{ kind: 'context', content: '' }];
  }

  private splitLines(value: string): string[] {
    if (!value) {
      return [''];
    }

    return value.replace(/\r\n/g, '\n').split('\n');
  }
}
