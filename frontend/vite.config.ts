import vue from '@vitejs/plugin-vue'
import { defineConfig } from 'vite'

// In development, /umbraco/* is proxied to the Umbraco backend so the browser
// sees a single origin and no CORS setup is needed.
const cmsUrl = process.env.UMBRACO_URL ?? 'http://localhost:60733'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    proxy: {
      '/umbraco': { target: cmsUrl, changeOrigin: true },
    },
  },
})
