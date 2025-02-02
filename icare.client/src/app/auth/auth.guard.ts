import { ActivatedRouteSnapshot, CanActivateFn } from '@angular/router';
import { AuthService } from './auth.service';
import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";

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
    const expectedRoles: string = route.data['roles'];
    if (!this.authService.userHasRole(expectedRoles)) {
      this.router.navigate(['/home']);
      return false;
    }
    
    return true;
  }
}
