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

    <div 
      v-if="message.content.contentType == CONTENT_TYPE.TEXT"
      class="message-content">
      {{ message.content.item }}
    </div>

    <img 
      v-else-if="message.content.contentType == CONTENT_TYPE.IMAGE"
      :src="baseURL + message.content.item "
      class="message-content"
    >

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

const baseURL = import.meta.env.VITE_BASE_URL;

interface IContentType {
  UNKNOWN: number,
  TEXT: number,
  IMAGE: number
}

const CONTENT_TYPE: IContentType = {
  UNKNOWN: 0,
  TEXT: 1,
  IMAGE: 2,
}

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
    .toLocaleTimeString([], { 
      hour: '2-digit', 
      minute: '2-digit' 
    })
}

function formateDate(date: Date) {
  return new Date(date)
    .toLocaleDateString([], {
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
  border-radius: 50px;
  background-color: #ffffff17;
}

.message-bubble {
  max-width: 60%;
  padding: 10px 14px;
  border-radius: 18px;
  margin-bottom: 12px;
  position: relative;
  word-wrap: break-word;
  color: white;
}

@media (max-width: 768px) {
  .message-bubble {
    max-width: 90%
  }
}

.message-bubble.me {
  align-self: flex-end;
  background-color: #188fff80;
  border-bottom-right-radius: 0;
}

.message-bubble.not_me{
  align-self: flex-start;
  background-color: #4946467e;
  border-bottom-left-radius: 0;
}

.message-author {
  font-size: 1.1rem;
}

.message-content {
  margin: 4px 0;
}

.message-time {
  font-size: 0.75rem;
  opacity: 0.8;
  text-align: right;
}
</style>