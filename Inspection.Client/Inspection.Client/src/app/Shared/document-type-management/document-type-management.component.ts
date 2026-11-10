import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CONTACT_TYPES, ContactTypes} from '../../Core/Models/ContactTypes.enum';
import {  DocumentTypeAdmin, DocumentTypeSave } from '../../Core/Models/DocumentType.model';
import { DocumentTypeService } from '../../Core/Services/documenttype.service';

@Component({
  selector: 'app-document-type-management',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './document-type-management.component.html',
  styleUrl: './document-type-management.component.css',
})
export class DocumentTypeManagementComponent implements OnInit {
  private fb = inject(FormBuilder);
  private service = inject(DocumentTypeService);

  readonly contactTypes = CONTACT_TYPES;

  types = signal<DocumentTypeAdmin[]>([]);
  editingId = signal<string | null>(null);
  loading = signal(false);
  saving = signal(false);
  message = signal<{ type: 'ok' | 'error'; text: string } | null>(null);

  isEditMode = computed(() => this.editingId() !== null);

  form = this.fb.nonNullable.group({
    documentTypeName: ['', [Validators.required, Validators.maxLength(150)]],
    documentTypeCode: [0, [Validators.required, Validators.min(1)]],
    allowedExtensions: ['*', [Validators.required, Validators.maxLength(200)]],
    maxSizeMB: [10, [Validators.required, Validators.min(1), Validators.max(10240)]],
    multipleAllowed: [true],
    isActive: [true],
    requiresName: [false],
    requiresNumber: [false],
    requiresReview: [false],
    requiresApproval: [false],
    visibleToContactTypes: this.fb.nonNullable.control<ContactTypes[]>([]),
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.service.getAll().subscribe({
      next: (data) => {
        this.types.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.showMessage('error', 'Could not load document types.');
      },
    });
  }

  // ---------- contact-type checkboxes ----------
  isContactTypeChecked(value: ContactTypes): boolean {
    return this.form.controls.visibleToContactTypes.value.includes(value);
  }

  toggleContactType(value: ContactTypes, checked: boolean): void {
    const current = this.form.controls.visibleToContactTypes.value;
    const next = checked ? [...current, value] : current.filter((c) => c !== value);
    this.form.controls.visibleToContactTypes.setValue(next);
  }

  // ---------- form actions ----------
  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const data: DocumentTypeSave = this.form.getRawValue();
    const id = this.editingId();

    this.saving.set(true);
    const request$ = id ? this.service.update(id, data) : this.service.create(data);

    request$.subscribe({
      next: () => {
        this.saving.set(false);
        this.showMessage('ok', id ? 'Document type updated.' : 'Document type added.');
        this.resetForm();
        this.load();
      },
      error: (err) => {
        this.saving.set(false);
        const apiText = err?.error?.error as string | undefined;
        this.showMessage('error', apiText ?? 'Could not save the document type.');
      },
    });
  }

  edit(type: DocumentTypeAdmin): void {
    this.editingId.set(type.documentTypeId);
    this.form.reset({
      documentTypeName: type.documentTypeName,
      documentTypeCode: type.documentTypeCode,
      allowedExtensions: type.allowedExtensions,
      maxSizeMB: type.maxSizeMB,
      multipleAllowed: type.multipleAllowed,
      isActive: type.isActive,
      requiresName: type.requiresName,
      requiresNumber: type.requiresNumber,
      requiresReview: type.requiresReview,
      requiresApproval: type.requiresApproval,
      visibleToContactTypes: [...type.visibleToContactTypes],
    });
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  remove(type: DocumentTypeAdmin): void {
    if (!confirm(`Delete "${type.documentTypeName}"?`)) return;

    this.service.delete(type.documentTypeId).subscribe({
      next: () => {
        if (this.editingId() === type.documentTypeId) this.resetForm();
        this.showMessage('ok', 'Document type deleted.');
        this.load();
      },
      error: (err) => {
        const apiText = err?.error?.error as string | undefined;
        this.showMessage('error', apiText ?? 'Could not delete the document type.');
      },
    });
  }

  resetForm(): void {
    this.editingId.set(null);
    this.form.reset({
      documentTypeName: '',
      documentTypeCode: 0,
      allowedExtensions: '*',
      maxSizeMB: 10,
      multipleAllowed: true,
      isActive: true,
      requiresName: false,
      requiresNumber: false,
      requiresReview: false,
      requiresApproval: false,
      visibleToContactTypes: [],
    });
  }

  // ---------- helpers ----------
  hasError(control: keyof typeof this.form.controls, error: string): boolean {
    const c = this.form.get(control as string);
    return !!c && c.touched && c.hasError(error);
  }

  contactTypeNames(type: DocumentTypeAdmin): string {
    if (type.visibleToContactTypes.length === 0) return 'Nobody yet';
    return this.contactTypes
      .filter((c) => type.visibleToContactTypes.includes(c.value))
      .map((c) => c.label)
      .join(', ');
  }

  private showMessage(type: 'ok' | 'error', text: string): void {
    this.message.set({ type, text });
    setTimeout(() => this.message.set(null), 6000);
  }
}
