import apiClient from "./apiClient";
import type { IMessagesResponse } from "./interfaces/messages/IMessagesResponse";

export default {
  async getMessages(messageId: number | null): Promise<IMessagesResponse[]>  {
    const response = await apiClient.post('/message/next-message-page-index', messageId);
    return response.data;
  },
}