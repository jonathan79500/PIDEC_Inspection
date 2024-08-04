import { bootstrapApplication } from '@angular/platform-browser';
import { config } from './app/app.config.server';
import { MasterComponent } from './app/Shared/master/master.component';

const bootstrap = () => bootstrapApplication(MasterComponent, config);

export default bootstrap;
