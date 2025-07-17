<template>

  <n-config-provider :theme="darkTheme">
    
    <n-card class="app_container">
      <n-message-provider>
        <router-view />
      </n-message-provider>
    </n-card>

  </n-config-provider>

</template>

<script setup lang="ts">
import { NConfigProvider, NMessageProvider, darkTheme } from 'naive-ui'

</script>

<style>

.app_container {
  height: 100vh;
  border-radius: 0;
}

</style>
