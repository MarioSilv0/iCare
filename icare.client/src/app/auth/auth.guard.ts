import { ActivatedRouteSnapshot, CanActivateFn } from '@angular/router';
import { AuthService } from './auth.service';
import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";
import { Observable, map } from "rxjs";

@Injectable({ providedIn: 'root' })

//Mário
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const isLogged = this.authService.isLogged();
    if (!isLogged) {
      this.router.navigate(['/login']);
      return false;
    }
    const expectedRoles = route.data['roles'];
    if (expectedRoles){
      const userRoles = this.authService.getUserRoles();
      if (!userRoles || !expectedRoles.includes(userRoles)) {
        this.router.navigate(['/home']);
        return false;
      }
    }

    return true;
  }

}
