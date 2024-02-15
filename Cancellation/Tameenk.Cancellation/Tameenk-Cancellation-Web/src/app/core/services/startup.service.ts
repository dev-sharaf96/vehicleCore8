import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class StartupService {

  constructor(private _authService: AuthService) {
  }
  load(): Promise<any> {
    if (this._authService.getUserId()) {
      this._authService.isAuthenticated().subscribe(v => {
        this._authService.currentUser$.next(this._authService.getUserId());
        if (!v.data) {
          return this._authService.getAccessToken(this._authService.getUserId());
        }
      }, (err) => {
        this._authService.deleteUserId();
        return this._authService.getAccessToken();
      });
    } else {
      return this._authService.getAccessToken();
    }
  }
}
