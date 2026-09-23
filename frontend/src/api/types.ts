// Shapes returned by the Umbraco Content Delivery API (v2).
// Only the fields this demo uses are typed.

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

// Block elements (element types seeded in Umbraco)

export type TextBlock = ApiElement<'textBlock', { heading: string | null; text: string | null }>
export type QuoteBlock = ApiElement<'quoteBlock', { quote: string | null; attribution: string | null }>

// Discriminated union on contentType: add new block types here.
export type ArticleBlock = TextBlock | QuoteBlock

export interface BlockListItem<T> {
  content: T
  settings: unknown | null
}

export interface BlockListModel<T> {
  items: BlockListItem<T>[]
}

// Document types

export type Article = ApiContent<
  'article',
  { summary: string | null; body: BlockListModel<ArticleBlock> | null }
>

export type ArticleList = ApiContent<'articleList', { intro: string | null }>
