import vue from '@vitejs/plugin-vue'
import { defineConfig } from 'vite'

// In development and preview, /umbraco/* (Delivery API) and /media/* (images) are proxied to
// the Umbraco backend so the browser sees a single origin and no CORS setup is needed.
const cmsUrl = process.env.UMBRACO_URL ?? 'http://localhost:60733'
const proxy = {
  '/umbraco': { target: cmsUrl, changeOrigin: true },
  '/media': { target: cmsUrl, changeOrigin: true },
}

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: { proxy },
  preview: { proxy },
})
