import { Component,booleanAttribute, computed, effect, inject, input, signal, untracked, viewChild } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Observable, catchError, concatMap, from, map, of, toArray } from 'rxjs';
import { DocumentDraft, DocumentItem} from '../../Core/Models/Document.model';
import { DocumentOwnerType } from '../../Core/Models/Document/DocumentOwnerType.model';
import { DocumentService } from '../../Core/Services/document.service';
import { DocumentFormDialogComponent } from '../document-form-dialog/document-form-dialog.component';


/**
 * Drop-in documents panel for any owner (inspector, company, ...).
 *
 *  - ownerId set   -> "Attach document" uploads straight away; saved documents are listed with download / delete.
 *  - ownerId null  -> documents are queued; call uploadPending(newOwnerId) once the owner has been saved.
 */
@Component({
  selector: 'app-document-manager',
  standalone: true,
  imports: [DatePipe, DocumentFormDialogComponent],
  templateUrl: './document-manager.component.html',
  styleUrl: './document-manager.component.css',
})
export class DocumentManagerComponent {
  private service = inject(DocumentService);
  private dialog = viewChild.required(DocumentFormDialogComponent);

  ownerType = input.required<DocumentOwnerType>();
  ownerId = input<string | null>(null);
  contactTypeCode = input<number | null>(null);
  disabled = input(false);
  disabledHint = input('');
  /** View-only: documents can be listed and downloaded, but not attached or deleted. */
  readOnly = input(false, { transform: booleanAttribute });

  documents = signal<DocumentItem[]>([]);
  pending = signal<DocumentDraft[]>([]);
  busy = signal(false);
  error = signal<string | null>(null);

  existingTypeIds = computed(() => [
    ...this.documents().map((d) => d.documentTypeId),
    ...this.pending().map((p) => p.documentTypeId),
  ]);

  constructor() {
    // Whenever the owner changes (another inspector selected, form cleared, ...) start fresh
    effect(() => {
      const id = this.ownerId();
      untracked(() => {
        this.pending.set([]);
        this.error.set(null);
        if (id) this.load(id);
        else this.documents.set([]);
      });
    });
  }

  openDialog(): void {
    if (this.readOnly()) return;
    this.dialog().open();
  }

  onDraft(draft: DocumentDraft): void {
    const id = this.ownerId();
    if (!id) {
      this.pending.update((list) => [...list, draft]);
      return;
    }

    this.busy.set(true);
    this.error.set(null);
    this.service.upload(this.ownerType(), id, draft).subscribe({
      next: (doc) => {
        this.documents.update((list) => [...list, doc]);
        this.busy.set(false);
      },
      error: (err) => {
        this.busy.set(false);
        this.error.set((err?.error?.error as string) ?? `Could not upload ${draft.file.name}.`);
      },
    });
  }

  /** Uploads the queued documents once the owner exists. Never errors: it reports how many failed. */
  uploadPending(ownerId: string): Observable<{ ok: number; failed: number }> {
    return from(this.pending()).pipe(
      concatMap((draft) =>
        this.service.upload(this.ownerType(), ownerId, draft).pipe(
          map(() => true),
          catchError(() => of(false)),
        ),
      ),
      toArray(),
      map((results) => ({
        ok: results.filter((r) => r).length,
        failed: results.filter((r) => !r).length,
      })),
    );
  }

  removePending(index: number): void {
    this.pending.update((list) => list.filter((_, i) => i !== index));
  }

  download(doc: DocumentItem): void {
    const id = this.ownerId();
    if (!id) return;
    this.service.download(this.ownerType(), id, doc.documentId).subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = doc.fileName;
        link.click();
        URL.revokeObjectURL(url);
      },
      error: () => this.error.set('Could not download the file.'),
    });
  }

  remove(doc: DocumentItem): void {
    const id = this.ownerId();
    if (this.readOnly() || !id || !confirm(`Delete ${doc.fileName}?`)) return;
    this.service.delete(this.ownerType(), id, doc.documentId).subscribe({
      next: () => this.documents.update((list) => list.filter((d) => d.documentId !== doc.documentId)),
      error: () => this.error.set('Could not delete the document.'),
    });
  }

  statusLabel(status?: string | null): string {
    return status ? status.replace(/([a-z])([A-Z])/g, '$1 $2') : '-';
  }

  formatSize(bytes: number): string {
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(0)} KB`;
    return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
  }

  private load(ownerId: string): void {
    this.service.list(this.ownerType(), ownerId).subscribe({
      next: (data) => this.documents.set(data),
      error: () => this.error.set('Could not load the documents.'),
    });
  }
}
