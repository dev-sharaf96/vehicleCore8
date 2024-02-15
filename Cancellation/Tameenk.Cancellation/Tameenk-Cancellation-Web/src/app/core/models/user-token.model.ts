export interface IUserToken {
  access_token: string;
  token_type: string;
  expires_in: number;
  client_id: string;
  userName: string;
  expiryDate: Date;
}
