import type { IMessage } from "../../stores/interfaces/IMessage";
import { useMessagesStore } from "../../stores/messages";

export const setupListeners = (connection: signalR.HubConnection) => {
  connection.on('ReceiveMessage', (message: IMessage) => {
    const messages = useMessagesStore();
    messages.addMessage(message);
  });

  connection.onclose((error) => {
    console.error('Connection closed:', error);
  });
};

export const removeListeners = (connection: signalR.HubConnection) => {
  connection.off('ReceiveMessage');
};