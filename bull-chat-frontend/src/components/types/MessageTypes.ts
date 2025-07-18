export type MessageAuthor = 'me' | 'not_me';

// [???] Этот синтаксис недопустим, если включен параметр "erasableSyntaxOnly"
// export enum MessageAuthor {
//   Me,
//   NotMe,
// }
export interface Message {
  text: string;
  sender: MessageAuthor;
  timestamp: Date;
}