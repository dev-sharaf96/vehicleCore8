import { Injectable, Injector } from '@angular/core';
import { HttpParams, HttpClient, HttpHeaders } from '@angular/common/http';
import { throwError, Observable } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { LocalizationService } from '.';

@Injectable({
  providedIn: 'root'
})
export abstract class ApiService {
  protected apiUrl = 'https://www.bcare.com.sa/QuotationApiTest/api/';
  _localizationService: LocalizationService;
  _http: HttpClient;

  constructor(private injector: Injector) {
    this._http = injector.get(HttpClient);
    this._localizationService = injector.get(LocalizationService);
  }

  private formatErrors(error: any) {
    return throwError(error.error);
  }

  get<T>(path: string, params = null, hdrs = null): Observable<T> {
    if (typeof params === 'string') {
      params = new HttpParams({ fromString: params });
    }
    const headers = hdrs || this.buildCommonHeader();
    return this._http.get<T>(`${this.apiUrl}${path}`, { headers, params }).pipe(
      tap(data => data, error => error),
      catchError(this.formatErrors)
    );
  }

  post<T>(path: string, body: any, params = null, hdrs = null): Observable<T> {
    params = new HttpParams({ fromString: params });
    const headers = hdrs || this.buildCommonHeader();
    return this._http
      .post<T>(`${this.apiUrl}${path}`, body, { headers, params })
      .pipe(
        tap(data => data, error => error),
        catchError(this.formatErrors)
      );
  }

  protected buildCommonHeader(): HttpHeaders {
    return new HttpHeaders({
      'Access-Control-Allow-Origin':  '*',
      // 'Content-Type':  'application/json',
      // 'Access-Control-Allow-Credentials':  'true',
      // 'Access-Control-Expose-Headers':  'FooBar',
      // 'Access-Control-Allow-Methods': 'GET',
      // 'Access-Control-Allow-Headers': 'Origin, Content-Type',
      // 'X-Requested-With': 'XMLHttpRequest',
      Language: this._localizationService.getCurrentLanguage().id.toString()
    });
  }
}
