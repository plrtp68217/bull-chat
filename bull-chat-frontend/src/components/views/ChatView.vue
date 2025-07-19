<template>
  <div class="root">


      <div class="chat-info"> 

        <n-card style="margin-bottom: 16px">

          <n-tabs type="line" animated>

            <n-tab-pane class="chat-info-tab" name="oasis" tab="В сети">
              <n-scrollbar>
                <p v-for="i in 50">
                  Бычок {{ i }}
                </p>
              </n-scrollbar>
            </n-tab-pane>

            <n-tab-pane class="chat-info-tab" name="the beatles" tab="Участники">
              <n-scrollbar>
                <p v-for="i in 90">
                  Бычок {{ i }}
                </p>
              </n-scrollbar>
            </n-tab-pane>

          </n-tabs>

        </n-card>
        
      </div>

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

</template>

<script setup lang="ts">
import { ref, onMounted, nextTick } from 'vue';
import { NInput, NButton, NScrollbar } from 'naive-ui';
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
  height: 100%;
  display: flex;
  gap: 10px;
}

.chat-info {
  width: 200px;
}

.chat-info-tab {
  height: 50vh;
}

.chat-container {
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 95vh;
  background-color: #202020ff;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden; 
}

.messages-container {
  height: 100%;
  padding: 16px;
  overflow: auto; 
  display: flex;
  flex-direction: column;
}

.input-area {
  padding: 12px;
  display: flex;
  gap: 8px;
  flex-shrink: 0; /* Запрещаем сжатие области ввода */
}
</style>