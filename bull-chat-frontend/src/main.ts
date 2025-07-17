import { createApp } from 'vue'
import App from './App.vue'
import router from '../router/router';
import naive from 'naive-ui'

import './style.css';

const app = createApp(App);

app
    .use(router)
    .use(naive)
    .mount('#app');