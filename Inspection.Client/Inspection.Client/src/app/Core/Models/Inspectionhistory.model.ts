export interface CirculationHistory {
  id: string;
  parentId: string | null;
  performerName: string | null;
  performerContactType: number | null;
  sequenceNo: number | null;
  actionType: number | null;
  qcActionType: number | null;
  previousActionType: number | null;
  performedActionDate: string | null;
  performedActionDateGregorian: string | null;   // ISO string; year 1 (0001-01-01) means "no action yet"
  comment: string | null;
  creatorComment: string | null;
  childCirculations: CirculationHistory[];
}

export interface RevisionHistory {
  revisionId: string;
  versionNumber: number | null;
  revisionNumber: number | null;
  submittedDate: string | null;
  submittedByName: string | null;
  comment: string | null;
  status: string | null;
  isFinal: boolean | null;
  circulations: CirculationHistory[];
}

// Mirrors onAction()/CirculationActionType() in VnewinspectionComponent: 1 Approve, 2 Reject, 3 Backward.
export function actionTypeLabel(circulation: CirculationHistory): string {
  const noActionYet =
    !circulation.performedActionDateGregorian ||
    new Date(circulation.performedActionDateGregorian).getFullYear() === 1;
  if (noActionYet) return 'Pending';

  switch (circulation.actionType) {
    case 1: return 'Approved';
    case 2: return 'Rejected';
    case 3: return 'Backwarded';
    default: return 'Action recorded';
  }
}

export function actionTypeClass(circulation: CirculationHistory): string {
  const noActionYet =
    !circulation.performedActionDateGregorian ||
    new Date(circulation.performedActionDateGregorian).getFullYear() === 1;
  if (noActionYet) return 'history-badge history-pending';

  switch (circulation.actionType) {
    case 1: return 'history-badge history-approved';
    case 2: return 'history-badge history-rejected';
    case 3: return 'history-badge history-backwarded';
    default: return 'history-badge';
  }
}
