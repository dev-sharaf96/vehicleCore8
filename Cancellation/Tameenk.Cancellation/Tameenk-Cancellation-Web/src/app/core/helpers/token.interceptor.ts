import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { IUserToken } from '../models';

@Injectable({
  providedIn: 'root'
})
export class TokenInterceptor implements HttpInterceptor {

  constructor() { }
  // function which will be called for all http calls
  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const currentUser = JSON.parse(localStorage.getItem('userToken')) as IUserToken;
    if (currentUser && currentUser.access_token) {
      // how to update the request Parameters
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${currentUser.access_token}`
        }
      });
    }
    return next.handle(request).pipe(
      tap(event => event, error => {
        // logging the http response to browser's console in case of a failuer
        if (event instanceof HttpResponse) {
          console.log('api call error :', event);
        }
      }
      )
    );
  }
}
