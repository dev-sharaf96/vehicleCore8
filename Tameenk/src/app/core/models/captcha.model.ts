export interface ICaptcha {
  image: string;
  token: string;
  expiredInSeconds: number;
}