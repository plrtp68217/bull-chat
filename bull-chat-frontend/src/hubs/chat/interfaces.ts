export interface IMessageDto {
  userId: number,
  content: string,
}

export interface HubInvokeEvents {
  SendMessage: (userId: string, content: string) => Promise<void>;
}

