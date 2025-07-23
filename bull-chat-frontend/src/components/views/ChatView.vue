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
        @click="logOut"
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

        <div v-if="triggerIsVisible" class="load-more-trigger">
          <n-spin size="small" />
        </div>

        <MessageComponent v-for="(message, index) in messagesStore.messages" 
          :key="index"
          :message="message"
          :previousMessage="index > 0 ? messagesStore.messages[index - 1] : null"
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
import router from '../../../router/router.ts';
import { ref, onMounted, nextTick, onUnmounted, watch } from 'vue';
import { NInput, NButton, useMessage } from 'naive-ui';

import type { DrawerPlacement } from 'naive-ui';

import type { MessageApi } from 'naive-ui';

import MessageComponent from '../chat/MessageComponent.vue';

import { useMessagesStore } from '../../stores/messages.ts';
import { useChatHub } from '../../hubs/chat';
import { api } from '../../api';

import useObserver from '../../observer/index.ts';

const { initObserver, disconnectObserver } = useObserver(
  '.load-more-trigger',
  {
    onVisible: updateMessagesContainer,
  },
);

const triggerIsVisible = ref(true);

const isObserverDetected = ref(false);

const flash: MessageApi = useMessage();

const isDrawerActive = ref(false);
const drawerPlacement = ref<DrawerPlacement>('top');

const newContent = ref('');

const messagesStore = useMessagesStore();

const messagesContainer = ref<HTMLElement | null>(null);
const isMessagesContainerAtBottom = ref(true)

const { start, stop, invoke } = useChatHub();

const sendMessage = () => {
  if (!newContent.value.trim()) return;

  invoke.SendMessage(newContent.value);
  newContent.value = '';
}

async function logOut() {
  try {
    await api.auth.logout();
    await stop(flash);
    router.push('/');
  }
  catch (error) {
    flash.error(`${error}`);
  }
}

async function updateMessagesContainer() {
  if (isObserverDetected.value || messagesContainer.value == null) return;

  isObserverDetected.value = true;

  const container = messagesContainer.value;
  const scrollBefore = container.scrollTop;
  const heightBefore = container.scrollHeight;
  
  try {
    const olderMessageId: number = messagesStore.messages[0].id;
    const oldMessages = await api.messages.getMessages(olderMessageId);

    if (oldMessages.length == 0) {
      triggerIsVisible.value = false;
      return;
    }

    messagesStore.prependMessages(oldMessages);

    await nextTick();

    const heightAfter = container.scrollHeight;
    container.scrollTop = scrollBefore + (heightAfter - heightBefore);
  }
  catch (error) {
    flash.error(`${error}`);
  }
  finally {
    isObserverDetected.value = false;
  }
}

function scrollToBottom() {
  nextTick(() => {
    if (messagesContainer.value) {
      messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight;
    }
  });
};

const checkScrollPosition = () => {
  if (!messagesContainer.value) return
  
  const { scrollTop, scrollHeight, clientHeight } = messagesContainer.value
  const threshold = 200;
  isMessagesContainerAtBottom.value = scrollHeight - (scrollTop + clientHeight) < threshold
}

function activateDrawer() {
  isDrawerActive.value = true
}

watch(
  () => messagesStore.getMessages?.length,
  (_old, _new) => {
    if (isMessagesContainerAtBottom.value) {
      scrollToBottom()
    }
  }
)

onMounted(async () => {
  try {
    await start(flash);
    const messages = await api.messages.getMessages(null);
    messagesStore.appendMessages(messages);

    initObserver();
    scrollToBottom();
  }
  catch (error) {
    flash.error(`${error}`);
  }

  if (messagesContainer.value) {
    messagesContainer.value.addEventListener('scroll', checkScrollPosition)
  }
});

onUnmounted(async () => {
  try {
    await stop(flash);
  }
  catch (error) {
    flash.error(`${error}`);
  }

  disconnectObserver();
  messagesStore.clearMessages();

  if (messagesContainer.value) {
    messagesContainer.value.removeEventListener('scroll', checkScrollPosition)
  }
})

</script>

<style scoped>
.root {
  height: 100%;
  margin: auto;
  display: flex;
  flex-direction: column;
  max-width: 1400px;
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

.load-more-trigger {
  align-self: center;
}

.input-area {
  padding: 12px;
  display: flex;
  gap: 8px;
  flex-shrink: 0;
}
</style>