export class ICommonLookup {
    data: LookupData[];
    totalCount: number;
    errors: IError[];
  }
  export class LookupData {
    id: number;
    name: string;
  }
  interface IError {
    code: string;
    description: string;
  }