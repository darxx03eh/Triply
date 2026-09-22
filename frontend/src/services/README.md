# Services

Pages and hooks only talk to the backend through this folder.

Every function here is backed either by the real API (`src/api/*`) or by a mock (`src/mocks/*`).
Mock-backed functions are marked with `// MOCK → <endpoint>`: when the endpoint is ready, replace the
body with a call to `src/api` and keep the same return type — no page needs to change.

Connected to the API: hotel page (details, rooms, reviews, nearby attractions), home page
(featured deals, trending destinations, recently visited) and search.
Still mocked: cart checkout / booking confirmation (`booking.service.ts`, backend Phase 6–7).
