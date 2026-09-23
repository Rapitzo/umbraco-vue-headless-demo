<script setup lang="ts">
import type { RichTextBlock } from '../../api/types'
import RichTextContent from '../RichTextContent.vue'

defineProps<{ block: RichTextBlock }>()
</script>

<template>
  <RichTextContent v-if="block.properties.text" :markup="block.properties.text.markup" />
</template>

