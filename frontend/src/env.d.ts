/// <reference types="vite/client" />

interface ImportMetaEnv {
  /** Absolute CMS origin for production builds. Leave unset in dev to use the Vite proxy. */
  readonly VITE_UMBRACO_URL?: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
