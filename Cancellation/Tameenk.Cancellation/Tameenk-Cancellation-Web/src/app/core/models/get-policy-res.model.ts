import { Policy } from './policy.model';

export class GetPolicyRes {
  ReferenceId: string;
  StatusCode: number;
  RequestNo: string;
  RequestExpiryDate: Date;
  Policies: Policy[];
}
