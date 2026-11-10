import { Component, Inject, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { InspectorService } from '../../../Core/Services/inspector.service'; // TODO: fix path to your real file
import { Inspector, SkillOption } from '../../../Core/Models/Inspector.model'; // TODO: fix path to your real file

export interface InspectorQuickAddData {
  /** The inspecting company this new person belongs to (Contact.ContactType PidecInspector(8) or TPInspector(9)). */
  inspectionCompanyContactId: string;
}

/**
 * A short create-only form, opened from the inspection form when "Register a new inspector" is used.
 * It does NOT replace InspectorManagementComponent (the full roster screen) - it just gets a usable
 * Inspector saved fast, with one skill, so the inspection can go out without leaving this page.
 */
@Component({
  selector: 'app-inspector-quick-add-dialog',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatDialogModule,
    MatFormFieldModule, MatInputModule, MatButtonModule, MatSelectModule,
  ],
  templateUrl: './inspector-quick-add-dialog.component.html',
  styleUrl: './inspector-quick-add-dialog.component.css',
})
export class InspectorQuickAddDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private service = inject(InspectorService);
  private dialogRef = inject(MatDialogRef<InspectorQuickAddDialogComponent>);

  skills = signal<SkillOption[]>([]);
  saving = signal(false);
  error = signal<string | null>(null);

  form: FormGroup = this.fb.group({
    firstName: ['', [Validators.required, Validators.maxLength(100)]],
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    position: [''],
    phone: [''],
    email: ['', Validators.email],
    skillId: ['', Validators.required],
  });

  constructor(@Inject(MAT_DIALOG_DATA) public data: InspectorQuickAddData) {}

  ngOnInit(): void {
    this.service.getActiveSkills().subscribe({
      next: (skills) => this.skills.set(skills),
      error: () => this.error.set('Could not load the skill list.'),
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.getRawValue();
    this.saving.set(true);
    this.error.set(null);

    this.service
      .create({
        inspectionCompanyContactId: this.data.inspectionCompanyContactId,
        isActive: true,
        firstName: v.firstName.trim(),
        lastName: v.lastName.trim(),
        position: v.position || null,
        phone: v.phone || null,
        email: v.email || null,
        expireDate: new Date(new Date().getFullYear() + 1, 11, 31).toISOString(),
        skills: [{ skillId: v.skillId, level: 'Experienced' }],
      })
      .subscribe({
        next: (created) => this.dialogRef.close(created),
        error: (err) => {
          this.saving.set(false);
          this.error.set(err?.error?.error ?? 'Could not save the inspector.');
        },
      });
  }

  cancel(): void {
    this.dialogRef.close(null);
  }
}
