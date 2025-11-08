import { ApplicationConfig, importProvidersFrom, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { RouterOutlet,RouterLink ,RouterLinkActive} from '@angular/router';
import { routes } from './app.routes';
import { BrowserModule, provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { CommonModule, NgIf, NgTemplateOutlet } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import {MAT_DIALOG_DEFAULT_OPTIONS} from '@angular/material/dialog';
import {provideStore} from '@ngrx/store';
import { provideToastr } from 'ngx-toastr';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { provideNativeDateAdapter } from '@angular/material/core';


export const appConfig: ApplicationConfig = {
  providers: [provideZoneChangeDetection({ eventCoalescing: true }),provideHttpClient(withFetch()),provideRouter(routes,withComponentInputBinding()), provideClientHydration(withEventReplay()),
    provideStore(),provideToastr()
    ,{provide: MAT_DIALOG_DEFAULT_OPTIONS, useValue: {hasBackdrop: false}}
    ,importProvidersFrom(
    CommonModule,FormsModule,ReactiveFormsModule,NgIf,NgTemplateOutlet,MatDatepickerModule
  ),provideNativeDateAdapter(), provideAnimationsAsync(), provideAnimationsAsync(), provideAnimationsAsync()]
};
