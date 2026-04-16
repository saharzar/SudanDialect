export interface AdminFeedbackItem {
    id: number;
    wordId: number;
    wordHeadword: string;
    feedbackText: string;
    timestamp: string;
    resolved: boolean;
}

export interface AdminFeedbackPage {
    items: AdminFeedbackItem[];
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
}

export interface AdminFeedbackQuery {
    page: number;
    pageSize: number;
    resolved?: boolean;
    wordId?: number;
    sortDirection: 'asc' | 'desc';
}
