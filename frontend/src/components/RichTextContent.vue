<script setup lang="ts">
import { useRouter } from 'vue-router'

defineProps<{ markup: string }>()

const router = useRouter()

// Rich text comes from the CMS as HTML. Internal links are handed to the router so they
// navigate without a full page load; external links and modifier clicks behave normally.
function onClick(event: MouseEvent) {
  const link = (event.target as HTMLElement).closest('a')
  if (!link || event.defaultPrevented || event.button !== 0 || event.metaKey || event.ctrlKey || event.shiftKey) return
  if (link.target === '_blank' || link.origin !== window.location.origin) return
  event.preventDefault()
  router.push(link.pathname + link.search + link.hash)
}
</script>

<template>
  <!-- Markup is authored by trusted editors in the Umbraco backoffice. -->
  <div class="prose" @click="onClick" v-html="markup" />
</template>
