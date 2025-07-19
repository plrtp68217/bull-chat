export interface IAuthResponse {
  token: string,
  user: {
    id: number,
    name: string
  }
}