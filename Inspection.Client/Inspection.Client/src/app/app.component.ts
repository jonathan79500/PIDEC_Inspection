import { Component, OnInit } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';



@Component({
    selector: 'app-root',
    imports: [RouterOutlet],
    templateUrl: './app.component.html',
    styles: []
})
export class AppComponent implements OnInit{

  constructor(private router: Router) {}


  ngOnInit(): void {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        if (event.url.includes('pidec')) {
          document.body.className = 'blue-theme';
        } else if (event.url.includes('vendor')) {
          document.body.className = 'green-theme';
        }
        else if (event.url.includes('inspector')) {
          document.body.className = 'orange-brown-theme';
        }
      }
    });
  }


}
