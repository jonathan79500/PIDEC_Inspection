import { Component, Inject, Input } from '@angular/core';
import { Circulation } from '../../../Core/Models/Circulation.model';
import { CommonModule } from '@angular/common';
import { Output, EventEmitter } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogActions, MatDialogContent } from '@angular/material/dialog';

@Component({
  selector: 'app-circulation-modal',
  imports: [CommonModule, MatDialogActions, MatDialogContent],
  templateUrl: './circulation-modal.component.html',
  styleUrl: './circulation-modal.component.css'
})
export class CirculationModalComponent {
 //@Input() circulations: Circulation[] = [];

  @Input() visible = false;


 constructor(
    public dialogRef: MatDialogRef<CirculationModalComponent>,
    @Inject(MAT_DIALOG_DATA)
    public circulations: Circulation[]
  ) { }

  close() {
    this.dialogRef.close();
  }

  select(circulation: Circulation) {
    this.dialogRef.close(circulation);
  }


}
