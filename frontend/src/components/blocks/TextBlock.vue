<script setup lang="ts">
import type { TextBlock } from '../../api/types'

defineProps<{ block: TextBlock }>()
</script>

<template>
  <section class="text-block">
    <h2 v-if="block.properties.heading">{{ block.properties.heading }}</h2>
    <p v-if="block.properties.text">{{ block.properties.text }}</p>
  </section>
</template>
