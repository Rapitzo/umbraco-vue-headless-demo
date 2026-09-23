import '@fontsource/gloock/index.css'
import '@fontsource-variable/schibsted-grotesk/wght.css'
import '@fontsource-variable/schibsted-grotesk/wght-italic.css'
import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import { router } from './router'

createApp(App).use(router).mount('#app')
