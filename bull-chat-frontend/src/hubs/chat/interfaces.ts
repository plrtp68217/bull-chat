export interface IMessageDto {
  userId: number,
  content: string,
}

export interface HubInvokeEvents {
  SendMessage: (content: string) => Promise<void>;
}

