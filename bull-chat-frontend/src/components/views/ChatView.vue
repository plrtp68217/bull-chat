<template>
  <div class="root">
    <div class="right_column" style="background-color: blue;"></div>
    <div class="center_column">
      <div class="chat-container">
        <div class="messages-container" ref="messagesContainer">
          <MessageComponent
            v-for="(message, index) in messages"
            :key="index"
            :text="message.text"
            :sender="message.sender"
            :time="formatTime(message.timestamp)"
          />
        </div>
        <div class="input-area">
          <n-input
            v-model:value="newMessage"
            type="textarea"
            placeholder="Введите сообщение..."
            :autosize="{ minRows: 2, maxRows: 5 }"
            @keyup.enter.prevent="sendMessage"
          />
          <n-button type="primary" @click="sendMessage" :disabled="!newMessage.trim()">
            Отправить
          </n-button>
        </div>
      </div>
    </div>
    <div class="left_column" style="background-color: green;"></div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, nextTick } from 'vue';
import { NInput, NButton } from 'naive-ui';
import MessageComponent from '../chat/MessageComponent.vue';
import type { Message } from '../types/MessageTypes';

const newMessage = ref('');
const messages = ref<Message[]>([
  { text: 'ыыыыыы', sender: 'not_me', timestamp: new Date() },
  { text: 'Скоро в стойло', sender: 'me', timestamp: new Date() }
]);

const messagesContainer = ref<HTMLElement | null>(null);

const sendMessage = () => {
  if (!newMessage.value.trim()) return;
  
  messages.value.push({
    text: newMessage.value,
    sender: 'me',
    timestamp: new Date()
  });
  
  newMessage.value = '';
  
  scrollToBottom();
  
  setTimeout(() => {
    messages.value.push({
      text: 'АААА',
      sender: 'not_me',
      timestamp: new Date()
    });
    scrollToBottom();
  }, 1000);
};

const formatTime = (date: Date) =>  date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

const scrollToBottom = () => {
  nextTick(() => {
    if (messagesContainer.value) {
      messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight;
    }
  });
};

onMounted(() => {
  scrollToBottom();
});
</script>

<style scoped>
.root {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  grid-template-rows: repeat(5, 1fr);
  grid-column-gap: 14px;
  grid-row-gap: 0px;
  height: 95vh; /* что то надо с этим делать */
  overflow: hidden; 
}

.center_column {
  grid-area: 1 / 2 / 6 / 5;
  display: flex;
  flex-direction: column;
  padding: 16px;
  background-color: #f5f5f500;
  height: 100%;
  overflow: hidden; 
}

.left_column {
  grid-area: 1 / 1 / 6 / 2;
  overflow: hidden; 
}

.right_column {
  grid-area: 1 / 5 / 6 / 6;
  overflow: hidden; 
}

.chat-container {
  display: flex;
  flex-direction: column;
  height: 100%;
  background-color: #202020ff; /* ЦВЕТ */
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden; 
}

.messages-container {
  flex-grow: 1;
  padding: 16px;
  overflow-y: auto; 
  display: flex;
  flex-direction: column;
}

.input-area {
  padding: 12px;
  border-top: 1px solid #e0e0e0;
  display: flex;
  gap: 8px;
  background-color: white;
  flex-shrink: 0; /* Запрещаем сжатие области ввода */
}
</style>