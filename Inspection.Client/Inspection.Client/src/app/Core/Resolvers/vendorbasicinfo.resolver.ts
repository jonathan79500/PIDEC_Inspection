import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { currentstateStore } from '../../Stores/currentstate.store'; // Update with your actual import
import { vendorInfoStore } from '../../Stores/vendorInfo.store'; // Update with your actual import
import { BasicVendorInfo } from '../Models/BasicVendorInfo.model'; // Update with your actual import

export const vendorbasicinfoResolver: ResolveFn<BasicVendorInfo> = (route, state, currectAppStore = inject(currentstateStore), vendorinfoStore = inject(vendorInfoStore)) :
  Observable<BasicVendorInfo> => {
    console.log('Resolver invoked');

  return vendorinfoStore.fetchBasicVendorInfo().pipe(
    map((data: BasicVendorInfo) => {
      console.log('Data in map:', data);
      if (data && data != null && data != undefined && data.orders?.length > 0) {
        console.log('in resolver when data: ', data);
        currectAppStore.setBasicVendorInfo(data);
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
