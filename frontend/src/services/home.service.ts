import { homeApi } from '@/api/home.api'

export const homeService = {
  getFeaturedDeals: () => homeApi.featuredDeals(5),

  getTrendingDestinations: () => homeApi.trendingCities(5),

  getRecentlyVisited: () => homeApi.recentHotels(5),
}
