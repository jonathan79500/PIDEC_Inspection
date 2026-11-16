import { DocumentManagerComponent } from './../document-manager/document-manager.component';
import { Component, OnInit, computed, inject, signal, viewChild } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DatePipe } from '@angular/common';
import {
  ContactOption,
  Inspector,
  InspectorSave,
  InspectorSkill,
  QC_STATES,
  SKILL_LEVELS,
  SkillLevel,
  SkillOption,
} from '../../Core/Models/Inspector.model';
import { InspectorService } from '../../Core/Services/inspector.service';
import { DocumentOwnerType } from '../../Core/Models/Document.model';

@Component({
  selector: 'app-inspector-management',
  standalone: true,
  imports: [ReactiveFormsModule, DatePipe, DocumentManagerComponent],
  templateUrl: './inspector-management.component.html',
  styleUrl: './inspector-management.component.css',
})
export class InspectorManagementComponent implements OnInit {
  private fb = inject(FormBuilder);
  private service = inject(InspectorService);

  /** The documents panel: used to upload queued files after a new inspector is created. */
  private docs = viewChild.required(DocumentManagerComponent);

  readonly ownerType = DocumentOwnerType.Inspector;   // owner type used by the documents panel
  readonly qcStates = QC_STATES;
  readonly skillLevels = SKILL_LEVELS;

  inspectors = signal<Inspector[]>([]);
  contacts = signal<ContactOption[]>([]);
  skillOptions = signal<SkillOption[]>([]);

  editingId = signal<string | null>(null);
  loading = signal(false);
  saving = signal(false);
  submitted = signal(false);
  message = signal<{ type: 'ok' | 'error'; text: string } | null>(null);

  isEditMode = computed(() => this.editingId() !== null);

  // Read-only: shown next to the form so the company can see where review stands, never editable here.
  originalQcState = computed<number>(() => {
    const id = this.editingId();
    return this.inspectors().find((i) => i.inspectorId === id)?.qcApprovalState ?? 2;
  });

  form = this.fb.group({
    inspectionCompanyContactId: ['', Validators.required],
    firstName: ['', [Validators.required, Validators.maxLength(100)]],
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    name: [''],
    position: [''],
    phone: ['', Validators.pattern(/^[0-9+()\-\s]{5,20}$/)],
    email: ['', Validators.email],
    description: [''],
    versionNumber: [{ value: 0, disabled: true }],
    expireDate: [this.toDateInput(new Date()), Validators.required],
    isActive: [true],
    skills: this.fb.array<FormGroup>([], Validators.minLength(1)),
  });

  // The contact decides which document types the user may attach (DocumentTypeVisibility)
  private selectedContactId = toSignal(this.form.controls.inspectionCompanyContactId.valueChanges, {
    initialValue: '',
  });

  contactTypeCode = computed<number | null>(() => {
    const contact = this.contacts().find((c) => c.contactId === this.selectedContactId());
    return contact ? contact.contactTypeCode : null;
  });

  get skillRows(): FormArray<FormGroup> {
    return this.form.controls.skills;
  }

  ngOnInit(): void {
    this.loadContacts();
    this.loadSkills();
    this.loadInspectors();
  }

  // ---------- Loading ----------
  loadInspectors(): void {
    this.loading.set(true);
    this.service.getAll().subscribe({
      next: (data) => {
        this.inspectors.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.showMessage('error', 'Could not load inspectors. Try again.');
      },
    });
  }

  loadContacts(): void {
    
    this.service.getContacts().subscribe({
      next: (data) => this.contacts.set(data),
      error: () => this.showMessage('error', 'Could not load contacts.'),
    });
  }

  loadSkills(): void {
    this.service.getSkills().subscribe({
      next: (data) => this.skillOptions.set(data),
      error: () => this.showMessage('error', 'Could not load skills.'),
    });
  }

  // ---------- Skills ----------
  private newSkillRow(skillId = '', level: SkillLevel = 'Experienced'): FormGroup {
    return this.fb.group({
      skillId: [skillId, Validators.required],
      level: [level, Validators.required],
    });
  }

  addSkill(): void {
    this.skillRows.push(this.newSkillRow());
  }

  removeSkill(index: number): void {
    this.skillRows.removeAt(index);
  }

  /** True when another row already uses this skill (so it can't be picked twice). */
  isSkillTaken(skillId: string, rowIndex: number): boolean {
    return this.skillRows.controls.some((g, i) => i !== rowIndex && g.value.skillId === skillId);
  }

  canAddSkill(): boolean {
    return this.skillRows.length < this.skillOptions().length;
  }

  // ---------- Form actions ----------
  save(): void {
    this.submitted.set(true);
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const first = raw.firstName?.trim() ?? '';
    const last = raw.lastName?.trim() ?? '';

    const payload: InspectorSave = {
      inspectorId: this.editingId() ?? undefined,
      inspectionCompanyContactId: raw.inspectionCompanyContactId!,
      isActive: !!raw.isActive,
      firstName: first,
      lastName: last,
      name: raw.name?.trim() || `${first} ${last}`.trim(),
      position: raw.position,
      phone: raw.phone,
      email: raw.email,
      description: raw.description,
      expireDate: new Date(raw.expireDate!).toISOString(),
      skills: (raw.skills as { skillId: string; level: SkillLevel }[]).map((s) => ({
        skillId: s.skillId,
        level: s.level,
      })),
    };

    this.saving.set(true);
    const id = this.editingId();
    const request$ = id ? this.service.update(id, payload) : this.service.create(payload);

    request$.subscribe({
      next: (saved) => {
        if (!id && saved.inspectorId) {
          // New inspector: now that it has an id, upload the documents queued in the panel
          this.docs()
            .uploadPending(saved.inspectorId)
            .subscribe(({ failed }) => {
              if (failed > 0) {
                this.finishSave(
                  `Inspector saved, but ${failed} document(s) failed to upload. Edit the inspector to attach them again.`,
                  'error',
                );
              } else {
                this.finishSave('Inspector added.');
              }
            });
        } else {
          this.finishSave('Inspector updated.');
        }
      },
      error: (err) => {
        this.saving.set(false);
        const apiText = err?.error?.error as string | undefined;
        this.showMessage('error', apiText ?? 'Could not save the inspector. Check the fields and try again.');
      },
    });
  }

  private finishSave(text: string, type: 'ok' | 'error' = 'ok'): void {
    this.saving.set(false);
    this.showMessage(type, text);
    this.resetForm();
    this.loadInspectors();
  }

  edit(inspector: Inspector): void {
    this.editingId.set(inspector.inspectorId ?? null);
    this.submitted.set(false);

    this.skillRows.clear();
    (inspector.skills ?? []).forEach((s: InspectorSkill) =>
      this.skillRows.push(this.newSkillRow(s.skillId, s.level)),
    );

    this.form.patchValue({
      inspectionCompanyContactId: inspector.inspectionCompanyContactId,
      firstName: inspector.firstName ?? '',
      lastName: inspector.lastName ?? '',
      name: inspector.name ?? '',
      position: inspector.position ?? '',
      phone: inspector.phone ?? '',
      email: inspector.email ?? '',
      description: inspector.description ?? '',
      versionNumber: inspector.versionNumber,
      expireDate: this.toDateInput(new Date(inspector.expireDate)),
      isActive: inspector.isActive,
    });

    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  remove(inspector: Inspector): void {
    if (!inspector.inspectorId) return;
    if (!confirm(`Delete ${inspector.name || 'this inspector'} and all of their documents?`)) return;

    this.service.delete(inspector.inspectorId).subscribe({
      next: () => {
        if (this.editingId() === inspector.inspectorId) this.resetForm();
        this.showMessage('ok', 'Inspector deleted.');
        this.loadInspectors();
      },
      error: () => this.showMessage('error', 'Could not delete the inspector.'),
    });
  }

  resetForm(): void {
    this.editingId.set(null);
    this.submitted.set(false);
    this.skillRows.clear();
    this.form.reset({
      inspectionCompanyContactId: '',
      firstName: '',
      lastName: '',
      name: '',
      position: '',
      phone: '',
      email: '',
      description: '',
      versionNumber: 0,
      expireDate: this.toDateInput(new Date()),
      isActive: true,
    });
  }

  // ---------- Helpers ----------
  qcLabel(value: number): string {
    return this.qcStates.find((s) => s.value === value)?.label ?? '-';
  }

  hasError(control: string, error: string): boolean {
    const c = this.form.get(control);
    return !!c && c.touched && c.hasError(error);
  }

  private currentCreatedDate(): string {
    const id = this.editingId();
    const existing = this.inspectors().find((i) => i.inspectorId === id);
    return existing?.createdDate ?? new Date().toISOString();
  }

  private toDateInput(d: Date): string {
    return d.toISOString().substring(0, 10); // yyyy-MM-dd
  }

  private showMessage(type: 'ok' | 'error', text: string): void {
    this.message.set({ type, text });
    setTimeout(() => this.message.set(null), 6000);
  }
}
