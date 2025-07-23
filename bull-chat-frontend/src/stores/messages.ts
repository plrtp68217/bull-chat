import { defineStore } from 'pinia'

import type { IMessage } from './interfaces/IMessage';

export const useMessagesStore = defineStore('messages', {
  state: () => ({
    messages: [] as IMessage[] |  null
  }),

  getters: {
    getMessages: (state) => state.messages
  },

  actions: {
    setMessages(messages: IMessage[]) {
      this.messages = messages;
    },
    addMessage(message: IMessage) {
      this.messages?.push(message)
    },
    addMessages(messages: IMessage[]) {
      this.messages?.push(...messages);
    },
    clearMessages() {
      this.messages?.splice(0, this.messages.length);
    }
  }
});