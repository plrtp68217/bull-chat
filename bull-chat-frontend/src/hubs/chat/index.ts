import { ref } from 'vue';
import * as signalR from '@microsoft/signalr';
import { createHubConnection } from './config';
import { setupListeners, removeListeners } from './events';
import type { HubInvokeEvents } from './interfaces';

export const useChatHub = () => {
  const connection = ref<signalR.HubConnection | null>(null);
  const isConnected = ref(false);

  const start = async (token: string) => {
    try {
      connection.value = createHubConnection(token);
      setupListeners(connection.value as signalR.HubConnection);
      
      await connection.value.start();
      isConnected.value = true;
    }
    catch (error) {
      console.log('Connection failed:', error);
      setTimeout(() => start(token), 5000);
    }
  };

  const stop = async () => {
    if (connection.value) {
      removeListeners(connection.value as signalR.HubConnection);
      await connection.value.stop();
      connection.value = null;
      isConnected.value = false;
    }
  };

  const invoke: HubInvokeEvents = {
    SendMessage: async (userId: number, content: string) => {
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