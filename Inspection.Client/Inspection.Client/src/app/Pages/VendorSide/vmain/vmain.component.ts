import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, input } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { Order } from '../../../Core/Models/Order.model';
import { vendorInfoStore } from '../../../Stores/vendorInfo.store';
import { currentstateStore } from '../../../Stores/currentstate.store';

@Component({
    selector: 'app-vmain',
    imports: [RouterOutlet, CommonModule],
    templateUrl: './vmain.component.html',
    styleUrl: './vmain.component.css'
})
export class VmainComponent {

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
