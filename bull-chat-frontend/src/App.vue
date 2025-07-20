<template>

  <n-config-provider class="config-provider" :theme="darkTheme">
    
    <div class="app_container">
      <n-message-provider>
        <router-view />
      </n-message-provider>
    </div>

  </n-config-provider>

</template>

<script setup lang="ts">
import { NConfigProvider, NMessageProvider, darkTheme } from 'naive-ui'

</script>

<style scoped>

.config-provider {
  height: 100vh;
}

.app_container {
  height: 100%;
  border-radius: 0;
  background-color: rgb(24, 24, 28);
  padding: 16px;
}

</style>
