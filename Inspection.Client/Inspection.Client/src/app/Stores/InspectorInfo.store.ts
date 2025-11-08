import { getState, patchState, signalStore, withMethods, withState } from "@ngrx/signals";
import { effect, inject } from "@angular/core";
import { BasicInfoService } from "../Core/Services/basic-info.service";
import { UserService } from "../Core/Services/user.service";
import { Observable, of, Subject } from "rxjs";
import { BasicInspectorInfo } from "../Core/Models/BasicInspectorInfo.model";



const initialState: BasicInspectorInfo = {
  name: '',
  userName: '',
  domainId: 0,
  inspectionTypes: [],
  inspectionSubjects:[],
  projects: [],
  orders:[],
  participated_Circulations: [],
  receivedInspectionNotifications: [] ,
  inspectionReports:[]

}

export const InspectorInfoStore = signalStore(
  { providedIn: 'root' },
  withState<BasicInspectorInfo>(initialState),

  withMethods((store) => {

    const stateSubject = new Subject<BasicInspectorInfo>();

    return {
      SetBasicInspectorInfo(basicInfo: any) {
        patchState(store, (state)=>({
          ...state,
          domainId:basicInfo.domainId,
          name:basicInfo.name,
          userName:basicInfo.userName,
          orders:[...basicInfo.orders],
          projects:[...basicInfo.projects],
          inspectionSubjects:[...(basicInfo.inspectionSubjects || [])],
          inspectionTypes: [...(basicInfo.inspectionTypes || [])],
         // orderContacts:[...(basicInfo.orderContacts || [])],
          orderInspectionSubjects:[...(basicInfo.orderInspectionSubjects || [])],
         // projectContacts:[...(basicInfo.projectContacts || [])],
         // orderSettingsExceptions:[...(basicInfo.orderSettingsExceptions || [])],
         // isLoading: true,
         // ...getState(store)
        }));
      },

      fetchBasicInspectorInfo: (): Observable<BasicInspectorInfo> => {
        const currentState = getState(store);
        if (currentState.projects != undefined && currentState.projects.length > 0) {
         // stateSubject.next(currentState);
          console.log('Emitting current state:', currentState);
          return of(currentState);//stateSubject.asObservable();
        } else {
          const userService = inject(UserService);
          const resultObservable = userService.GetVendorData();
          resultObservable.subscribe((basicInfo) => {
            console.log('Fetched vendor data:', basicInfo);
            patchState(store, (state) => ({
              ...state,
              orders: basicInfo.orders,
              name: basicInfo.name,
              userName: basicInfo.userName,
              domainId: basicInfo.domainId,
              inspectionTypes: basicInfo.inspectionTypes,
              inspectionSubjects:basicInfo.inspectionSubjects,
              orderContacts:basicInfo.orderContacts,
              orderInspectionSubjects:basicInfo.orderInspectionSubjects,
              projectContacts : basicInfo.projectContacts,
              orderSettingsExceptions: basicInfo.orderSettingsExceptions,
              projects: basicInfo.projects,
              isLoading: false
            }));
            console.log('State after patch:', getState(store));
          });
          return resultObservable;
        }

    }
  }
  })
);

