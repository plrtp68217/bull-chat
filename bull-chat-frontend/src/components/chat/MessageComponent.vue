<template>

  <div :class="['message-bubble', userId == message.user.id ? 'me' : 'not_me']">

    <div class="message-author">
      {{ message.user.name }}
    </div>

    <div class="message-content">
      {{ message.content.item }}
    </div>

    <div class="message-time">
      {{ new Date(message.date).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}}
    </div>

  </div> 

</template>

<script setup lang="ts">
import type { PropType } from 'vue'
import type { IMessage } from '../../stores/interfaces/IMessage';
import { useUserStore } from '../../stores/user';

const userStore = useUserStore();
const userId: number = userStore.getUserId;

defineProps({
  message: {
    type: Object as PropType<IMessage>,
    required: true
  },
});
</script>

<style scoped>
.message-bubble {
  max-width: 80%;
  padding: 10px 14px;
  border-radius: 18px;
  margin-bottom: 12px;
  position: relative;
  word-wrap: break-word;
  color: white;
}

.message-bubble.me {
  align-self: flex-end;
  background-color: #1890ff;
  border-bottom-right-radius: 4px;
}

.message-bubble.not_me{
  align-self: flex-start;
  background-color: #494646fa;
  border-bottom-left-radius: 4px;
}

.message-content {
  margin-bottom: 4px;
}

.message-time {
  font-size: 0.75rem;
  opacity: 0.8;
  text-align: right;
}
</style>