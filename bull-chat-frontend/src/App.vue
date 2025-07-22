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
import router from '../router/router';
import { api } from './api';
import { onMounted } from 'vue';
import { useUserStore } from './stores/user';

const userStore = useUserStore();

async function checkAuthAndRedirect() {
  try {
    const user = await api.auth.validate();
    userStore.setUser(user);
    
    router.push('/chat');
  }
  catch {
    router.push('/');
  }
}

onMounted(() => {
  checkAuthAndRedirect();
})

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
