import { Inject, inject, signal } from '@angular/core';
import { CanActivateFn,Router } from '@angular/router';
import { Observable } from 'rxjs';

export const authGuard: CanActivateFn = (route, state):boolean => {

  let check = signal<boolean>(false);
  const router= Inject(Router);
  if(check()==true)
    return true;
  else
  { 
  router.navigate(['notfound']);
  return false;
  } 
   
};
