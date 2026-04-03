import { CommonModule } from '@angular/common';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnDestroy, inject } from '@angular/core';
import { Subject, of } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, switchMap, takeUntil, tap } from 'rxjs/operators';
import { environment } from '../environments/environment';

type WordSearchResult = {
  id: number;
  headword: string;
  definition: string;
  similarityScore: number;
};

@Component({
  selector: 'app-root',
  imports: [CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnDestroy {
  private readonly http = inject(HttpClient);
  private readonly searchInput$ = new Subject<string>();
  private readonly destroy$ = new Subject<void>();

  protected searchQuery = '';
  protected results: WordSearchResult[] = [];
  protected selectedWord: WordSearchResult | null = null;
  protected isLoading = false;
  protected showDropdown = false;
  protected hasRequestError = false;

  constructor() {
    this.searchInput$
      .pipe(
        debounceTime(150),
        distinctUntilChanged(),
        tap(() => {
          this.isLoading = true;
          this.hasRequestError = false;
        }),
        switchMap((query) => {
          const params = new HttpParams().set('query', query);
          return this.http
            .get<WordSearchResult[]>(`${environment.apiBaseUrl}/api/words/search`, { params })
            .pipe(
              catchError(() => {
                this.hasRequestError = true;
                return of<WordSearchResult[]>([]);
              })
            );
        }),
        takeUntil(this.destroy$)
      )
      .subscribe((results) => {
        this.results = this.sortResultsBySimilarity(results);
        this.isLoading = false;
        this.showDropdown = this.searchQuery.trim().length > 0;
      });
  }

  private sortResultsBySimilarity(results: WordSearchResult[]): WordSearchResult[] {
    return [...results].sort((first, second) => {
      const scoreDifference = second.similarityScore - first.similarityScore;
      if (scoreDifference !== 0) {
        return scoreDifference;
      }

      return first.headword.localeCompare(second.headword, 'ar');
    });
  }

  protected onSearchInput(value: string): void {
    this.searchQuery = value;
    this.selectedWord = null;

    const trimmedQuery = value.trim();
    if (!trimmedQuery) {
      this.results = [];
      this.isLoading = false;
      this.hasRequestError = false;
      this.showDropdown = false;
      return;
    }

    this.showDropdown = true;
    this.searchInput$.next(trimmedQuery);
  }

  protected selectWord(word: WordSearchResult): void {
    this.selectedWord = word;
    this.showDropdown = false;
  }

  protected trackByWordId(_index: number, word: WordSearchResult): number {
    return word.id;
  }

  protected get shouldShowNoResults(): boolean {
    return !this.isLoading && !this.hasRequestError && this.searchQuery.trim().length > 0 && this.results.length === 0;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.searchInput$.complete();
  }
}
