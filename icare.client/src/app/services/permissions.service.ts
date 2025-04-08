
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { StorageUtil } from '../utils/StorageUtil';
import { PROFILE } from './users.service';
import { Permissions } from '../../models'


@Injectable({
  providedIn: 'root'
})
export class PermissionsService {
  
  constructor(private http: HttpClient) { }

  /**
   * Retrieves the user's permissions from `localStorage`.
   * 
   * @returns {Permissions | null} The stored permissions if available, otherwise `null`.
   */
  getPermissions(): Permissions | null {
    return StorageUtil.getFromStorage<Permissions>('permissions');
  }

  /**
   * Fetches the user's permissions from the API and stores them in `localStorage`.
   * 
   * @returns {Observable<boolean>} An observable that emits the retrieved permission status.
   */
  fetchPermissions(): Observable<Permissions> {
    return this.http.get<Permissions>(`${PROFILE}/permissions`).pipe(
      tap(permissions => this.setPermissions(permissions))
    );
  }

  setPermissions(updatedPermissions: Partial<Permissions>): void {
    const current = this.getPermissions() ?? {};
    const newPermissions = { ...current, ...updatedPermissions };

    StorageUtil.saveToStorage('permissions', newPermissions);
  }
}
