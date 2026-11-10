import { DocumentOwnerType } from './Document/DocumentOwnerType.model';
// Single definition, shared by the manager, the dialog and the service
export { DocumentOwnerType } from './Document/DocumentOwnerType.model';

export interface DocumentTypeOption {
  documentTypeId: string;
  documentTypeName: string;
  documentTypeCode: number;
  requiresApproval: boolean;
  requiresReview: boolean;
  requiresName: boolean;
  requiresNumber: boolean;
  multipleAllowed: boolean;
  allowedExtensions: string[]; // empty = any file type
  maxSizeMB: number;
}

export interface DocumentItem {
  documentId: string;
  documentTypeId: string;
  documentTypeName: string;
  name?: string | null;
  number?: string | null;
  status?: string | null;
  issueDate?: string | null;
  expireDate?: string | null;
  approvedBy?: string | null;
  approvedDate?: string | null;
  version: string;
  fileName: string;
  sizeBytes: number;
}

// What the dialog hands back: everything needed to upload, nothing uploaded yet
export interface DocumentDraft {
  file: File;
  documentTypeId: string;
  documentTypeName: string;
  name: string | null;
  number: string | null;
  issueDate: string | null; // yyyy-MM-dd
  expireDate: string | null;
}
