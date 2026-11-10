import { Component, ElementRef, computed, inject, input, output, signal, viewChild } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DocumentDraft, DocumentTypeOption } from '../../Core/Models/Document.model';
import { DocumentService } from '../../Core/Services/document.service';

/**
 * The "attach a document" form. It is metadata-driven: what you see (name, number, allowed
 * files, size limit, warnings) comes from the selected DocumentType.
 * It does not upload anything: it emits a DocumentDraft and the host decides what to do with it.
 *
 * Usage:  <app-document-form-dialog #dlg [contactTypeCode]="3" (saved)="onDraft($event)" />
 *         <button (click)="dlg.open()">Attach</button>
 */
@Component({
  selector: 'app-document-form-dialog',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './document-form-dialog.component.html',
  styleUrl: './document-form-dialog.component.css',
})
export class DocumentFormDialogComponent {
  private fb = inject(FormBuilder);
  private service = inject(DocumentService);

  /** Only document types visible to this contact type are offered. null = all active types. */
  contactTypeCode = input<number | null>(null);
  /** Type ids already attached (used to block a second one when MultipleAllowed is false). */
  existingTypeIds = input<string[]>([]);

  saved = output<DocumentDraft>();

  private dialogEl = viewChild.required<ElementRef<HTMLDialogElement>>('dlg');

  types = signal<DocumentTypeOption[]>([]);
  loadingTypes = signal(false);
  loadError = signal(false);
  selectedType = signal<DocumentTypeOption | null>(null);
  selectedFile = signal<File | null>(null);
  fileError = signal<string | null>(null);
  dateError = signal<string | null>(null);
  submitted = signal(false);

  acceptAttr = computed(() => this.selectedType()?.allowedExtensions.join(',') ?? '');

  // NOTE: no <form> tag in the template on purpose. This dialog can sit inside the host's own <form>,
  // and a nested form would also trigger the host's submit.
  form = this.fb.nonNullable.group({
    documentTypeId: ['', Validators.required],
    name: [''],
    number: [''],
    issueDate: [''],
    expireDate: [''],
  });

  constructor() {
    this.form.controls.documentTypeId.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe((id) => this.onTypeChanged(id));
  }

  // ---------- public API ----------
  open(): void {
    this.reset();
    this.loadTypes();
    this.dialogEl().nativeElement.showModal();
  }

  close(): void {
    this.dialogEl().nativeElement.close();
  }

  // ---------- actions ----------
  submit(): void {
    this.submitted.set(true);

    const type = this.selectedType();
    if (this.form.invalid || !type) {
      this.form.markAllAsTouched();
      return;
    }

    const file = this.selectedFile();
    if (!file) {
      this.fileError.set('Choose a file.');
      return;
    }
    if (this.fileError()) return;

    const v = this.form.getRawValue();
    if (v.issueDate && v.expireDate && v.expireDate < v.issueDate) {
      this.dateError.set('The expiry date cannot be before the issue date.');
      return;
    }
    this.dateError.set(null);

    this.saved.emit({
      file,
      documentTypeId: type.documentTypeId,
      documentTypeName: type.documentTypeName,
      name: type.requiresName ? v.name.trim() || null : null,
      number: type.requiresNumber ? v.number.trim() || null : null,
      issueDate: v.issueDate || null,
      expireDate: v.expireDate || null,
    });
    this.close();
  }

  /**
   * Enter in a text box would otherwise submit the host's <form> (this dialog is nested inside it).
   * Treat it as "Attach" instead. Buttons, date and file inputs keep their normal Enter behaviour.
   */
  onEnter(event: Event): void {
    const el = event.target as HTMLInputElement;
    if (el.tagName === 'INPUT' && el.type === 'text') {
      event.preventDefault();
      this.submit();
    }
  }

  onFileChange(input: HTMLInputElement): void {
    const file = input.files?.[0] ?? null;
    this.selectedFile.set(file);
    this.fileError.set(file ? this.validateFile(file, this.selectedType()) : null);
  }

  isTypeTaken(t: DocumentTypeOption): boolean {
    return !t.multipleAllowed && this.existingTypeIds().includes(t.documentTypeId);
  }

  hasError(control: 'documentTypeId' | 'name' | 'number', error: string): boolean {
    const c = this.form.controls[control];
    return (this.submitted() || c.touched) && c.hasError(error);
  }

  formatSize(mb: number): string {
    return mb >= 1024 ? `${(mb / 1024).toFixed(mb % 1024 === 0 ? 0 : 1)} GB` : `${mb} MB`;
  }

  // ---------- internals ----------
  private loadTypes(): void {
    this.loadingTypes.set(true);
    this.loadError.set(false);
    this.service.getTypes(this.contactTypeCode()).subscribe({
      next: (data) => {
        this.types.set(data);
        this.loadingTypes.set(false);
      },
      error: () => {
        this.loadingTypes.set(false);
        this.loadError.set(true);
      },
    });
  }

  /** Applies the rules of the chosen type: which fields are required, and re-checks the chosen file. */
  private onTypeChanged(id: string | null): void {
    const type = this.types().find((t) => t.documentTypeId === id) ?? null;
    this.selectedType.set(type);

    const { name, number } = this.form.controls;
    name.setValidators(type?.requiresName ? [Validators.required, Validators.maxLength(200)] : []);
    number.setValidators(type?.requiresNumber ? [Validators.required, Validators.maxLength(100)] : []);
    if (!type?.requiresName) name.setValue('', { emitEvent: false });
    if (!type?.requiresNumber) number.setValue('', { emitEvent: false });
    name.updateValueAndValidity({ emitEvent: false });
    number.updateValueAndValidity({ emitEvent: false });

    const file = this.selectedFile();
    this.fileError.set(file ? this.validateFile(file, type) : null);
  }

  private validateFile(file: File, type: DocumentTypeOption | null): string | null {
    if (file.size === 0) return 'The file is empty.';
    if (!type) return null; // checked again once a type is chosen

    const dot = file.name.lastIndexOf('.');
    const ext = dot >= 0 ? file.name.substring(dot).toLowerCase() : '';
    if (type.allowedExtensions.length > 0 && !type.allowedExtensions.includes(ext)  && !type.allowedExtensions.includes("*")) {
      return `Allowed file types: ${type.allowedExtensions.join(', ')}.`;
    }
    if (file.size > type.maxSizeMB * 1024 * 1024) {
      return `The file is larger than ${this.formatSize(type.maxSizeMB)}.`;
    }
    return null;
  }

  private reset(): void {
    this.form.reset();
    this.selectedType.set(null);
    this.selectedFile.set(null);
    this.fileError.set(null);
    this.dateError.set(null);
    this.submitted.set(false);
  }
}
