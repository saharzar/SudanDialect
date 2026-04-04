import { Component, OnDestroy, inject } from '@angular/core';
import { Subject, of } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, switchMap, takeUntil, tap } from 'rxjs/operators';
import { SearchBarComponent } from '../../components/search-bar/search-bar.component';
import { WordCardComponent } from '../../components/word-card/word-card.component';
import { WordSearchResult } from '../../models/word-search-result';
import { WordSearchService } from '../../services/word-search.service';

@Component({
  selector: 'app-home-page',
  imports: [SearchBarComponent, WordCardComponent],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.css'
})
export class HomePageComponent implements OnDestroy {
  private readonly wordSearchService = inject(WordSearchService);
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
        switchMap((query) =>
          this.wordSearchService.search(query).pipe(
            catchError(() => {
              this.hasRequestError = true;
              return of<WordSearchResult[]>([]);
            })
          )
        ),
        takeUntil(this.destroy$)
      )
      .subscribe((results) => {
        this.results = results;
        this.isLoading = false;
        this.showDropdown = this.searchQuery.trim().length > 0;
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

  protected get shouldShowNoResults(): boolean {
    return !this.isLoading && !this.hasRequestError && this.searchQuery.trim().length > 0 && this.results.length === 0;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.searchInput$.complete();
  }
}
