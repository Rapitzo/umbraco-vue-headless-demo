import { CMS_ORIGIN } from './client'
import type { MediaItem } from './types'

/**
 * Image renditions. The CMS signs these server-side (ImagePresetMiddleware), so only the
 * presets and widths below exist. Keep both lists in sync with the backend.
 */
export type ImagePreset = 'wide' | 'standard' | 'square' | 'original'

const RATIOS: Record<ImagePreset, number | null> = {
  wide: 16 / 9,
  standard: 4 / 3,
  square: 1,
  original: null,
}

const WIDTHS = [480, 800, 1200, 1600] as const

export function imageUrl(media: MediaItem, preset: ImagePreset, width: (typeof WIDTHS)[number]): string {
  const params = new URLSearchParams({ preset, width: String(width) })
  if (media.focalPoint) {
    params.set('fx', media.focalPoint.left.toFixed(3))
    params.set('fy', media.focalPoint.top.toFixed(3))
  }
  return `${CMS_ORIGIN}${media.url}?${params}`
}

export function imageSrcset(media: MediaItem, preset: ImagePreset): string {
  return WIDTHS.map((width) => `${imageUrl(media, preset, width)} ${width}w`).join(', ')
}

/** Intrinsic size for the width/height attributes, so the browser reserves space before load. */
export function imageSize(media: MediaItem, preset: ImagePreset): { width: number; height: number } {
  const ratio = RATIOS[preset] ?? (media.width && media.height ? media.width / media.height : 3 / 2)
  return { width: 1600, height: Math.round(1600 / ratio) }
}

export function altText(media: MediaItem): string {
  return media.properties?.altText?.trim() ?? ''
}

/** Media pickers return arrays; these pages only ever pick one image. */
export function firstImage(images: MediaItem[] | null | undefined): MediaItem | null {
  return images?.[0] ?? null
}
