export interface AdminWordSuggestionItem {
    id: number;
    headword: string;
    definition: string;
    email?: string | null;
    resolved: boolean;
    timestamp: string;
}

export interface AdminWordSuggestionPage {
    items: AdminWordSuggestionItem[];
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
}

export interface AdminWordSuggestionQuery {
    page: number;
    pageSize: number;
    query?: string;
    resolved?: boolean;
    sortDirection: 'asc' | 'desc';
}
