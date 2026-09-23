import type { Article, ArticleList, PagedResponse } from './types'

// Relative base URL: Vite proxies /umbraco to the CMS in development (see vite.config.ts).
const BASE = `${import.meta.env.VITE_UMBRACO_URL ?? ''}/umbraco/delivery/api/v2`

export class NotFoundError extends Error {}

async function get<T>(path: string, params?: Record<string, string>): Promise<T> {
  const query = params ? `?${new URLSearchParams(params)}` : ''
  const response = await fetch(`${BASE}${path}${query}`, {
    headers: { Accept: 'application/json' },
  })

  if (response.status === 404) throw new NotFoundError(`Not found: ${path}`)
  if (!response.ok) throw new Error(`Delivery API ${response.status} for ${path}`)

  return (await response.json()) as T
}

/** The root "Insights" page (route "/"). */
export function getArticleList(): Promise<ArticleList> {
  return get<ArticleList>('/content/item/')
}

/** Published articles under the root, in backoffice sort order. */
export async function getArticles(): Promise<Article[]> {
  const page = await get<PagedResponse<Article>>('/content', {
    fetch: 'children:/',
    filter: 'contentType:article',
    sort: 'sortOrder:asc',
    take: '20',
  })
  return page.items
}

/** A single article by its Umbraco route, e.g. "/typing-the-delivery-api/". */
export function getArticleByPath(path: string): Promise<Article> {
  const encoded = path.split('/').map(encodeURIComponent).join('/')
  return get<Article>(`/content/item${encoded}`)
}
