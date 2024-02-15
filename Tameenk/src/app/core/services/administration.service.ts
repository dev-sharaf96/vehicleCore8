import { Injectable, Injector } from '@angular/core';
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment";
import { ApiService } from "./api.service";
import { CommonResponse, IInsuranceCompany } from '..';

@Injectable()
export class AdministrationService extends ApiService {

  constructor(private _injector: Injector) {
    super(_injector);
    this.apiUrl = environment.administrationApiUrl + "insurance-company/";
  }

  getInsuranceCompanies(): Observable<CommonResponse<IInsuranceCompany[]>> {
    return super.get<CommonResponse<IInsuranceCompany[]>>('all');
  }
}
