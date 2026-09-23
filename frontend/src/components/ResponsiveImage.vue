<script setup lang="ts">
import { computed } from 'vue'
import { altText, imageSize, imageSrcset, imageUrl, type ImagePreset } from '../api/media'
import type { MediaItem } from '../api/types'

const props = withDefaults(
  defineProps<{
    media: MediaItem
    preset: ImagePreset
    /** The `sizes` attribute: how wide the image is rendered at each breakpoint. */
    sizes: string
    eager?: boolean
  }>(),
  { eager: false },
)

const size = computed(() => imageSize(props.media, props.preset))
</script>

<template>
  <img
    :src="imageUrl(media, preset, 800)"
    :srcset="imageSrcset(media, preset)"
    :sizes="sizes"
    :alt="altText(media)"
    :width="size.width"
    :height="size.height"
    :loading="eager ? 'eager' : 'lazy'"
    :fetchpriority="eager ? 'high' : undefined"
    decoding="async"
  />
</template>
