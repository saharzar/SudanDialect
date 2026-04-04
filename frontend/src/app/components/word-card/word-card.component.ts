import { Component, Input } from '@angular/core';
import { WordSearchResult } from '../../models/word-search-result';

@Component({
  selector: 'app-word-card',
  templateUrl: './word-card.component.html',
  styleUrl: './word-card.component.css'
})
export class WordCardComponent {
  @Input({ required: true }) word!: WordSearchResult;
}
