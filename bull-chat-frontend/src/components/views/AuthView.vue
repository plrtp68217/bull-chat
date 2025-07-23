<template>

  <div class="form-container">

    <n-card class="form-card">
  
      <n-tabs
        default-value="signin"
        size="large"
        animated
        pane-wrapper-style="margin: 0 -4px"
        pane-style="padding-left: 4px; padding-right: 4px; box-sizing: border-box;"
      >
  
        <n-tab-pane name="signin" tab="Авторизация">
  
          <LoginForm @login-event="loginUser"/>
  
        </n-tab-pane>
  
        <n-tab-pane name="signup" tab="Регистрация">
  
          <RegistrationForm @register-event="registerUser"/>
  
        </n-tab-pane>
  
      </n-tabs>
  
    </n-card>

  </div>


</template>

<script setup lang="ts">
import router from '../../../router/router.ts';
import { useMessage } from 'naive-ui';

import LoginForm from '../auth/LoginForm.vue';
import RegistrationForm from '../auth/RegistrationForm.vue';

import {api} from '../../api/index.ts';
import type { IAuthDto } from '../../api/interfaces/authorization/IAuthDto.ts';
import { useUserStore } from '../../stores/user.ts';

const userStore = useUserStore();

const flash = useMessage();

async function loginUser(dto: IAuthDto) {
  try {
    const user = await api.auth.login(dto);
    userStore.setUser(user);

    router.push('/chat');
    flash.success('Отлично, вы авторизованы!');
  }
  catch (error) {
    flash.error(`${error}`);
  }
}

async function registerUser(dto: IAuthDto) {
  try {
    const registerResponse = await api.auth.register(dto);
    flash.success(`${registerResponse}`);
  }
  catch (error) {
    flash.error(`${error}`);
  }
}

</script>

<style scoped>

.form-container {
  height: 100%;
  display: grid;
  place-items: center;
}

.form-card {
  width: 500px;
}

@media (max-width: 768px) {
  .form-card {
    width: auto;
  }
}

</style>
