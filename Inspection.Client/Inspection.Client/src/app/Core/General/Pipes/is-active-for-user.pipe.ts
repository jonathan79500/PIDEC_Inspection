import { Pipe, PipeTransform } from '@angular/core';
import { Circulation } from '../../Models/Circulation.model';


@Pipe({
  name: 'isActiveForUser',
  pure: true // keeps it performant; change to false if inputs are dynamic and need re-evaluation
})
export class IsActiveForUserPipe implements PipeTransform {
  transform(circulation: Circulation, currentCirculation: Circulation): boolean {
    return (
      circulation.performerContactPerson.id === currentCirculation.performerContactPerson.id &&
      (circulation.actionType == null)
    );
  }
}
