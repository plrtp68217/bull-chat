<template>
  <n-form ref="formRef" :model="modelRef" :rules="rules">

    <n-form-item path="name" label="Имя">
      <n-input v-model:value="modelRef.name" @keydown.enter.prevent placeholder="Бычок"/>
    </n-form-item>

    <n-form-item path="password" label="Пароль">
      <n-input
        v-model:value="modelRef.password"
        type="password"
        placeholder="Породистый пароль"
        @input="handlePasswordInput"
        @keydown.enter.prevent
      />
    </n-form-item>

    <n-row :gutter="[0, 24]">
      <n-col :span="24">
        <div style="display: flex; justify-content: flex-end">
          <n-button
            :disabled="modelRef.name === null"
            round
            type="primary"
            @click="handleValidateButtonClick"
          >
            Принять
          </n-button>
        </div>
      </n-col>
    </n-row>

  </n-form>

</template>

<script setup lang="ts">
import type {
  FormInst,
  FormItemInst,
  FormItemRule,
  FormRules,
  FormValidationError
} from 'naive-ui'
import { useMessage } from 'naive-ui'
import { ref } from 'vue'

interface ModelType {
  name: string | null
  password: string | null
  reenteredPassword: string | null
}

const emit = defineEmits(['loginEvent'])

const formRef = ref<FormInst | null>(null)
const rPasswordFormItemRef = ref<FormItemInst | null>(null)
const message = useMessage()
const modelRef = ref<ModelType>({
  name: null,
  password: null,
  reenteredPassword: null
})

const rules: FormRules = {
  name: [
    {
      required: true,
      validator(rule: FormItemRule, value: string) {
        if (!value) {
          return new Error('Имя является обязательным')
        }
        return true
      },
      trigger: ['input', 'blur']
    }
  ],
  password: [
    {
      required: true,
      message: 'Пароль обязателен'
    }
  ],
}

function handlePasswordInput() {
  if (modelRef.value.reenteredPassword) {
    rPasswordFormItemRef.value?.validate({ trigger: 'password-input' })
  }
}

function handleValidateButtonClick(e: MouseEvent) {
  e.preventDefault()
  formRef.value?.validate((errors: Array<FormValidationError> | undefined) => {
    if (!errors) {
      emit('loginEvent', {login: modelRef.value.name, password: modelRef.value.password})
    }
    else {
      console.log(errors)
      message.error('Invalid')
    }
  })
}

</script>

<style scoped></style>
