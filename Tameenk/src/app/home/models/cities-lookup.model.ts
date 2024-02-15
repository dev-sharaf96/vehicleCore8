export interface ICitiesLookup {
  data: IData[];
  totalCount: number;
  errors: IError[];
}
interface IData {
  code: number;
  englishDescription: string;
  arabicDescription: string;
  name: string;
}
interface IError {
  code: string;
  description: string;
}
