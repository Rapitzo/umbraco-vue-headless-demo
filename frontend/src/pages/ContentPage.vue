<script setup lang="ts">
import type { ContentPage } from '../api/types'
import BlockGrid from '../components/BlockGrid.vue'
import PageHeader from '../components/PageHeader.vue'

defineProps<{ page: ContentPage }>()
</script>

<template>
  <PageHeader :title="page.name" :intro="page.properties.intro" />
  <BlockGrid :model="page.properties.blocks" />
</template>
