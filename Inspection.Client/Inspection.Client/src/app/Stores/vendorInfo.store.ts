import { getState, patchState, signalStore, withMethods, withState } from "@ngrx/signals";
import { BasicVendorInfo } from "../Core/Models/BasicVendorInfo.model";
import { effect, inject } from "@angular/core";
import { BasicInfoService } from "../Core/Services/basic-info.service";
import { UserService } from "../Core/Services/user.service";
import { state } from "@angular/animations";
import { Observable, of, Subject } from "rxjs";



const initialState: BasicVendorInfo = {
  name: '',
  userName: '',
  domainId: 0,
  inspectionTypes: [],
  inspectionSubjects:[],
  orders: [],
  projects: [],
  isLoading: true,
  participated_Circulations: [],
  orderContacts: [],
  receivedInspectionNotifications: [],
  orderInspectionSubjects: [],
  orderSettingsExceptions:[],
  projectContacts:[]
}

export const vendorInfoStore = signalStore(
  { providedIn: 'root' },
  withState<BasicVendorInfo>(initialState),

  withMethods((store) => {

    const stateSubject = new Subject<BasicVendorInfo>();

    return {
      SetBasicVendorInfo(basicInfo: any) {
        patchState(store, (state)=>({
          ...state,
          domainId:basicInfo.domainId,
          name:basicInfo.name,
          userName:basicInfo.userName,
          orders:[...basicInfo.orders],
          projects:[...basicInfo.projects],
          inspectionSubjects:[...(basicInfo.inspectionSubjects || [])],
          inspectionTypes: [...(basicInfo.inspectionTypes || [])],
          orderContacts:[...(basicInfo.orderContacts || [])],
          orderInspectionSubjects:[...(basicInfo.orderInspectionSubjects || [])],
          projectContacts:[...(basicInfo.projectContacts || [])],
          orderSettingsExceptions:[...(basicInfo.orderSettingsExceptions || [])],
          isLoading: true,
         // ...getState(store)
        }));
      },

      fetchBasicVendorInfo: (): Observable<BasicVendorInfo> => {
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

