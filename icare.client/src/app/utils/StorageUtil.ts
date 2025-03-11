/**
 * @file Defines the `StorageUtil` class, which provides utility methods 
 * for safely interacting with `localStorage`, including storing, retrieving, 
 * and parsing JSON data.
 * 
 * @author João Morais  - 202001541
 * @author Luís Martins - 202100239
 * @author Mário Silva  - 202000500
 * 
 * @date Last Modified: 2025-03-05
 */

/**
 * The `StorageUtil` class provides static utility methods to safely handle
 * local storage operations, including parsing, saving, and retrieving JSON data.
 */
export class StorageUtil {

  /**
    * Safely parses a JSON string into a TypeScript object.
    * If parsing fails, it logs an error and returns a fallback value.
    * 
    * @template T - The expected type of the parsed object.
    * @param {string} data - The JSON string to be parsed.
    * @param {T | null} [fallback=null] - A fallback value to return in case of a parsing error.
    * @returns {T | null} The parsed object if successful, otherwise the fallback value.
    */
  static safeParse<T>(data: string, fallback: T | null = null): T | null {
    try {
      return JSON.parse(data) as T;
    } catch (error) {
      console.error("Failed to parse JSON from localStorage:", error);
      return fallback;
    }
  }

  /**
    * Saves a key-value pair to `localStorage` after converting the value to a JSON string.
    * If storing fails (e.g., due to storage quota exceeded), it logs an error.
    * 
    * @template T - The type of the value being stored.
    * @param {string} key - The key under which the value will be stored.
    * @param {T} value - The value to store in `localStorage`.
    */
  static saveToStorage<T>(key: string, value: T): void {
    try {
      localStorage.setItem(key, JSON.stringify(value));
    } catch (error) {
      console.error(`Failed to save ${key} to localStorage:`, error);
    }
  }

  /**
   * Retrieves a value from `localStorage` and safely parses it as JSON.
   * If the key does not exist or parsing fails, it returns a fallback value.
   * 
   * @template T - The expected type of the stored object.
   * @param {string} key - The key to retrieve from `localStorage`.
   * @param {T | null} [fallback=null] - A fallback value to return if the key is not found or parsing fails.
   * @returns {T | null} The retrieved and parsed object, or the fallback value if an error occurs.
   */
  static getFromStorage<T>(key: string, fallback: T | null = null): T | null {
    const data = localStorage.getItem(key);
    return data ? this.safeParse<T>(data, fallback) : fallback;
  }
}
