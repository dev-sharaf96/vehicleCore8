import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { AuthenticationService } from './authentication.service';
import { Observable } from 'rxjs';
import { retry, tap } from 'rxjs/operators';


@Injectable()
export class StartupService {
    constructor(private _authenticationService: AuthenticationService) {
    }
    load(): Promise<any> {
        return this._authenticationService.getAccessToken();
    }
}