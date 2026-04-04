import { Component, EventEmitter, Input, Output } from '@angular/core';
import { WordSearchResult } from '../../models/word-search-result';

@Component({
  selector: 'app-search-bar',
  templateUrl: './search-bar.component.html',
  styleUrl: './search-bar.component.css'
})
export class SearchBarComponent {
  @Input() query = '';
  @Input() results: WordSearchResult[] = [];
  @Input() isLoading = false;
  @Input() showDropdown = false;
  @Input() hasRequestError = false;
  @Input() shouldShowNoResults = false;

  @Output() queryChanged = new EventEmitter<string>();
  @Output() wordSelected = new EventEmitter<WordSearchResult>();

  protected onInput(value: string): void {
    this.queryChanged.emit(value);
  }

  protected selectWord(word: WordSearchResult): void {
    this.wordSelected.emit(word);
  }
}
