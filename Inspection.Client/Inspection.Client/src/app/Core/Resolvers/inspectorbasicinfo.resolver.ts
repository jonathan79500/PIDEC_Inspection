import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { currentstateStore } from '../../Stores/currentstate.store'; // Update with your actual import
import { BasicInspectorInfo } from '../Models/BasicInspectorInfo.model';
import { InspectorInfoStore } from '../../Stores/InspectorInfo.store';

export const inspectorbasicinfoResolver: ResolveFn<BasicInspectorInfo> = (route, state, currectAppStore = inject(currentstateStore), inspectorinfoStore = inject(InspectorInfoStore)) :
  Observable<BasicInspectorInfo> => {
    console.log('Resolver invoked');

  return inspectorinfoStore.fetchBasicInspectorInfo().pipe(
    map((data: BasicInspectorInfo) => {
      console.log('Data in map:', data);
      if (data && data != null && data != undefined && data.orders?.length > 0) {
        console.log('in resolver when data: ', data);
        currectAppStore.setBasicInspectorInfo(data);
        return data;
      } else {
        console.log('in resolver when null: ', data);
        return null;
      }
    })
    ,
    catchError(error => {
      console.error('Error in resolver: ', error);
      return of(null); // Return null or handle the error as needed
    })
  );
};
