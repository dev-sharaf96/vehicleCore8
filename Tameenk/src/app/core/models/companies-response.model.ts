import { IInsuranceCompany } from "./insurance-company.model";

export interface ICompaniesResponse {
  data: IInsuranceCompany[];
  totalCount: number;
  errors: IError[];
}

interface IError {
  code: string;
  description: string;
}
