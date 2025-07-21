<template>
  <div class="root">

    <div class="chat-header">

      <n-button 
        tertiary
        round
        type="primary" 
        @click="activateDrawer"
      >
        Информация
      </n-button>

      
      <n-button 
        tertiary 
        round 
        type="info"
      >
        Выход
      </n-button>
      
    </div>

    <n-drawer v-model:show="isDrawerActive" :placement="drawerPlacement">
      <n-drawer-content>
        Бычок без еды продержится ровно 5 минут, после чего начинается тряска.
      </n-drawer-content>
    </n-drawer>

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
import { ref, onMounted, nextTick, onUnmounted, watch } from 'vue';
import { NInput, NButton, useMessage } from 'naive-ui';

import type { DrawerPlacement } from 'naive-ui';

import type { MessageApi } from 'naive-ui';

import MessageComponent from '../chat/MessageComponent.vue';

import { useChatHub } from '../../hubs/chat';
import { useUserStore } from '../../stores/user';
import { useMessagesStore } from '../../stores/messages';

const flash: MessageApi = useMessage();

const isDrawerActive = ref(false);
const drawerPlacement = ref<DrawerPlacement>('top');

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

function activateDrawer() {
  isDrawerActive.value = true
}

watch(
  () => messagesStore.messages?.length,
  (_old, _new) => scrollToBottom()
)

onMounted(async () => {
  if (token) await start(token, flash);
  scrollToBottom();
});

onUnmounted(async () => {
  await stop(flash);
})
</script>

<style scoped>
.root {
  height: 100%;
  margin: auto;
  display: flex;
  flex-direction: column;
  max-width: 1100px;
}

.chat-header {
  display: flex;
  justify-content: space-between;
}

.chat-container {
  width: 100%;
  height: 100%;
  display: flex;
  margin-top: 10px;
  flex-direction: column;
  background-color: rgba(32, 32, 32, 0.185);
  border-radius: 4px;
  box-shadow: 0 2px 8px rgba(240, 240, 240, 0.171);
  overflow: hidden; 
}

.messages-container {
  height: 100%;
  padding: 16px;
  overflow-y: auto; 
  display: flex;
  flex-direction: column;
}

.input-area {
  padding: 12px;
  display: flex;
  gap: 8px;
  flex-shrink: 0;
}
</style>