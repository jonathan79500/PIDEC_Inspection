import { ResolveFn } from '@angular/router';
import { BasicQCMemberInfo } from '../Models/BasicQCMemberInfo.model';
import { inject } from '@angular/core';
import { currentstateStore } from '../../Stores/currentstate.store';
import { QCInfoStore } from '../../Stores/QCInfo.store';
import { catchError, map, Observable, of } from 'rxjs';

export const qCMembersbasicinfoResolver: ResolveFn<BasicQCMemberInfo> = (route, state,currectAppStore = inject(currentstateStore), qcMemberinfoStore = inject(QCInfoStore)):
 Observable<BasicQCMemberInfo> => {
  
  return qcMemberinfoStore.fetchBasicQCMemberInfo().pipe(
    map((data: BasicQCMemberInfo) => {
      if (data && data != null && data != undefined && data.projects?.length > 0) {
        console.log('in resolver when data: ', data);
        currectAppStore.setBasicQCInfo(data);
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