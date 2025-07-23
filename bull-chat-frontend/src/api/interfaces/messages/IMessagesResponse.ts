import type { IUser } from "../../../stores/interfaces/IUser";

export interface IMessagesResponse {
  date: Date,
  user: IUser,
  content: {
    item: string,
    contentType: number
  },
  id: number,
}