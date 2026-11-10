export type SkillLevel = 'Internship' | 'Experienced' | 'Professional';

export interface ContactOption {
  contactId: string;
  name: string;
  contactTypeCode: number; // decides which document types are available
}

export interface SkillOption {
  skillId: string;
  code: string; // VT, RT, UT, MT
  name: string; // Visual Testing
}

export interface InspectorSkill {
  skillId: string;
  code?: string;
  name?: string;
  level: SkillLevel;
}

export interface Inspector {
  inspectorId?: string;
  inspectionCompanyContactId: string;
  inspectionCompanyContactName?: string;
  isActive: boolean;
  name?: string | null;
  firstName?: string | null;
  lastName?: string | null;
  description?: string | null;
  position?: string | null;
  phone?: string | null;
  email?: string | null;
  qcApprovalState: number; // 0 = Rejected, 1 = Approved, 2 = NoAction
  versionNumber: number;
  createdDate: string;
  expireDate: string;
  skills: InspectorSkill[];
}

export interface InspectorReview {
  comment?: string;

}


// Read-only display only: the company can never set this. See /qc-review for the Approve/Reject actions.
export const QC_STATES: { value: number; label: string }[] = [
  { value: 0, label: 'Rejected' },
  { value: 1, label: 'Approved' },
  { value: 2, label: 'Pending review' },
];

export const SKILL_LEVELS: { value: SkillLevel; label: string }[] = [
  { value: 'Internship', label: 'Internship' },
  { value: 'Experienced', label: 'Experienced' },
  { value: 'Professional', label: 'Professional' },
];


// What the inspection company's form sends. No qcApprovalState here: QC is the only one who may set it,
// through InspectorQcService.approve / .reject (see qc-review/inspector-qc.service.ts).
export interface InspectorSave {
  inspectorId?: string;
  inspectionCompanyContactId: string;
  isActive: boolean;
  name?: string | null;
  firstName?: string | null;
  lastName?: string | null;
  description?: string | null;
  position?: string | null;
  phone?: string | null;
  email?: string | null;
  expireDate: string;
  skills: InspectorSkill[];
}
