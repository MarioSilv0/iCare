import { CanActivateFn } from '@angular/router';
import { AuthService } from './auth.service';
import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";
import { Observable, map } from "rxjs";

@Injectable({ providedIn: 'root' })

//Mário
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) { }

  canActivate(): boolean {
    const isLogged = this.authService.isLogged();
    if (!isLogged) {
      this.router.navigate(['/login']);
    }
    return isLogged;
  }

}
