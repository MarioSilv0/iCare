/**
 * @file Defines the `MapKeysPipe` class, a custom Angular pipe that extracts keys from a Map.
 * It is used to dynamically transform Map objects into arrays of keys for easier iteration in templates.
 * 
 * @author João Morais  - 202001541
 * @author Luís Martins - 202100239
 * @author Mário Silva  - 202000500
 * 
 * @date Last Modified: 2025-03-01
 */

import { Pipe, PipeTransform } from '@angular/core';

/**
  * The `MapKeysPipe` class is an Angular pipe that transforms a `Map` into an array of its keys.
  * This allows for easier iteration over a `Map` in Angular templates.
  */
@Pipe({
  name: 'mapKeys',
  pure: false // So the UI updates dynamically
})
export class MapKeysPipe implements PipeTransform {

  /**
   * Transforms a given `Map` into an array containing its keys.
   * 
   * @param {Map<any, any>} map - The `Map` to extract keys from.
   * @returns {any[]} An array containing the keys of the given `Map`.
   */
  transform(map: Map<any, any>): any[] {
    return Array.from(map.keys());
  }
}
