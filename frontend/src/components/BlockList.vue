<script setup lang="ts">
import type { Component } from 'vue'
import type { ArticleBlock, BlockListModel } from '../api/types'
import QuoteBlock from './blocks/QuoteBlock.vue'
import TextBlock from './blocks/TextBlock.vue'

defineProps<{ model: BlockListModel<ArticleBlock> | null }>()

// Element type alias -> component. Blocks without an entry are skipped.
const registry: Record<ArticleBlock['contentType'], Component> = {
  textBlock: TextBlock,
  quoteBlock: QuoteBlock,
}
</script>

<template>
  <div v-if="model" class="block-list">
    <template v-for="item in model.items" :key="item.content.id">
      <component
        :is="registry[item.content.contentType]"
        v-if="registry[item.content.contentType]"
        :block="item.content"
      />
    </template>
  </div>
</template>
