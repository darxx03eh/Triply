# Triply Frontend

The Triply frontend is a responsive React single-page application for discovering hotels, managing bookings, and administering platform content. It is designed to run with the [Triply API](../README.md) as part of the complete Docker Compose stack.

[![React 19](https://img.shields.io/badge/React-19-61DAFB?logo=react&logoColor=white)](https://react.dev/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.9-3178C6?logo=typescript&logoColor=white)](https://www.typescriptlang.org/)
[![Vite](https://img.shields.io/badge/Vite-8-646CFF?logo=vite&logoColor=white)](https://vite.dev/)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-4-06B6D4?logo=tailwindcss&logoColor=white)](https://tailwindcss.com/)

## Contents

- [Technology](#technology)
- [Run locally](#run-locally)
- [Scripts](#scripts)
- [Application areas](#application-areas)
- [Architecture](#architecture)
- [API integration](#api-integration)
- [Authentication](#authentication)
- [Production build](#production-build)

## Technology

| Area | Choice |
| --- | --- |
| UI | React 19, Tailwind CSS 4, Lucide icons, Plus Jakarta Sans |
| Routing | React Router with route-level code splitting |
| Server state | TanStack Query |
| Forms and validation | React Hook Form and Zod |
| HTTP | Axios with a typed API client and normalized errors |
| Notifications | Sonner |
| Tooling | Vite and TypeScript |

## Run locally

### Full stack with Docker

From the repository root, start every service—including the API, workers, database, and this frontend:

```bash
cp .env.example .env
docker compose up -d --build
```

Open the application at [http://localhost:3000](http://localhost:3000). nginx serves the SPA and proxies `/api/*` to the API container, preserving the same-origin setup required by the refresh-token cookie.

### Frontend development server

Start the backend first, then run the Vite server:

```bash
cp frontend/.env.example frontend/.env
cd frontend
npm ci
npm run dev
```

The development server runs at [http://localhost:5173](http://localhost:5173). It forwards `/api` requests to `http://localhost:8080` by default. Set `VITE_API_PROXY_TARGET` in `frontend/.env` to use another API address.

## Scripts

| Command | Description |
| --- | --- |
| `npm run dev` | Starts Vite with hot-module replacement |
| `npm run typecheck` | Runs the TypeScript compiler without emitting files |
| `npm run build` | Type-checks and creates an optimized production bundle in `dist/` |
| `npm run preview` | Serves the production bundle locally |

## Application areas

| Area | Routes | Capabilities |
| --- | --- | --- |
| Discovery | `/`, `/search`, `/hotels/:hotelId` | Search, filtering, hotel details, rooms, deals, reviews, and nearby attractions |
| Account | `/login`, `/register`, `/confirm-email` | Registration, sign-in, token refresh, and email confirmation |
| Booking | `/checkout`, `/bookings`, `/booking/:confirmationNumber` | Cart checkout, booking history, payment, cancellation, and invoice download |
| Administration | `/admin/*` | Dashboard and management of cities, hotels, rooms, amenities, deals, and users |

Protected booking routes require authentication. Administrative routes additionally require the administrator role.

## Architecture

```text
src/
├── api/          Resource-level HTTP clients and API-to-UI transformations
├── components/   Reusable UI, layouts, search, hotel, and admin components
├── features/     Cross-page auth and cart providers, hooks, and route guards
├── hooks/        Shared UI and query hooks
├── lib/          Axios client, error handling, token storage, formatting, and query helpers
├── pages/        Route components, lazily loaded except for the home page
├── services/     Feature-oriented orchestration for multi-endpoint workflows
├── types/        TypeScript contracts for API and UI data
├── mocks/        Local fallback image assets
└── styles/       Global Tailwind styles
```

Use the `@/` import alias for files under `src/`. Pages access resource clients in `api/` or feature services in `services/`; shared HTTP behavior remains centralized in `lib/http.ts`.

## API integration

All requests use the relative base path `/api/v1`. In development, Vite proxies that path to the API; in Docker, nginx proxies it. This avoids hard-coding an API host into browser code.

The API client sends cookies with each request, attaches an in-memory access token when available, and converts failed responses into a consistent `ApiError`. Keep endpoint-specific request and response mapping in `src/api/`; use `src/services/` when a user action coordinates multiple API calls or presentation-specific transformations.

Connected API areas include authentication, search, hotel details, rooms, reviews, attractions, cities, deals, cart operations, booking and payment flows, recent visits, and administration. Consult [the API reference](http://localhost:8080/swagger) while the local stack is running for request schemas and the full endpoint catalog.

## Authentication

- The access token is held in memory and mirrored to local storage by `src/lib/tokenStore.ts`.
- The refresh token is an `HttpOnly; SameSite=Strict` cookie and is never exposed to JavaScript.
- `AuthProvider` refreshes the session shortly before the access token expires.
- `RequireAuth`, `RequireAdmin`, and `GuestOnly` protect routes according to session state and the JWT role claim.

## Production build

The Docker build uses a Node build stage followed by nginx. It compiles the SPA, serves static assets, and proxies API requests through the same public origin.

```bash
docker compose up -d --build frontend
```

For a non-Docker production bundle, run `npm run build` and deploy the generated `dist/` directory behind a web server configured to return `index.html` for client-side routes and proxy `/api` to Triply.Api.
