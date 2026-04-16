export type AdminAuditActionType = 'update' | 'deactivate' | 'reactivate';

export interface AdminAuditEntry {
  id: number;
  wordId: number;
  wordHeadword: string;
  adminUserId: string;
  adminDisplayName: string;
  editedAt: string;
  actionType: AdminAuditActionType;
  oldHeadword: string;
  newHeadword: string;
  oldDefinition: string;
  newDefinition: string;
  oldIsActive: boolean;
  newIsActive: boolean;
  oldNormalizedHeadword: string;
  newNormalizedHeadword: string;
  oldNormalizedDefinition: string;
  newNormalizedDefinition: string;
  clientIp?: string | null;
  userAgent?: string | null;
}

export interface AdminAuditPage {
  items: AdminAuditEntry[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface AdminAuditQuery {
  page: number;
  pageSize: number;
  actionType?: AdminAuditActionType;
  sortDirection: 'asc' | 'desc';
}
