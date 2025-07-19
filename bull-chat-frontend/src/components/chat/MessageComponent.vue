<template>
  <div :class="['message-bubble', sender]">
    <div class="message-content">
      {{ text }}
    </div>
    <div class="message-time">
      {{ time }}
    </div>
  </div>
</template>

<script setup lang="ts">
import type { MessageAuthor } from '../types/MessageTypes';

defineProps({
  text: {
    type: String,
    required: true
  },
  sender: {
    type: String,
    validator: (value: MessageAuthor) => ['me','not_me'].includes(value),
    required: true
  },
  time: {
    type: String,
    default: () => new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
  }
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