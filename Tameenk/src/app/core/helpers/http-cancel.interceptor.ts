import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/takeUntil';
import { HttpCancelService } from '..';

@Injectable({
  providedIn: 'root'
})
export class HttpCancelInterceptor implements HttpInterceptor {
  HttpUrl: string;
  constructor(private _httpCancelService: HttpCancelService) { }
  intercept<T>(req: HttpRequest<T>, next: HttpHandler): Observable<HttpEvent<T>> {
    this.HttpUrl = this._httpCancelService.pendingRequestsUrl;
    if (req.url.indexOf(this.HttpUrl) !== -1) {
      return next.handle(req).takeUntil(this._httpCancelService.onCancelPendingRequests());
    }
    return next.handle(req);
  }
}
