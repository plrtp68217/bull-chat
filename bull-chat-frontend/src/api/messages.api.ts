import apiClient from "./apiClient";
import type { IMessagesResponse } from "./interfaces/messages/IMessagesResponse";

export default {
  async getMessages(messageId: number | null): Promise<IMessagesResponse[]>  {
    const response = await apiClient.post('/message/next-message-page-index', messageId);
    return response.data;
  },
  async uploadImage(formData: FormData) {
    const config = { headers: {'Content-Type': 'multipart/form-data'}}
    const response = await apiClient.post('/message/media/upload-image', formData, config);
    return response.data;
  }
}