import apiClient from "./apiClient";
import type { IMessagesDto } from "./interfaces/messages/IMessagesDto";
import type { IMessagesResponse } from "./interfaces/messages/IMessagesResponse";

export default {
  async getMessages(dto: IMessagesDto): Promise<IMessagesResponse[]>  {
    const response = await apiClient.post('/message/next-message-page', dto);
    return response.data;
  },
}