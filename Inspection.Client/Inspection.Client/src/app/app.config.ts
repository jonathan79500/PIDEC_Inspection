import { ApplicationConfig, importProvidersFrom, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { RouterOutlet,RouterLink ,RouterLinkActive} from '@angular/router';
import { routes } from './app.routes';
import { BrowserModule, provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { CommonModule, NgIf, NgTemplateOutlet } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { provideHttpClient, withFetch } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [provideZoneChangeDetection({ eventCoalescing: true }),provideHttpClient(withFetch()),provideRouter(routes,withComponentInputBinding()), provideClientHydration(withEventReplay()),importProvidersFrom(
    CommonModule,FormsModule,ReactiveFormsModule,NgIf,NgTemplateOutlet
  )]
};
