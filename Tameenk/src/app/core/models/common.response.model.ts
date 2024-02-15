import { Error } from '.';

/** The common response model */
export class CommonResponse<T> {
  /** The result data */
  data: T;

  /** The list of error models */
  errors: Error[];

  /** The total count of result from the source. */
  totalCount: number;
}
