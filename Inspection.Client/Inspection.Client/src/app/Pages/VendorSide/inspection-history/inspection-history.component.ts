import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CirculationModalComponent } from '../circulation-modal/circulation-modal.component'; // TODO: adjust path to your real file


import { InspectionHistoryService } from '../../../Core/Services/Inspectionhistory.service';
import { actionTypeClass, actionTypeLabel, CirculationHistory, RevisionHistory } from '../../../Core/Models/Inspectionhistory.model';

@Component({
  selector: 'app-inspection-history',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule],
  templateUrl: './inspection-history.component.html',
  styleUrl: './inspection-history.component.css',
})
export class InspectionHistoryComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private service = inject(InspectionHistoryService);
  private dialog = inject(MatDialog);

  readonly actionTypeLabel = actionTypeLabel;
  readonly actionTypeClass = actionTypeClass;

  revisions = signal<RevisionHistory[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);

  ngOnInit(): void {
    // Matches how VnewinspectionComponent reads it: this.RequestId = params.get('RequestId')
    const requestId = this.route.snapshot.paramMap.get('requestId') ?? this.route.snapshot.queryParamMap.get('requestId');
    if (!requestId) {
      this.error.set('No request id was given.');
      return;
    }
    this.load(requestId);
  }

  private load(requestId: string): void {
    this.loading.set(true);
    this.service.getByRequestId(requestId).subscribe({
      next: (data) => {
        this.revisions.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Could not load the history for this request.');
      },
    });
  }

  hasChildren(circulation: CirculationHistory): boolean {
    return circulation.childCirculations.length > 0;
  }

  /** Same pattern as openCirculationModal() in VnewinspectionComponent: a dialog over the sorted children. */
  openChildren(circulation: CirculationHistory): void {
    if (!this.hasChildren(circulation)) return;

    const sorted = [...circulation.childCirculations].sort(
      (a, b) => (a.sequenceNo ?? 0) - (b.sequenceNo ?? 0),
    );

    this.dialog.open(CirculationModalComponent, {
      width: '900px',
      height: '600px',
      data: sorted,
      panelClass: 'circulation-dialog',
    });
  }
}
