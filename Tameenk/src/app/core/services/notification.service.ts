import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private notification = new Subject<any>();

  constructor() { }

  error(message: string) {
    this.notification.next({ text: message, status: 'error' });
    
  }
  success(message: string) {
    this.notification.next({ text: message, status: 'success' });

  }
  clearMessage() {
    this.notification.next();
  }

  getMessage(): Observable<any> {
    return this.notification.asObservable();
  }
}
