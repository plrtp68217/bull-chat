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
              v-for="(message, index) in messagesStore.getMessages"
              :key="index"
              :message="message"
            />
          </div>

        <div class="input-area">
          <n-input
            v-model:value="newContent"
            type="textarea"
            placeholder="Введите сообщение..."
            :autosize="{ minRows: 2, maxRows: 5 }"
            @keyup.enter.prevent="sendMessage"
          />

          <n-button type="primary" @click="sendMessage" :disabled="!newContent.trim()">
            Отправить
          </n-button>

        </div>

      </div>

  </div>

</template>

<script setup lang="ts">
import { ref, onMounted, nextTick, onUnmounted } from 'vue';
import { NInput, NButton, NScrollbar } from 'naive-ui';
import MessageComponent from '../chat/MessageComponent.vue';
import { useChatHub } from '../../hubs/chat';
import { useUserStore } from '../../stores/user';
import { useMessagesStore } from '../../stores/messages';
import { watch } from 'vue';

const newContent = ref('');

const userStore = useUserStore();
const messagesStore = useMessagesStore();

const messagesContainer = ref<HTMLElement | null>(null);

const { start, stop, invoke } = useChatHub();
const token = localStorage.getItem('authToken') || '';

const sendMessage = () => {
  if (!newContent.value.trim()) return;

  invoke.SendMessage(userStore.getUserId!, newContent.value);
  newContent.value = '';
}

const scrollToBottom = () => {
  nextTick(() => {
    if (messagesContainer.value) {
      messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight;
    }
  });
};

watch(
  () => messagesStore.messages?.length,
  () => {
    scrollToBottom()
  },
)

onMounted(async () => {
  if (token) await start(token);
  scrollToBottom();
});

onUnmounted(async () => {
  await stop();
})
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