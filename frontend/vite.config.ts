import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'

/** Reverse proxy: browser → Vite (same origin) → backend. Avoids CORS in dev. */
function apiProxy(env: Record<string, string>) {
  const target = env.VITE_DEV_PROXY_TARGET || 'https://localhost:7074'
  return {
    '/api': {
      target,
      changeOrigin: true,
      secure: false,
    },
    '/hubs': {
      target,
      changeOrigin: true,
      secure: false,
      ws: true,
    },
  }
}

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  const proxy = apiProxy(env)

  return {
    plugins: [vue()],
    server: {
      host: '0.0.0.0',
      port: 5173,
      proxy,
    },
    preview: {
      host: '0.0.0.0',
      port: 5173,
      proxy,
    },
  }
})
