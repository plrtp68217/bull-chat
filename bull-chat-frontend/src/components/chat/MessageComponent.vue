<template>

  <div 
    v-if="previousMessage === null || datesIsNotEquals(message.date, previousMessage.date)" 
    class="message-date"
  >
    {{ formateDate(message.date) }}
  </div>

  <div :class="['message-bubble', message.user.id == userStore.getUserId ? 'me' : 'not_me']">
    <div class="message-author">
      {{ message.user.name }}
    </div>

    <div class="message-content">
      {{ message.content.item }}
    </div>

    <div class="message-time">
      {{ formateDateToTime(message.date) }}
    </div>

  </div> 

</template>

<script setup lang="ts">
import { type PropType } from 'vue'
import type { IMessage } from '../../stores/interfaces/IMessage';
import { useUserStore } from '../../stores/user';

const userStore = useUserStore();

defineProps({
  message: {
    type: Object as PropType<IMessage>,
    required: true
  },
  previousMessage: {
    type: Object as PropType<IMessage | null>,
    required: true
  }
});

function formateDateToTime(date: Date) {
  return new Date(date)
    .toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

function formateDate(date: Date) {
  return new Date(date).toLocaleDateString(undefined, {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
}

function datesIsNotEquals(date1: Date, date2: Date) {
  date1 = new Date(date1)
  date2 = new Date(date2)
  date1.setHours(0, 0, 0, 0);
  date2.setHours(0, 0, 0, 0);

  return date1.getTime() !== date2.getTime();
}

</script>

<style scoped>

.message-date {
  width: 120px;
  align-self: center;
  text-align: center;
  color: white;
  padding: 2px 4px;
  margin: 8px 0;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(240, 240, 240, 0.171);
}

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
  border-bottom-right-radius: 0;
}

.message-bubble.not_me{
  align-self: flex-start;
  background-color: #494646fa;
  border-bottom-left-radius: 0;
}

.message-author {
  font-weight: 600;
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