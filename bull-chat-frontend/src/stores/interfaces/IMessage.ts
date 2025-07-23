import type { IUser } from "./IUser";

export type IMessageAuthor = 'me' | 'not_me';

// [???] Этот синтаксис недопустим, если включен параметр "erasableSyntaxOnly"
// export enum MessageAuthor {
//   Me,
//   NotMe,
// }

export interface IMessage {
  user: IUser,
  content:  {
    item: string,
    contentType: number,
  };
  date: Date;
  id: number;
}