import { Component, effect, inject, input } from '@angular/core';
import { currentstateStore } from '../../../Stores/currentstate.store';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-ins-main',
  imports: [RouterOutlet,CommonModule],
  templateUrl: './ins-main.component.html',
  styleUrl: './ins-main.component.css'
})
export class InsMainComponent {
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
