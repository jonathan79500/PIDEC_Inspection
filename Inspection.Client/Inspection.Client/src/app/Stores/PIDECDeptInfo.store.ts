import { BasicPIDECDeptInfo } from './../Core/Models/BasicPIDECDeptInfo.model';
import { getState, patchState, signalStore, withMethods, withState } from "@ngrx/signals";
import { effect, inject } from "@angular/core";
import { UserService } from "../Core/Services/user.service";
import { Observable, of } from "rxjs";



const initialState: BasicPIDECDeptInfo = {
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
isLoading : true
}

export const PIDECDeptInfoStore = signalStore(
  {providedIn:'root'},
  withState<BasicPIDECDeptInfo>(initialState),

  withMethods((store)=>{

    return {
      SetBasicPIDECDeptInfo(basicInfo: any) {
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
         // ...getState(store)
        }));
      },

    fetchBasicPIDECDeptInfo: (): Observable<BasicPIDECDeptInfo> => {
       const currentState = getState(store);
       if (currentState.projects != undefined && currentState.projects.length > 0) {
               // stateSubject.next(currentState);
             console.log('Emitting current state:', currentState);
                return of(currentState);
      } else {
      const userService = inject(UserService);
      const resultObservable =  userService.GetPIDECDeptMemberData();
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
          isLoading:false
        }));

        console.log('in QC Member signal store after patchstate',basicInfo);

      } )
      return resultObservable;
    }
  }
  }
  })
);
