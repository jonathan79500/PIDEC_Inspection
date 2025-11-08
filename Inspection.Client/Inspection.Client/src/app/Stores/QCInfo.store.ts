import { getState, patchState, signalStore, withMethods, withState } from "@ngrx/signals";

import { effect, inject } from "@angular/core";
import { UserService } from "../Core/Services/user.service";
import { BasicQCMemberInfo } from "../Core/Models/BasicQCMemberInfo.model";
import { Observable, of } from "rxjs";



const initialState: BasicQCMemberInfo = {
name:'' ,
userName:'',
domainId:0,
inspectionTypes:[],
inspectionSubjects:[],
projects:[],
eDMSOrders:[],
orderMCContactPersons:[],
orderOwnerContactPersons:[],
orderInspectionSubjects:[],
orders:[],
participated_Circulations:[],
receivedInspectionNotifications:[],
isLoading : true,
usefulContacts :[]
}

export const QCInfoStore = signalStore(
  {providedIn:'root'},
  withState<BasicQCMemberInfo>(initialState),

  withMethods((store)=>{

    return {
      SetBasicQCInfo(basicInfo: any) {
        patchState(store, (state)=>({
          ...state,
          domainId:basicInfo.domainId,
          name:basicInfo.name,
          userName:basicInfo.userName,
          eDMSOrders:[...(basicInfo.eDMSOrders || [])],
          orders:[...(basicInfo.orders || [])],
          projects:[...(basicInfo.projects || [])],
          inspectionSubjects:[...(basicInfo.inspectionSubjects || [])],
          inspectionTypes: [...(basicInfo.inspectionTypes || [])],
          orderMCContactPersons:[...(basicInfo.orderMCContactPersons || [])],
          orderOwnerContactPersons:[...(basicInfo.orderOwnerContactPersons || [])],
          participated_Circulations:[...(basicInfo.participated_Circulations || [])],
          receivedInspectionNotifications:[...(basicInfo.receivedInspectionNotifications || [])],
          isLoading: true,
          usefulContacts:[...(basicInfo.usefulContacts || [])]
         // ...getState(store)
        }));
      },

    fetchBasicQCMemberInfo: (): Observable<BasicQCMemberInfo> => {
       const currentState = getState(store);
       if (currentState.projects != undefined && currentState.projects.length > 0) {
               // stateSubject.next(currentState);
             console.log('Emitting current state:', currentState);
                return of(currentState);
      } else {
      const userService = inject(UserService);
      const resultObservable =  userService.GetQCMemberData();
      resultObservable.subscribe((basicInfo)=>
      {
        console.log('before ng init in QC store',basicInfo);
        patchState(store,(state)=>({

          ...state,
          receivedInspections:basicInfo.receivedInspections,
          name:basicInfo.name,
          userName:basicInfo.userName,
          domainId:basicInfo.domainId,
          inspectionTypes:basicInfo.inspectionTypes,
          projects: basicInfo.projects,
          isLoading:false,
          usefulContacts: basicInfo.usefulContacts
        }));

        console.log('in QC Member signal store after patchstate',basicInfo);

      } )
      return resultObservable;
    }
  }
  }
  })
);
