import { ResolveFn } from '@angular/router';
import { BasicQCMemberInfo } from '../Models/BasicQCMemberInfo.model';
import { inject } from '@angular/core';
import { currentstateStore } from '../../Stores/currentstate.store';
import { catchError, map, Observable, of } from 'rxjs';
import { BasicPIDECDeptInfo } from '../Models/BasicPIDECDeptInfo.model';
import { PIDECDeptInfoStore } from '../../Stores/PIDECDeptInfo.store';

export const pidecdeptbasicinfoResolver: ResolveFn<BasicPIDECDeptInfo> = (route, state,currectAppStore = inject(currentstateStore), pidecDeptinfoStore = inject(PIDECDeptInfoStore)):
 Observable<BasicPIDECDeptInfo> => {

  return pidecDeptinfoStore.fetchBasicPIDECDeptInfo().pipe(
    map((data: BasicPIDECDeptInfo) => {
      if (data && data != null && data != undefined && data.projects?.length > 0) {
        console.log('in resolver when data: ', data);
        currectAppStore.setBasicPIDECDeptInfo(data);
        return data;
      } else {
        console.log('in resolver when null: ', data);
        return null;
      }
    }),
    catchError(error => {
      console.error('Error in resolver: ', error);
      return of(null); // Return null or handle the error as needed
    })
  );
};
