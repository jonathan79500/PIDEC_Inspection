import { CommonModule, NgIf, NgTemplateOutlet } from '@angular/common';
import { Component, ElementRef, NgModule, TemplateRef, input, viewChild } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';


@Component({
  selector: 'app-login',
  standalone: true,
   imports: [CommonModule],  
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  title = input();
ctx: any;
div =  viewChild<ElementRef>('estimateTemplate')

  constructor()
  {
    let totalEstimate : number =10;
    this.ctx={estimate :totalEstimate};
  }
  
  

 
}
