import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'
import 'bootstrap/dist/css/bootstrap.css'

import { library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

import axios from 'axios';

axios.defaults.baseURL = 'https://localhost:7037';

const app = createApp(App)

app.use(createPinia())
app.use(router)

app.mount('#app')


