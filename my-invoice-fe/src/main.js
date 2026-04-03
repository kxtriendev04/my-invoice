import { createApp } from 'vue'
import App from './App.vue'
import './assets/css/icons.css'
import './assets/css/common.css'
import './assets/css/reset.css'
import './assets/css/style.css'
import router from './router'
import { createPinia } from 'pinia'

const app = createApp(App)
const pinia = createPinia()
app.use(router)
app.use(pinia)
app.mount('#app')
