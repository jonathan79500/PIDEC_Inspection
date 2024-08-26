import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { MasterComponent } from './app/Shared/master/master.component';
import { LoginComponent } from './app/Shared/login/login.component';
import { MainComponent } from './app/Shared/main/main.component';

bootstrapApplication(MainComponent, appConfig)
// .then((result)=>{
//   console.log(result);
// })
  .catch((err) => console.error(err));
