import type { HomePage, NavigationItem, NewsTeaser, PagedResponse, Page } from './types'

/** CMS origin. Empty in development and preview, where Vite proxies /umbraco and /media. */
export const CMS_ORIGIN = import.meta.env.VITE_UMBRACO_URL ?? ''

const BASE = `${CMS_ORIGIN}/umbraco/delivery/api/v2`

// Media custom properties (alt text) are only included when expanded, also inside blocks.
const EXPAND_IMAGES = 'properties[image,blocks[properties[image]]]'

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

function encodePath(path: string): string {
  const withSlash = path.endsWith('/') ? path : `${path}/`
  return withSlash.split('/').map(encodeURIComponent).join('/')
}

/** Any page by its Umbraco route, e.g. "/our-bread/". The route decides which page component renders. */
export function getPageByPath(path: string): Promise<Page> {
  return get<Page>(`/content/item${encodePath(path)}`, { expand: EXPAND_IMAGES })
}

/** The root page. It carries the site settings (name, contact details, footer). */
export function getSiteRoot(): Promise<HomePage> {
  return get<HomePage>('/content/item/')
}

/** Top-level pages for the main navigation, in backoffice sort order. */
export async function getNavigation(): Promise<NavigationItem[]> {
  const page = await get<PagedResponse<NavigationItem>>('/content', {
    fetch: 'children:/',
    fields: 'properties[hideFromNavigation]',
    sort: 'sortOrder:asc',
    take: '20',
  })
  return page.items.filter((item) => !item.properties.hideFromNavigation)
}

/** News items under a news list, newest first (publishDate sort is added on the CMS side). */
export async function getNewsItems(newsListId: string, take = 20): Promise<NewsTeaser[]> {
  const page = await get<PagedResponse<NewsTeaser>>('/content', {
    fetch: `children:${newsListId}`,
    filter: 'contentType:newsItem',
    sort: 'publishDate:desc',
    fields: 'properties[publishDate,teaser,image]',
    expand: 'properties[image]',
    take: String(take),
  })
  return page.items
}
