import { InjectionToken } from '@angular/core';
import { IWorkflowHandler } from '../Interfaces/IWorkflowHandler';


export const WORKFLOW_HANDLERS =
    new InjectionToken<IWorkflowHandler[]>(
        'WORKFLOW_HANDLERS'
    );
