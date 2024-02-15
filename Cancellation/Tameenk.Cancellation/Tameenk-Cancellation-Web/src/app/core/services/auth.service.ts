import { Injectable, Injector } from '@angular/core';
import { ApiService } from './api.service';
import { environment } from 'src/environments/environment';
import { Subject, Observable } from 'rxjs';
import { IUserToken, CommonResponse } from '../models';
@Injectable({
  providedIn: 'root'
})
export class AuthService extends ApiService {
  _localStorageUserTokenKey = 'userToken';
  _localStorageUserIdKey = 'userId';
  _localStorageRememberMe = 'rememberMe';
  currentUser$: Subject<string> = new Subject<string>();
  constructor(private _injector: Injector) {
    super(_injector);
    this.apiUrl = environment.identityApiUrl;
  }
  getAccessToken(userId?): Promise<IUserToken> {
    const token = this.getUserToken();
    if (token != null) {
      const currentDate = new Date();
      const expiryDate = new Date(token.expiryDate);
      if (expiryDate.getTime() > currentDate.getTime()) {
        const diff = Math.floor((expiryDate.getTime() - currentDate.getTime()));
        if (diff > 0) {
          setTimeout(() => {
            this.getAccessToken(userId);
          }, diff);
          return new Promise<IUserToken>(resolve => {
            resolve(token);
          });
        }
      }
    }
    return this._http.post<CommonResponse<IUserToken>>(environment.identityApiUrl + 'identity/GetAccessToken', {'UserId': userId})
      .toPromise()
      .then(data => {
        if (data && data.data.access_token) {
          setTimeout(() => {
            this.getAccessToken(userId);
          }, data.data.expires_in * 1000);
          data.data.expiryDate = new Date();
          data.data.expiryDate.setSeconds(data.data.expiryDate.getSeconds() + data.data.expires_in);
          localStorage.setItem(this._localStorageUserTokenKey, JSON.stringify(data.data));
        }
        return data.data;
      });
  }
  getUserToken(): IUserToken {
    return JSON.parse(localStorage.getItem(this._localStorageUserTokenKey)) as IUserToken;
  }
  deleteUserToken() {
    // remove user from local storage to log user out
    localStorage.removeItem(this._localStorageUserTokenKey);
  }
  getUserId(): string {
    return localStorage.getItem(this._localStorageUserIdKey) as string;
  }
  setUserId(userId) {
    localStorage.setItem(this._localStorageUserIdKey, userId);
    this.currentUser$.next(userId);
  }
  deleteUserId() {
    this.deleteUserToken();
    localStorage.removeItem(this._localStorageUserIdKey);
    this.currentUser$.next(undefined);
  }
  addRememberMe(value) {
    localStorage.setItem(this._localStorageRememberMe, value);
  }
  hasRememberMe() {
    return localStorage.getItem(this._localStorageRememberMe) === 'true' ? true : false;
  }
  isAuthenticated(): Observable<CommonResponse<boolean>> {
    return super.get<CommonResponse<boolean>>('identity/IsAuthenticated');
  }
}
