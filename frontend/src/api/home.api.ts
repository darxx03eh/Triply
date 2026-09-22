import { http, unwrap } from '@/lib/http'
import type { ApiResponse } from '@/types/api'
import type { TrendingCity } from '@/types/city'
import type { FeaturedDeal, RecentHotel } from '@/types/hotel'

/** Triply.Api/Endpoints/HomeEndpoints.cs — count is 1..20 (default 5). */
export const homeApi = {
  featuredDeals: (count = 5) =>
    unwrap(http.get<ApiResponse<FeaturedDeal[]>>('/deals/featured', { params: { count } })),

  trendingCities: (count = 5) =>
    unwrap(http.get<ApiResponse<TrendingCity[]>>('/cities/trending', { params: { count } })),

  /** Visits are recorded by the backend when a signed-in user opens GET /hotels/{id}. */
  recentHotels: (count = 5) =>
    unwrap(http.get<ApiResponse<RecentHotel[]>>('/users/me/recent-hotels', { params: { count } })),
}
