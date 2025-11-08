import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LoginComponent } from "../login/login.component";


@Component({
    selector: 'app-main',
    imports: [RouterOutlet],
    templateUrl: './main.component.html',
    styleUrl: './main.component.css',
    encapsulation: ViewEncapsulation.None
})
export class MainComponent implements OnInit{


  /**
   *
   */
  constructor() {
    
    
  }
  ngOnInit(): void {
   
  }

}
