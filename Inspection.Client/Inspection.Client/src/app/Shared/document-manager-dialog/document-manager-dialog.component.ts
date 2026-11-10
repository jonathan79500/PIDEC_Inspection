import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { Component, Inject } from '@angular/core';
import { DocumentDialogData } from '../../Core/Models/Document/DocumentDialogData.model';
import { DocumentManagerComponent } from "../document-manager/document-manager.component";
import {DocumentOwnerType} from '../../Core/Models/Document/DocumentOwnerType.model'

@Component({
    selector: 'app-document-manager-dialog',
    imports: [MatDialogModule, DocumentManagerComponent],
    templateUrl: './document-manager-dialog.component.html'
})
export class DocumentManagerDialogComponent {

    constructor(

        @Inject(MAT_DIALOG_DATA)

        public data: DocumentDialogData

    ) { }

}
