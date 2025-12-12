import { BasicPIDECDeptInfo } from './../Core/Models/BasicPIDECDeptInfo.model';

import {patchState, signalStore, withMethods, withState} from '@ngrx/signals'
import { User } from '../Core/Models/User.model';
import { Project } from '../Core/Models/Project.model';
import { Order } from '../Core/Models/Order.model';
import { Vendor } from '../Core/Models/Vendor.model';
import { state } from '@angular/animations';
import { BasicQCMemberInfo } from '../Core/Models/BasicQCMemberInfo.model';
import { BasicVendorInfo } from '../Core/Models/BasicVendorInfo.model';
import { BasicInfoService } from '../Core/Services/basic-info.service';
import { runInInjectionContext,inject } from '@angular/core';
import { BehaviorSubject, catchError, finalize, Observable, of, shareReplay, take, tap } from 'rxjs';
import { UserTypes } from '../Core/Models/UserTypes.enum';
import { Route } from '@angular/router';
import { BasicInspectorInfo } from '../Core/Models/BasicInspectorInfo.model';



interface currentAppState
{
  currentuser: User,
  currentproject: Project,
  currentorder: Order
  vendorData:Vendor,
  vendorBasicInfo : BasicVendorInfo,
  qCMemberBasicInfo:BasicQCMemberInfo,
  pidecDeptBasicInfo :BasicPIDECDeptInfo,
  inspectorBasicInfo:BasicInspectorInfo
}

const initialState :currentAppState = {
  currentuser :{} as User,
  currentorder: {} as Order,
  currentproject:{} as Project,
  vendorData:{} as Vendor,
  vendorBasicInfo:{} as BasicVendorInfo,
  qCMemberBasicInfo:{} as BasicQCMemberInfo,
  pidecDeptBasicInfo:{} as BasicPIDECDeptInfo,
  inspectorBasicInfo :{} as BasicInspectorInfo
};

export const currentstateStore = signalStore(
  {providedIn: 'root'},
  withState<currentAppState>(initialState),
  withMethods((store)=>(
    {
      loggedIn(loggedInuser:User){
          patchState(store, {
            currentuser:{...loggedInuser},
            ...state
      })
      },
       setBasicPIDECDeptInfo(basicPIDECDeptinfo:BasicPIDECDeptInfo)
      {
        patchState(store,{
          pidecDeptBasicInfo:{...basicPIDECDeptinfo},
          ...state
        })
      },
      setBasicQCInfo(basicQCinfo:BasicQCMemberInfo)
      {
        patchState(store,{
          qCMemberBasicInfo:{...basicQCinfo},
          ...state
        })
      },
      setBasicVendorInfo(basicVendorinfo:BasicVendorInfo)
      {
        patchState(store,{
          vendorBasicInfo:{...basicVendorinfo},
          ...state
        })
      },
       setBasicInspectorInfo(basicInspectorinfo:BasicInspectorInfo)
      {
        patchState(store,{
          inspectorBasicInfo:{...basicInspectorinfo},
          ...state
        })
      },

      ChangeProject(FromRoute : string ,basicInfoService: BasicInfoService,project:Project){

        if(store.currentuser().userType == UserTypes.PIDEC_TO || store.currentuser().userType == UserTypes.PIDEC_CC)
        {

        patchState(store ,{
          currentproject: project==null ? store.currentproject():project,
        })
        if(FromRoute.toLowerCase().includes('pidec/orders')){
          this.GetEdmsOrders_ProId(basicInfoService);
        }
        else if(FromRoute.toLowerCase().includes('pidec/projectsettings'))
        {

        }

      }
        console.log('in change prj_order',store.currentproject(),store.currentorder());
      },
      ChangeProject_Order(project:Project,order:Order){
        patchState(store,{
          currentproject:{... project==null ? store.currentproject():project},
          currentorder:{...order==null ? store.currentorder():order},
          ...state
        })
        console.log('in change prj_order',store.currentproject(),store.currentorder());
      },
      GetEdmsOrders_ProId(basicInfoService: BasicInfoService):Observable<Order[]>
      {

                  const resultObservable = basicInfoService.getEdmsOrdersByProjectId(store.currentproject()["fsId"]);
                  resultObservable.subscribe((edmsOrders) => {
                    console.log('Fetched EdmsOrders data:', edmsOrders);
                    patchState(store, (state) => ({
                      ...state,
                      qCMemberBasicInfo:{
                        ...state.qCMemberBasicInfo,
                        eDMSOrders:[...edmsOrders]
                      },
                      isLoading: false
                    }));
                  });

                  return resultObservable;
      },
      // ChangeEdmsOrder(basicInfoService: BasicInfoService,edmsId:string):Order
      // {
       ChangeEdmsOrder(basicInfoService: BasicInfoService,edmsId:string):Observable<Order | null>
      {
        if (!store['__currentOrder$']) {
    store['__currentOrder$'] = new BehaviorSubject<Order | null>(null);
    store['__inFlight$'] = null as Observable<Order | null> | null;
  }

                 const currentOrder$ = store['__currentOrder$'] as BehaviorSubject<Order | null>;
  let inFlight$ = store['__inFlight$'] as Observable<Order | null> | null;

  if (!edmsId) {
    currentOrder$.next(null);
    return of(null);
  }

  // If a request is already running, return the same shared observable
  if (inFlight$) {
    return inFlight$;
  }

  // create shared request; shareReplay ensures single HTTP for multiple subscribers
  inFlight$ = basicInfoService
    .getEdmsOrderDataIfExist(edmsId, store.currentproject().id)
    .pipe(
      tap(order => {
        // update store state via BehaviorSubject
        currentOrder$.next(order);
        // also update any signal-based state you already have
        patchState(store, (s) => ({
          ...s,
          currentorder: order,
          qCMemberBasicInfo: { ...s.qCMemberBasicInfo },
          isLoading: false
        }));
      }),
      catchError(err => {
        // optional: update state on error and rethrow or return null
        currentOrder$.next(null);
        throw err;
      }),
      finalize(() => {
        // clear in-flight when complete/error
        store['__inFlight$'] = null;
      }),
      shareReplay({ bufferSize: 1, refCount: true })
    );

  // store the in-flight observable so concurrent calls reuse it
  store['__inFlight$'] = inFlight$;
  return inFlight$;
},

// helper to expose the BehaviorSubject as Observable
GetCurrentEdmsOrder: (): Observable<Order | null> => {
  if (!store['__currentOrder$']) {
    store['__currentOrder$'] = new BehaviorSubject<Order | null>(null);
  }
  return (store['__currentOrder$'] as BehaviorSubject<Order | null>).asObservable();
},
      SetCurrentOrder(currentOrder:Order)
      {


                    patchState(store, (state) => ({
                      ...state,
                      currentorder:currentOrder,
                      qCMemberBasicInfo:{
                        ...state.qCMemberBasicInfo

                      },
                      isLoading: false
                    }));


      },
      ChangeVendorData(vendorData:Vendor){
        patchState(store,{
          vendorData:vendorData,
          ...state
        })

        console.log('in change vendorData',store.vendorData(),store.currentuser());
      }
    }
  ))
);
