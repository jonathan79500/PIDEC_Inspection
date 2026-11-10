import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Inspector } from '../../Core/Models/Inspector.model';
import { InspectorQcService } from '../../Core/Services/Inspectorqc.service';

@Component({
  selector: 'app-inspector-qc-review',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './inspector-qc-review.component.html',
  styleUrl: './inspector-qc-review.component.css',
})
export class InspectorQcReviewComponent implements OnInit {
  private service = inject(InspectorQcService);

  inspectors = signal<Inspector[]>([]);
  loading = signal(false);
  busyId = signal<string | null>(null);
  message = signal<{ type: 'ok' | 'error'; text: string } | null>(null);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.service.getPending().subscribe({
      next: (data) => {
        this.inspectors.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.showMessage('error', 'Could not load the pending inspectors.');
      },
    });
  }

  approve(inspector: Inspector): void {
    const comment = window.prompt(`Approve ${inspector.name}. Add a note (optional):`, '') ?? undefined;
    this.busyId.set(inspector.inspectorId!);
    this.service.approve(inspector.inspectorId!, comment).subscribe({
      next: () => this.onDecided(inspector, 'Inspector approved.'),
      error: (err) => this.onError(err, 'Could not approve the inspector.'),
    });
  }

  reject(inspector: Inspector): void {
    const comment = window.prompt(`Reject ${inspector.name}. Reason (required):`, '');
    if (comment === null) return;              // the user cancelled
    if (!comment.trim()) {
      this.showMessage('error', 'A reason is required to reject an inspector.');
      return;
    }

    this.busyId.set(inspector.inspectorId!);
    this.service.reject(inspector.inspectorId!, comment.trim()).subscribe({
      next: () => this.onDecided(inspector, 'Inspector rejected.'),
      error: (err) => this.onError(err, 'Could not reject the inspector.'),
    });
  }

  private onDecided(inspector: Inspector, text: string): void {
    this.busyId.set(null);
    this.showMessage('ok', text);
    // the inspector leaves the pending queue once it has a decision
    this.inspectors.update((list) => list.filter((i) => i.inspectorId !== inspector.inspectorId));
  }

  private onError(err: unknown, fallback: string): void {
    this.busyId.set(null);
    const apiText = (err as { error?: { error?: string } })?.error?.error;
    this.showMessage('error', apiText ?? fallback);
  }

  private showMessage(type: 'ok' | 'error', text: string): void {
    this.message.set({ type, text });
    setTimeout(() => this.message.set(null), 6000);
  }
}
