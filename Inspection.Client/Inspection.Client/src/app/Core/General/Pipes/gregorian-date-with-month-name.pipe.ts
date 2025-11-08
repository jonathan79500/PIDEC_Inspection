import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'gregorianDateWithMonthName'
})
export class GregorianDateWithMonthNamePipe implements PipeTransform {
  transform(value: Date | string): string {
    if (!value) {
      return '';
    }

    const date = new Date(value);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: '2-digit'
    });
  }
}
