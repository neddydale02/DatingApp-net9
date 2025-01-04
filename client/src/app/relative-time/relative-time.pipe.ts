import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'relativeTime'
})
export class RelativeTimePipe implements PipeTransform {
  transform(value: Date | string | number): string {
    if (!value) return '';

    const now = new Date();
    const date = new Date(value);
    const seconds = Math.floor((now.getTime() - date.getTime()) / 1000);

    if (seconds < 29) return 'Just now';
    const intervals: { [key: string]: number } = {
      'year': 31536000,
      'month': 2592000,
      'week': 604800,
      'day': 86400,
      'hour': 3600,
      'minute': 60,
      'second': 1
    };
    let counter;
    for (const i in intervals) {
      counter = Math.floor(seconds / intervals[i]);
      if (counter > 0) {
        if (counter === 1) {
          return `${counter} ${i} ago`; // singular (1 day ago)
        } else {
          return `${counter} ${i}s ago`; // plural (2 days ago)
        }
      }
    }
    return value.toString();
  }
}