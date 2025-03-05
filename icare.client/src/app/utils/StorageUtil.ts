export class StorageUtil {

  static safeParse<T>(data: string, fallback: T | null = null): T | null {
    try {
      return JSON.parse(data) as T;
    } catch (error) {
      console.error("Failed to parse JSON from localStorage:", error);
      return fallback;
    }
  }

  static saveToStorage<T>(key: string, value: T): void {
    try {
      localStorage.setItem(key, JSON.stringify(value));
    } catch (error) {
      console.error(`Failed to save ${key} to localStorage:`, error);
    }
  }

  static getFromStorage<T>(key: string, fallback: T | null = null): T | null {
    const data = localStorage.getItem(key);
    return data ? this.safeParse<T>(data, fallback) : fallback;
  }
}
