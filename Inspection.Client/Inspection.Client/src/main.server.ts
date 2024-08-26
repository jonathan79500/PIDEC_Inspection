import { bootstrapApplication } from '@angular/platform-browser';
import { config } from './app/app.config.server';
import { MasterComponent } from './app/Shared/master/master.component';
import { ApplicationModule } from '@angular/core';
import { LoginComponent } from './app/Shared/login/login.component';

const bootstrap = () => bootstrapApplication(LoginComponent, config);

export default bootstrap;
