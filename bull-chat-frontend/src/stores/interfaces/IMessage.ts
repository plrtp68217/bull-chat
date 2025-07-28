import type { IUser } from "./IUser";

export type IMessageAuthor = 'me' | 'not_me';

export interface IMessage {
  user: IUser,
  content:  {
    item: string,
    contentType: number,
  };
  date: Date;
  id: number;
}