// Shapes returned by the Umbraco Content Delivery API (v2).
// Only the fields this site uses are typed.

export interface ApiRoute {
  path: string
  startItem: { id: string; path: string }
}

export interface ApiContent<TType extends string, TProps> {
  id: string
  contentType: TType
  name: string
  createDate: string
  updateDate: string
  route: ApiRoute
  properties: TProps
}

export interface ApiElement<TType extends string, TProps> {
  id: string
  contentType: TType
  properties: TProps
}

export interface PagedResponse<T> {
  total: number
  items: T[]
}

/** A picked content item (Content Picker). Its properties are not expanded. */
export type ContentLink = ApiContent<string, Record<string, never>>

/** A picked media item (Media Picker). Custom properties such as altText need `expand`. */
export interface MediaItem {
  id: string
  name: string
  url: string
  width: number | null
  height: number | null
  focalPoint: { left: number; top: number } | null
  properties?: { altText?: string | null }
}

export interface RichText {
  markup: string
}

// Block elements

export type HeroBlock = ApiElement<
  'heroBlock',
  {
    heading: string
    text: string | null
    image: MediaItem[] | null
    ctaLabel: string | null
    ctaLink: ContentLink | null
  }
>
export type RichTextBlock = ApiElement<'richTextBlock', { text: RichText | null }>
export type ImageBlock = ApiElement<'imageBlock', { image: MediaItem[] | null; caption: string | null }>
export type QuoteBlock = ApiElement<'quoteBlock', { quote: string; attribution: string | null }>
export type CardRowBlock = ApiElement<'cardRowBlock', { heading: string | null }>
export type CardBlock = ApiElement<
  'cardBlock',
  { title: string; text: string | null; image: MediaItem[] | null; link: ContentLink | null }
>

export type StoryBlock = ApiElement<
  'storyBlock',
  { heading: string; text: RichText | null; image: MediaItem[] | null; caption: string | null; imageOnLeft: boolean | null }
>

export type BakeItemBlock = ApiElement<
  'bakeItemBlock',
  {
    name: string
    note: string | null
    readyAt: string | null
    price: number | null
    soldOut: boolean | null
    image: MediaItem[] | null
  }
>
export type BakeBoardBlock = ApiElement<
  'bakeBoardBlock',
  { heading: string; intro: string | null; items: BlockListModel<BakeItemBlock> | null }
>

export type CinemaCardBlock = ApiElement<
  'cinemaCardBlock',
  { kicker: string | null; title: string; text: string | null; icon: MediaItem[] | null; link: ContentLink | null }
>
export type CinemaBlock = ApiElement<
  'cinemaBlock',
  {
    heading: string
    intro: string | null
    tags: string | null
    firstHeading: string | null
    firstText: string | null
    fact1Value: string | null
    fact1Label: string | null
    fact2Value: string | null
    fact2Label: string | null
    secondHeading: string | null
    secondText: string | null
    ctaLabel: string | null
    ctaLink: ContentLink | null
    cards: BlockListModel<CinemaCardBlock> | null
    boardHeading: string | null
    boardIntro: string | null
    bakes: BlockListModel<BakeItemBlock> | null
    skyLayer: MediaItem[] | null
    backLayer: MediaItem[] | null
    foregroundLayer: MediaItem[] | null
    splitLeftLayer: MediaItem[] | null
    closeUpLayer: MediaItem[] | null
  }
>

// Discriminated union on contentType: add new block types here and in BlockGrid.vue.
export type GridBlock =
  | HeroBlock
  | RichTextBlock
  | ImageBlock
  | QuoteBlock
  | CardRowBlock
  | CardBlock
  | StoryBlock
  | BakeBoardBlock
  | CinemaBlock

export interface BlockListModel<T> {
  items: { content: T }[]
}

export interface BlockGridArea {
  alias: string
  items: BlockGridItem[]
}

export interface BlockGridItem {
  columnSpan: number
  areaGridColumns: number | null
  areas: BlockGridArea[]
  // Content can be an element type this frontend does not know yet; BlockGrid.vue skips it.
  content: ApiElement<string, unknown>
}

export interface BlockGridModel {
  gridColumns: number
  items: BlockGridItem[]
}

// Document types

interface PageBase {
  metaDescription: string | null
  hideFromNavigation: boolean | null
}

export type HomePage = ApiContent<
  'home',
  PageBase & {
    blocks: BlockGridModel | null
    siteName: string
    logoText: string | null
    tagline: string | null
    address: string | null
    openingHours: string | null
    phone: string | null
    email: string | null
    footerNote: string | null
    mapUrl: string | null
  }
>

export type ContentPage = ApiContent<'contentPage', PageBase & { intro: string | null; blocks: BlockGridModel | null }>

export type NewsListPage = ApiContent<'newsList', PageBase & { intro: string | null }>

export type NewsItemPage = ApiContent<
  'newsItem',
  PageBase & {
    publishDate: string
    teaser: string
    image: MediaItem[] | null
    blocks: BlockGridModel | null
  }
>

/** A news item as listed on the news page: only the fields the list needs are requested. */
export type NewsTeaser = ApiContent<'newsItem', Pick<NewsItemPage['properties'], 'publishDate' | 'teaser' | 'image'>>

export type ContactPage = ApiContent<
  'contactPage',
  PageBase & { intro: string | null; image: MediaItem[] | null; formIntro: string | null }
>

export type Page = HomePage | ContentPage | NewsListPage | NewsItemPage | ContactPage

/** Any page in the tree, used for navigation where only name, route and flags matter. */
export type NavigationItem = ApiContent<string, Partial<PageBase>>
