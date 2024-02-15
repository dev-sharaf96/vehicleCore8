import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HttpCancelService {

  private cancelPendingRequests$ = new Subject();
  pendingRequestsUrl: string;

  constructor() { }

  /** Cancels all pending Http requests. */
  public cancelPendingRequests() {
    this.cancelPendingRequests$.next();
  }

  public onCancelPendingRequests() {
    return this.cancelPendingRequests$;
  }
}
