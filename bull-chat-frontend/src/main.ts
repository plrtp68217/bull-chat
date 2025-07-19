import { createApp } from 'vue'
import App from './App.vue'
import router from '../router/router';
import naive from 'naive-ui';
import { createPinia } from 'pinia';

import './style.css';

const pinia = createPinia();

const app = createApp(App);

app
    .use(pinia)
    .use(router)
    .use(naive)
    .mount('#app');