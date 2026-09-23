import '@fontsource-variable/fraunces'
import '@fontsource-variable/source-sans-3'
import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import { router } from './router'

createApp(App).use(router).mount('#app')
