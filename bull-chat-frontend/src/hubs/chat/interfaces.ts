export interface IMessageDto {
  userId: number,
  content: string,
}

export interface HubInvokeEvents {
  SendMessage: (userId: number, content: string) => Promise<void>;
}

