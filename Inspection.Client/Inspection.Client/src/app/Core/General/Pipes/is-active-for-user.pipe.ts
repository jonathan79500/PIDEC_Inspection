import { Pipe, PipeTransform } from '@angular/core';
import { Circulation } from '../../Models/Circulation.model';

@Pipe({
  name: 'isActiveForUser',
  pure: true
})
export class IsActiveForUserPipe implements PipeTransform {
  transform(circulation: Circulation | null, currentCirculation: Circulation | null): boolean {
    // if either circulation or currentCirculation is null, return false
    if (!circulation || !currentCirculation) {
      return false;
    }

    const performer1 = circulation.performerContactPerson;
    const performer2 = currentCirculation.performerContactPerson;

    return (
      performer1 != null &&
      performer2 != null &&
      performer1.id === performer2.id &&
      (circulation.performedActionDateGregorian == null || circulation.performedActionDateGregorian.toString().trim() ==='0001-01-01T00:00:00')
    );
  }
}

