<script setup lang="ts">
import type { HomePage } from '../api/types'
import BlockGrid from '../components/BlockGrid.vue'

defineProps<{ page: HomePage }>()
</script>

<template>
  <h1 class="visually-hidden">{{ page.properties.siteName }}</h1>
  <div class="home container">
    <BlockGrid :model="page.properties.blocks" />
  </div>
</template>

<style scoped>
.home {
  padding-top: clamp(1.5rem, 5vw, 4rem);
}
</style>
