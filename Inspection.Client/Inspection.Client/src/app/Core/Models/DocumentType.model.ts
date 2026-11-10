import { CONTACT_TYPES, ContactTypes } from "./ContactTypes.enum";

export interface DocumentType
{
     documentTypeId :string,
  documentTypeName :string,
  documentTypeCode :number,
  requiresApproval :boolean,
  requiresReview :boolean,
  requiresName :boolean,
  requiresNumber :boolean,
  multipleAllowed :boolean,
   isActive :boolean,
  allowedExtensions :string,
  maxSizeMB :string
}

// GET api/document-types/admin, GET .../admin/{id}
export interface DocumentTypeAdmin {
  documentTypeId: string;
  documentTypeName: string;
  documentTypeCode: number;
  requiresApproval: boolean;
  requiresReview: boolean;
  requiresName: boolean;
  requiresNumber: boolean;
  multipleAllowed: boolean;
  isActive: boolean;
  allowedExtensions: string;   // raw pattern as stored, e.g. "*", ".pdf,.docx", "pdf;docx"
  maxSizeMB: number;
  visibleToContactTypes: ContactTypes[];
  documentCount: number;       // how many documents currently use this type (server-computed)
}

// POST / PUT body
export interface DocumentTypeSave {
  documentTypeName: string;
  documentTypeCode: number;
  requiresApproval: boolean;
  requiresReview: boolean;
  requiresName: boolean;
  requiresNumber: boolean;
  multipleAllowed: boolean;
  isActive: boolean;
  allowedExtensions: string;
  maxSizeMB: number;
  visibleToContactTypes: ContactTypes[];
}

export function contactTypeLabel(value: ContactTypes): string {
  return CONTACT_TYPES.find((c) => c.value === value)?.label ?? String(value);
}
