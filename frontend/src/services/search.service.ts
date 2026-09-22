import { searchApi } from '@/api/search.api'
import type { HotelSort, SearchHotelsResponse, SearchParamsState } from '@/types/search'

const sorts: Record<HotelSort, string> = {
  recommended: 'recommended',
  'price-asc': 'price_asc',
  'price-desc': 'price_desc',
  'stars-desc': 'stars_desc',
  'stars-asc': 'stars_asc',
  'name-asc': 'name',
}

/** GET /api/v1/search — availability for the dates and guests, filters, sorting and paging in one call. */
export const searchService = {
  searchHotels: (state: SearchParamsState, page: number, pageSize = 8): Promise<SearchHotelsResponse> =>
    searchApi.hotels({
      q: state.q,
      cityId: state.cityId,
      checkIn: state.checkIn,
      checkOut: state.checkOut,
      adults: state.adults,
      children: state.children,
      rooms: state.rooms,
      minPrice: state.minPrice,
      maxPrice: state.maxPrice,
      stars: state.stars,
      types: state.types,
      amenities: state.amenities,
      sort: sorts[state.sort] ?? 'recommended',
      page,
      pageSize,
    }),
}
