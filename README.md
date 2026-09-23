# Umbraco + Vue headless demo

A small reference project: Umbraco 17 (LTS) serves content through the Content Delivery API, and a Vue 3 + TypeScript frontend renders it. SQLite keeps setup to zero; the schema and sample content are created in code on first boot.

Built by Rickard Lindbom · Lindforge Digital Studio · [Available for contract work](https://portfolio-rick.vercel.app/work-with-me)

## What it demonstrates

- Delivery API enabled on a stock Umbraco 17 project (`backend/`), with no custom controllers.
- Schema as code. `backend/Seeding/DemoContentSeeder.cs` creates two element types, a Block List data type and two document types, then publishes three articles. It runs once and skips if the schema already exists.
- Typed API client. `frontend/src/api/` holds the response types and the only module that calls `fetch`. Block elements are a discriminated union on `contentType`.
- Block list rendering. `BlockList.vue` maps each element type alias to a Vue component. Blocks the frontend does not know are skipped.
- CMS-owned URLs. The frontend route mirrors the Umbraco route, so editors control slugs from the backoffice.
- Listing and detail pages. The list uses the query endpoint (`fetch=children:/`); the detail page fetches one item by path.

## Architecture

```
 Editors                                   Visitors
    |                                          |
    v                                          v
+---------------------------+      +---------------------------+
| Umbraco 17 backoffice     |      | Vue 3 + Vite frontend     |
| /umbraco                  |      | /            list page    |
|                           |      | /:path       detail page  |
| Document types (code)     |      |                           |
|   articleList             |      | api/client.ts  (fetch)    |
|   article                 |      | api/types.ts   (types)    |
|     summary, body (Block  |      | BlockList.vue (registry)  |
|     List: textBlock,      |      |   TextBlock / QuoteBlock  |
|     quoteBlock)           |      +-------------+-------------+
|                           |                    |
| Content Delivery API v2   |<-------------------+
| /umbraco/delivery/api/v2  |   JSON over HTTP (Vite proxy in dev)
+-------------+-------------+
              |
              v
        SQLite database
     (umbraco/Data, gitignored)
```

## Requirements

- .NET 10 SDK
- Node.js 20.19+ or 22.12+ (required by Vite 8)

## Run it

1. Start the CMS. The first run installs Umbraco into a local SQLite file and seeds the content.

   ```bash
   cd backend
   cp appsettings.Local.example.json appsettings.Local.json
   # edit appsettings.Local.json and set your own admin email and password (10+ characters)
   dotnet run
   ```

   The backoffice is at http://localhost:60733/umbraco. Sign in with the credentials from `appsettings.Local.json`.

2. Start the frontend in a second terminal.

   ```bash
   cd frontend
   npm install
   npm run dev
   ```

   Open the URL Vite prints (usually http://localhost:5173). Requests to `/umbraco/*` are proxied to the CMS. Set `UMBRACO_URL` if the CMS runs on a different address.

On a fresh install Umbraco builds the Delivery API search index in the background. The list page can be empty for about a minute after the first boot; detail pages work straight away.

To start over, stop the CMS and delete `backend/umbraco/Data`.

## Try the API directly

```text
# root page
http://localhost:60733/umbraco/delivery/api/v2/content/item/
# articles under the root
http://localhost:60733/umbraco/delivery/api/v2/content?fetch=children:/&filter=contentType:article
# one article by route
http://localhost:60733/umbraco/delivery/api/v2/content/item/typing-the-delivery-api/
```

## Build

```bash
dotnet build            # from the repo root, uses HeadlessDemo.sln
cd frontend && npm run build
```

For a production frontend build that talks to a CMS on another origin, set `VITE_UMBRACO_URL` at build time and allow that origin with CORS on the Umbraco side.

## Project layout

```
backend/
  Program.cs                       Umbraco host with AddDeliveryApi()
  Seeding/DemoContentComposer.cs   registers the seeder
  Seeding/DemoContentSeeder.cs     schema and sample content
  appsettings.json                 SQLite connection, DeliveryApi:Enabled
frontend/
  src/api/types.ts                 Delivery API response types
  src/api/client.ts                typed fetch wrapper
  src/components/BlockList.vue     block alias to component registry
  src/components/blocks/           one component per element type
  src/pages/                       list and detail pages
  vite.config.ts                   dev proxy to the CMS
```

## License

MIT. See [LICENSE](LICENSE).
