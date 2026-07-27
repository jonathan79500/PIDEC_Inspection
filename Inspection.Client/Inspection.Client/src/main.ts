import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { MasterComponent } from './app/Shared/master/master.component';
import { LoginComponent } from './app/Shared/login/login.component';
import { MainComponent } from './app/Shared/main/main.component';
import { AppComponent } from './app/app.component';

export function getBaseUrl() {
  //return document.getElementsByTagName('base')[0].href;
  return `http://localhost:5196/`;
 //return `https://pims.pidec.com/api/`;
//return `http://ppc.pidec.biz/`;

}


export function getSignUrl() {
  return `http://portal.pidec.com/personnel/images/signature/`;
}

const providers = [
  { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] },
  { provide: 'SIGN_URL', useFactory: getSignUrl, deps: [] }
];

bootstrapApplication(AppComponent, appConfig)
.then((result)=>{


  console.log(result);
})
  .catch((err) => console.error(err));
