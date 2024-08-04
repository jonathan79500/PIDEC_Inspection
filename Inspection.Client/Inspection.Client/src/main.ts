import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { MasterComponent } from './app/Shared/master/master.component';

bootstrapApplication(MasterComponent, appConfig)
  .catch((err) => console.error(err));
