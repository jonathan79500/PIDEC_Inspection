import { ApplicationConfig, importProvidersFrom, InjectionToken, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { RouterOutlet,RouterLink ,RouterLinkActive} from '@angular/router';
import { routes } from './app.routes';
import { BrowserModule, provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { CommonModule, HashLocationStrategy, LocationStrategy, NgIf, NgTemplateOutlet } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import {MAT_DIALOG_DEFAULT_OPTIONS} from '@angular/material/dialog';
import {provideStore} from '@ngrx/store';
import { provideToastr } from 'ngx-toastr';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { provideNativeDateAdapter } from '@angular/material/core';
import { QCWorkflowHandler } from './Core/WorkflowEngine/WorkflowHandlers/QCWorkflowHandler';
import { TechnicalLeadWorkflowHandler } from './Core/WorkflowEngine/WorkflowHandlers/TechnicalLeadWorkflowHandler';
import { PInspectorWorkflowHandler } from './Core/WorkflowEngine/WorkflowHandlers/PInspectorWorkflowHandler';
import { TInspectorWorkflowHandler } from './Core/WorkflowEngine/WorkflowHandlers/TInspectorWorkflowHandler';
import { IWorkflowHandler } from './Core/WorkflowEngine/Interfaces/IWorkflowHandler';
import { WORKFLOW_HANDLERS } from './Core/WorkflowEngine/WorkflowHandlers/workflow-handler.token';
import { VendorWorkflowHandler } from './Core/WorkflowEngine/WorkflowHandlers/VendorWorkflowHandler';

export const appConfig: ApplicationConfig = {
  providers: [provideZoneChangeDetection({ eventCoalescing: true }),provideHttpClient(withFetch()),provideRouter(routes,withComponentInputBinding()), provideClientHydration(withEventReplay()),
    provideStore(),provideToastr()
    ,{provide: MAT_DIALOG_DEFAULT_OPTIONS, useValue: {hasBackdrop: false}}
    // ,{ provide: LocationStrategy, useClass: HashLocationStrategy }
    ,importProvidersFrom(
    CommonModule,FormsModule,ReactiveFormsModule,NgIf,NgTemplateOutlet,MatDatepickerModule
  ),provideNativeDateAdapter(), provideAnimationsAsync(), provideAnimationsAsync(), provideAnimationsAsync(),
QCWorkflowHandler,
    TInspectorWorkflowHandler,
    TechnicalLeadWorkflowHandler,
    PInspectorWorkflowHandler,
    VendorWorkflowHandler,

    {
        provide: WORKFLOW_HANDLERS,
        multi: true,
        useExisting: QCWorkflowHandler
    },

    {
        provide: WORKFLOW_HANDLERS,
        multi: true,
        useExisting: TInspectorWorkflowHandler
    },

    {
        provide: WORKFLOW_HANDLERS,
        multi: true,
        useExisting: TechnicalLeadWorkflowHandler
    },
   {
        provide: WORKFLOW_HANDLERS,
        multi: true,
        useExisting: PInspectorWorkflowHandler
    },
   {
        provide: WORKFLOW_HANDLERS,
        multi: true,
        useExisting: VendorWorkflowHandler
    }]
};
