import { ref } from 'vue'
import { getNavigation, getSiteRoot } from '../api/client'
import type { HomePage, NavigationItem } from '../api/types'

// Site-wide data, loaded once and shared by the header, footer and contact page.
const settings = ref<HomePage['properties'] | null>(null)
const navigation = ref<NavigationItem[]>([])
const error = ref<string | null>(null)
// Content type of the page on screen, set by PageResolver. The footer uses it to skip
// details the page itself already shows.
const pageType = ref<string | null>(null)
let loading: Promise<void> | null = null

function load(): Promise<void> {
  return Promise.all([getSiteRoot(), getNavigation()])
    .then(([root, items]) => {
      settings.value = root.properties
      navigation.value = items
    })
    .catch((e: unknown) => {
      error.value = e instanceof Error ? e.message : String(e)
      loading = null
    })
}

export function useSite() {
  loading ??= load()
  return { settings, navigation, error, pageType }
}
