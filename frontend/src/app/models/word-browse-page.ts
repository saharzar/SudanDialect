import { WordSummary } from './word-summary';

export type WordBrowsePage = {
    items: WordSummary[];
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
};
