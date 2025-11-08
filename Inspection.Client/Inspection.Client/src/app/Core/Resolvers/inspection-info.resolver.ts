import { ResolveFn } from '@angular/router';
import { InspectionService, queryNewNotificationNumberResult } from '../Services/inspection.service';
import { Inject, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { currentstateStore } from '../../Stores/currentstate.store';

export const inspectionInfoResolver: ResolveFn<queryNewNotificationNumberResult> =  (route, state,inspectionService: InspectionService=  inject(InspectionService),currentAppState = inject(currentstateStore)) :
Observable<queryNewNotificationNumberResult> =>
inspectionService.getNewNotificationNumber(currentAppState.currentproject()?.number,currentAppState.currentorder()?.name);
