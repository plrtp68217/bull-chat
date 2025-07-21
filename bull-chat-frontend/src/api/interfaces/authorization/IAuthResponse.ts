import type { IUser } from "../../../stores/interfaces/IUser";

export interface IAuthResponse {
  token: string,
  user: IUser
}