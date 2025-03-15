/**
 * @file Defines the `AuthInterceptor` class, responsible for intercepting HTTP requests
 * and appending an authorization token when required. Requests to specific APIs bypass authentication.
 * 
 * @author João Morais  - 202001541
 * @author Luís Martins - 202100239
 * @author Mário Silva  - 202000500
 * 
 * @date Last Modified: 2025-03-01
 */

import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';

/**
 * `AuthInterceptor` class implements the `HttpInterceptor` interface,
 * modifying outgoing HTTP requests by appending an authorization token when necessary.
 * Requests to specified APIs are left unmodified.
 */
@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor() { }

  /**
   * Intercepts outgoing HTTP requests to check if an authentication token should be appended.
   * If the request is targeting certain APIs, the authorization header is not added.
   * Otherwise, if a valid token is found in local storage, it is included in the request headers.
   *
   * @param {HttpRequest<any>} request - The HTTP request being intercepted.
   * @param {HttpHandler} next - The HTTP handler that processes the request.
   * @returns {Observable<HttpEvent<any>>} An observable representing the processed HTTP request.
   */
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (request.url.includes('themealdb.com')
      || request.url.includes('localhost:4000/graphql')
      || request.url.includes('127.0.0.1:5000/translate')
    ) {
      return next.handle(request); // Não adiciona Authorization para esta API, Mário
    }

    const token = localStorage.getItem('authToken');

    if (token) {
      request = request.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
    }

    return next.handle(request);
  }
}
