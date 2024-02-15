import { Injectable, Injector, OnDestroy } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { map, retry, tap, window } from 'rxjs/operators';
import { ApiService } from './api.service';
import { environment } from '../../../environments/environment';
import { UserToken } from '../models/user-token.model';
import { Observable } from 'rxjs';
import { ICaptcha, CommonResponse } from '../models';

@Injectable()
export class AuthenticationService extends ApiService implements OnDestroy {

    ngOnDestroy(): void {
        this.logout();
    }
    _localStorageUserTokenKey: string = "userToken";
    /**
     * Create instance of AuthenticationService
     * @constructor
     */
    constructor(private _injector: Injector) {
        super(_injector);
        this.apiUrl = environment.identityUrl;
    }

    getAccessToken(): Promise<UserToken> {
        let token = this.getUserToken();
        if (token != null) {
            var currentDate = new Date();
            let expiryDate = new Date(token.expiryDate);
            if (expiryDate.getTime() > currentDate.getTime()) {
                var diff = Math.floor((expiryDate.getTime() - currentDate.getTime()));
                if (diff > 0) {
                    setTimeout(() => {
                        this.getAccessToken();
                    }, diff);
                    return new Promise<UserToken>(resolve => {
                        resolve(token);
                    });
                }
            }
           
        }

        const headers = this.buildCommonHeader();
        return this._http.get<UserToken>(environment.accessTokenUrl, { headers })
        .toPromise()
        .then(userToken => {
            if (userToken && userToken.access_token) {
                setTimeout(() => {
                    this.getAccessToken();
                }, userToken.expires_in * 1000);
                userToken.expiryDate = new Date();
                userToken.expiryDate.setSeconds(userToken.expiryDate.getSeconds() + userToken.expires_in);
                localStorage.setItem(this._localStorageUserTokenKey, JSON.stringify(userToken));
            }
            return userToken;
        });
    }

    getUserToken(): UserToken {
        return JSON.parse(localStorage.getItem(this._localStorageUserTokenKey)) as UserToken;
    }
    getFormUrlEncoded(toConvert) {
        const formBody = [];
        for (const property in toConvert) {
            const encodedKey = encodeURIComponent(property);
            const encodedValue = encodeURIComponent(toConvert[property]);
            formBody.push(encodedKey + '=' + encodedValue);
        }
        return formBody.join('&');
    }
    /**
     * logout
     * @method
     * @memberOf {AuthenticationService}
     */
    logout() {
        // remove user from local storage to log user out
        localStorage.removeItem(this._localStorageUserTokenKey);
    }
    getCaptcha() {
        return super.get<CommonResponse<ICaptcha>>("identity/captcha", null, new HttpHeaders({
            'Cache-Control': 'no-cache, no-store, must-revalidate',
            'Pragma': 'no-cache',
            'Expires': 'Sat, 01 Jan 2000 00:00:00 GMT',
            'Language': this._localizationService.getCurrentLanguage().id.toString()
        }));
    }
    validateCaptcha(body) {
        return super.post<CommonResponse<boolean>>("identity/captcha/validate", body);
    }
}