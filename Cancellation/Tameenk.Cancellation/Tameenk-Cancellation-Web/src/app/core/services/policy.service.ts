import { Injectable, Injector } from '@angular/core';
import { ApiService } from './api.service';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { GetPolicyRes, Reason, Bank } from '../models';
import { InsuranceCompany } from '../models/insurance-company.model';

@Injectable({
  providedIn: 'root'
})
export class PolicyService extends ApiService {
  constructor(private _injector: Injector) {
    super(_injector);
    this.apiUrl = environment.policyApiUrl;
  }

  getPolicies(body): Observable<GetPolicyRes> {
    return super.post<GetPolicyRes>('', body);
  }

  getReasons(): Observable<Reason[]> {
    return super.get<Reason[]>('lookups/getReasons', null);
  }
  getInsuranceCompanies(): Observable<InsuranceCompany[]> {
    return super.get<InsuranceCompany[]>('lookups/getInsuranceCompanies', null);
  }
  getAllInsuranceCompanies(): Observable<InsuranceCompany[]> {
    return super.get<InsuranceCompany[]>('insuranceCompanies', null);
  }
  addInsuranceCompany(body): Observable<InsuranceCompany> {
    return super.post<InsuranceCompany>('insuranceCompanies', body);
  }
  getInsuranceCompany(code): Observable<InsuranceCompany> {
    return super.post<InsuranceCompany>('InsuranceCompany', `bankCode=${code}`);
  }
  getAllBanks(): Observable<Bank[]> {
    return super.get<Bank[]>('BankCodes', null);
  }
  addBank(body): Observable<Bank> {
    return super.post<Bank>('BankCodes', body);
  }
  getBank(code): Observable<Bank> {
    return super.get<Bank>('BankCodes', `bankCode=${code}`);
  }
  addReasons(body): Observable<Reason> {
    return super.post<Reason>('BankCodes', body);
  }
}
