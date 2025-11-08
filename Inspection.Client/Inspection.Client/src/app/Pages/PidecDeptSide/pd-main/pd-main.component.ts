import { CommonModule } from '@angular/common';
import { Component, effect, inject, input } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { currentstateStore } from '../../../Stores/currentstate.store';

@Component({
  selector: 'app-pd-main',
  imports: [RouterOutlet,CommonModule],
  templateUrl: './pd-main.component.html',
  styleUrl: './pd-main.component.css'
})
export class PdMainComponent {
 currentAppState = inject(currentstateStore);
  private SelectedOrder :any = undefined;
  isLeftSidebarCollapsed = input.required<boolean>();
  screenWidth = input.required<number>();

  constructor() {
    effect(()=>{
      this.SelectedOrder = this.currentAppState.currentorder();
    })

  }
}
