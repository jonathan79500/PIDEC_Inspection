import { CommonModule } from '@angular/common';
import { Component, effect, inject, input } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { currentstateStore } from '../../../Stores/currentstate.store';

@Component({
    selector: 'app-pmain',
    imports: [RouterOutlet, CommonModule],
    templateUrl: './pmain.component.html',
    styleUrl: './pmain.component.css'
})
export class PmainComponent {

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
