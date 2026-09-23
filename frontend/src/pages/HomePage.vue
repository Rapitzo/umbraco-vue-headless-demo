<script setup lang="ts">
import type { HomePage } from '../api/types'
import BlockGrid from '../components/BlockGrid.vue'

defineProps<{ page: HomePage }>()
</script>

<template>
  <h1 class="visually-hidden">{{ page.properties.siteName }}</h1>
  <BlockGrid :model="page.properties.blocks" />
</template>
