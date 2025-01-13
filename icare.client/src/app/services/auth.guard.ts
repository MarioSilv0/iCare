import { CanActivateFn } from '@angular/router';
import { AuthService } from './auth.service';  // Importando o AuthService
import { inject } from '@angular/core';
import { Router } from '@angular/router';


//Mário
export const AuthGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);  // Usando inject para acessar o AuthService
  const router = inject(Router);  // Usando inject para acessar o Router

  if (authService.isAuthenticated()) {
    return true; 
  } else {
    router.navigate(['/login']); 
    return false; 
  }
};

