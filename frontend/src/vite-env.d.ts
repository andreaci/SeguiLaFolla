/// <reference types="vite/client" />

interface ImportMetaEnv {
  /** Backend origin when not using dev proxy (no trailing slash) */
  readonly VITE_API_BASE_URL?: string
  /** Dev: Vite proxy target (vite.config.ts only) */
  readonly VITE_DEV_PROXY_TARGET?: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
