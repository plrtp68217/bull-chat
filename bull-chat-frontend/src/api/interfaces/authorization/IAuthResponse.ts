import type { IUser } from "../../../stores/interfaces/IUser";

export interface IAuthResponse {
  user: IUser
  token: string,
}