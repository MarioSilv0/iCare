import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'mapKeys',
  pure: false // So the UI updates dynamically
})
export class MapKeysPipe implements PipeTransform {
  transform(map: Map<any, any>): any[] {
    return Array.from(map.keys());
  }
}
