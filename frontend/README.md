# Triply — Frontend

React 19 + TypeScript + Vite single-page app for the Triply hotel booking platform.

| Area | Choice |
| --- | --- |
| UI | React 19, Tailwind CSS v4, lucide-react icons, Plus Jakarta Sans |
| Routing | React Router (data router, route-level code splitting) |
| Server state | TanStack Query |
| HTTP | axios (`src/lib/http.ts`) |
| Forms | react-hook-form + zod |
| Toasts | sonner |

## Running

### With the whole stack (recommended)

The root `docker-compose.yml` builds this folder and serves it with nginx on **http://localhost:3000**.

```bash
docker compose up -d --build
```

nginx serves the SPA and proxies `/api/*` to the API container, so the browser talks to a single origin.
That matters because the refresh token is an `HttpOnly; SameSite=Strict` cookie.

### Local development

Start the backend (docker compose) and then:

```bash
cd frontend
npm install
npm run dev
```

Vite runs on http://localhost:5173 and proxies `/api` to `http://localhost:8080` (override with `VITE_API_PROXY_TARGET`, see `.env.example`).

| Script | Description |
| --- | --- |
| `npm run dev` | Dev server with hot reload |
| `npm run build` | Type-check and production build to `dist/` |
| `npm run typecheck` | Type-check only |
| `npm run preview` | Serve the production build locally |

## Project structure

```
src/
├── api/            One file per backend endpoint group — thin typed wrappers around axios
├── mocks/          Static data for features the backend does not expose yet
├── services/       What pages use: combines api/ and mocks/ (the integration seam)
├── types/          TypeScript models that mirror the backend DTOs
├── lib/            http client, token store, JWT decoding, Sieve query builder, formatting
├── features/       auth (provider, guards) and cart (provider)
├── hooks/          URL search state, lookups for admin selects, debounce
├── components/
│   ├── ui/         Design system: Button, Field, Modal, Drawer, Badge, Tabs, Pagination…
│   ├── layout/     Navbar, Footer, PublicLayout, AdminLayout (collapsible sidebar)
│   ├── search/     SearchBar (destination autocomplete, dates, guests & rooms)
│   ├── hotel/      Hotel / deal / destination cards
│   └── admin/      DataTable, page header, row actions, image upload box
└── pages/          Public pages, auth pages and admin pages
```

## Pages

| Route | Page | Data |
| --- | --- | --- |
| `/` | Home: search, featured deals, recently visited, trending destinations | mixed (see below) |
| `/search` | Availability search: filters (price, stars, type, amenities), sorting, infinite scroll | API |
| `/hotels/:id` | Gallery (fullscreen), details, amenities, map, reviews, rooms + add to cart | mixed |
| `/checkout` | Guest details, payment method, special requests | mock |
| `/booking/:confirmationNumber` | Confirmation with print / save as PDF | mock |
| `/login`, `/register`, `/confirm-email` | Authentication | API |
| `/admin` | Dashboard | API |
| `/admin/cities` · `/hotels` · `/rooms` · `/amenities` | Grids with filters, create, edit drawer, delete | API |

## Backend integration

Pages never call axios directly. They call `src/services/*`, and every service function is either:

- **API**: calls `src/api/*` (already connected), or
- **MOCK / LOCAL**: returns data from `src/mocks/*` or the browser, marked with a `// MOCK → <endpoint>` comment.

To connect a new endpoint, add it to `src/api/*`, then replace the body of the matching service function.
Keep the return type and no page has to change.

### Connected to the backend

| Feature | Endpoints |
| --- | --- |
| Auth | `POST /auth/register`, `POST /auth/login`, `POST /auth/refresh`, `DELETE /auth/logout`, `GET /auth/confirm-email` |
| Search | `GET /search` (dates, guests, rooms, price range, stars, types, amenities, sort, paging) |
| Cities | `GET/POST /cities`, `GET/PUT/DELETE /cities/{id}`, `POST/DELETE /cities/{id}/thumbnail` |
| Hotels | `GET/POST /hotels`, `GET/PUT/DELETE /hotels/{id}` |
| Hotel images | `GET/POST /hotels/{id}/images`, `DELETE /hotels/{id}/images/{imageId}` (status is polled while pending) |
| Rooms | `GET/POST /rooms`, `GET/PUT/DELETE /rooms/{id}`, `GET /hotels/{id}/rooms` |
| Amenities | `GET/POST /amenities`, `PUT/DELETE /amenities/{id}`, `GET/PUT /hotels/{id}/amenities` |

### Still mocked (waiting for the backend)

| Service function | Replace with | Notes |
| --- | --- | --- |
| `homeService.getFeaturedDeals` | `GET /deals/featured` | `FeaturedDeal[]` |
| `homeService.getTrendingDestinations` | `GET /cities/trending` | `TrendingCity[]`, top 5 by visits |
| `homeService.getRecentlyVisited` / `recentHotelsService` | `GET/POST /users/me/recent-hotels` | stored in localStorage for now |
| `hotelService.getReviews` | `GET /hotels/{id}/reviews` | `Review[]` |
| `hotelService.getNearbyAttractions` | `GET /hotels/{id}/attractions` | `NearbyAttraction[]` |
| `bookingService.create` | `POST /bookings` | payment + invoice e-mail |
| `bookingService.getByConfirmationNumber` | `GET /bookings/{confirmationNumber}` | `BookingConfirmation` |

### Temporary workarounds to remove

- **Hotel page for guests**: `GET /hotels/{id}` is admin-only, so `hotelService.getDetails` falls back to the public
  hotel list (no description or gallery) when it gets 401/403. Remove the fallback once the endpoint is public.

## Authentication

- The access token is kept in memory and mirrored to localStorage (`src/lib/tokenStore.ts`).
  The refresh token is never readable from JS; it's an HttpOnly cookie set by the API.
- `POST /auth/refresh` requires a *valid* access token, so `AuthProvider` refreshes silently 60 seconds before it expires.
- Roles come from the JWT `role` claim. `/admin/*` is guarded by `RequireAdmin`.
- The confirmation e-mail links to `/confirm-email` on `FRONTEND_BASE_URL`; that SPA page calls
  `/api/v1/auth/confirm-email` with XHR. nginx still redirects browser visits to the legacy API link.
