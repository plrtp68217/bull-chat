<template>

<n-modal
  class="modal"
  :show="isVisible"
  @update:show="$emit('update:isVisible', $event)"
>

    <n-card
      :title="title"
      :bordered="true"
      size="huge"
      role="dialog"
      aria-modal="true"
    >
      <slot></slot>

      <template #footer>
        {{ footer }}
      </template>
      
    </n-card>

  </n-modal>

</template>

<script setup lang="ts">

defineProps({
  title: {
    type: String,
    required: true,
  },
  footer: {
    type: String,
    required: true,
  },
  isVisible: {
    type: Boolean,
    required: true,
  }
})

</script>

<style scoped>

.modal {
  width: 95%;
  max-width: 600px;
  margin: 0 auto;
  top: '50%';
  transform: 'translateY(-50%)'
}


</style>
