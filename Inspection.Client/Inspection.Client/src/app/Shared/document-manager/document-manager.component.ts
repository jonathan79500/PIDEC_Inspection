import { Component, Input } from '@angular/core';
import { DocumentVersionVM } from '../../Core/Models/Document/DocumentVersionVM.model';
import { DocumentVM } from '../../Core/Models/Document/DocumentVM.model';
import { DocumentTypeVM } from '../../Core/Models/Document/DocumentTypeVM.model';
import { DocumentOwnerType } from '../../Core/Models/Document/DocumentOwnerType.model';
import {  MatIconModule } from '@angular/material/icon';
import {  MatTableModule } from '@angular/material/table';
import {  MatFormFieldModule, MatLabel } from '@angular/material/form-field';
import {  MatInputModule } from '@angular/material/input';
import {  MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialogModule } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatMenuModule } from '@angular/material/menu';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-document-manager',
  imports:[CommonModule,FormsModule,ReactiveFormsModule,MatExpansionModule,MatProgressBarModule,MatMenuModule,MatProgressSpinnerModule,MatIconModule,MatTableModule,MatFormFieldModule,MatLabel,MatInputModule,MatButtonModule,MatSelectModule,MatCardModule,MatDialogModule,MatTooltipModule ],
  templateUrl: './document-manager.component.html',
  styleUrls: ['./document-manager.component.css']
})
export class DocumentManagerComponent {

  @Input() ownerId!: string;

  @Input() ownerType!: DocumentOwnerType;

  @Input() readOnly = false;

  @Input() allowedDocumentTypes: DocumentTypeVM[] = [];

  @Input() documents: DocumentVM[] = [];

  selectedDocumentType?: number;

  selectedFiles: File[] = [];

  searchText = '';

  displayedVersionColumns = [
    'version',
    'status',
    'uploaded',
    'uploadedBy',
    'remarks',
    'actions'
  ];

  //------------------------------------------------------

  onFilesSelected(event: Event): void {

    const input = event.target as HTMLInputElement;

    if (!input.files)
      return;

    this.selectedFiles = Array.from(input.files);
  }

  //------------------------------------------------------

  uploadNewDocument(): void {

    if (!this.selectedFiles.length)
      return;

    if (!this.selectedDocumentType)
      return;

    // TODO:
    // this.documentService.uploadNewDocument(...);

  }

  //------------------------------------------------------

  uploadNewVersion(document: DocumentVM): void {

    // open upload dialog

    console.log(document);

  }

  //------------------------------------------------------

  preview(version: DocumentVersionVM): void {

    // TODO

    console.log(version);

  }

  //------------------------------------------------------

  download(version: DocumentVersionVM): void {

    // TODO

    console.log(version);

  }

  //------------------------------------------------------

  deleteVersion(version: DocumentVersionVM): void {

    // TODO

    console.log(version);

  }

}
