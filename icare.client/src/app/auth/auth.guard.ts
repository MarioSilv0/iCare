import { CanActivateFn } from '@angular/router';
import { AuthService } from './auth.service';
import { inject } from '@angular/core';
import { Router } from '@angular/router';


//Mário
export const AuthGuard: CanActivateFn = (): boolean => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const state = authService.isAuthenticated().subscribe((isAuthenticated: boolean) => isAuthenticated );
  if (!state) {
      router.navigate(['/login']);
      console.log(false);
      return false;
    }
    console.log(true);
    return true;
}

