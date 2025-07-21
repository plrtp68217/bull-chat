import { ref } from 'vue';
import type { MessageApi } from 'naive-ui';
import * as signalR from '@microsoft/signalr';
import { createHubConnection } from './config';
import { setupListeners, removeListeners } from './events';
import type { HubInvokeEvents } from './interfaces';

export const useChatHub = () => {
  const connection = ref<signalR.HubConnection | null>(null);
  const isConnected = ref(false);

  const start = async (token: string, flash: MessageApi) => {
    try {
      connection.value = createHubConnection(token);
      setupListeners(connection.value as signalR.HubConnection);
      
      await connection.value.start();
      isConnected.value = true;

      flash.success("Подключение установлено!")
    }
    catch (error) {
      flash.error("Подключение разорвано, попытка переподключения...")
      setTimeout(() => start(token, flash), 5000);
    }
  };

  const stop = async (flash: MessageApi) => {
    if (connection.value) {
      removeListeners(connection.value as signalR.HubConnection);
      await connection.value.stop();
      connection.value = null;
      isConnected.value = false;
      flash.warning("Вы отключились");
    }
  };

  const invoke: HubInvokeEvents = {
    SendMessage: async (userId: string, content: string) => {
      if (connection.value?.state === signalR.HubConnectionState.Connected) {
        await connection.value.invoke('SendMessage', userId, content);
      }
    },
  };

  return {
    connection,
    isConnected,
    start,
    stop,
    invoke
  };
};