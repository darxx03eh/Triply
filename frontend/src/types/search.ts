import type { HotelType } from './hotel'

export type HotelSort = 'recommended' | 'price-asc' | 'price-desc' | 'stars-desc' | 'stars-asc' | 'name-asc'

/** Everything the search page reads from / writes to the URL. */
export interface SearchParamsState {
  q: string
  cityId: string
  checkIn: string
  checkOut: string
  adults: number
  children: number
  rooms: number
  stars: number[]
  types: HotelType[]
  minPrice: number | null
  maxPrice: number | null
  amenities: string[]
  sort: HotelSort
}

/** Triply.Application/DTOs/Search/HotelSearchItemResponse.cs */
export interface HotelSearchItem {
  hotelId: string
  name: string
  cityId: string
  cityName: string
  country: string
  starRating: number
  hotelType: HotelType
  address: string | null
  description: string | null
  thumbnailUrl: string | null
  latitude: number | null
  longitude: number | null
  minPricePerNight: number
  totalPrice: number
  availableRooms: number
  amenities: string[]
}

/** Triply.Application/DTOs/Search/SearchHotelsResponse.cs */
export interface SearchHotelsResponse {
  items: HotelSearchItem[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
  checkIn: string
  checkOut: string
  nights: number
  adults: number
  children: number
  rooms: number
}

/** Query string accepted by GET /api/v1/search. */
export interface SearchHotelsQuery {
  q?: string
  cityId?: string
  checkIn?: string
  checkOut?: string
  adults?: number
  children?: number
  rooms?: number
  minPrice?: number | null
  maxPrice?: number | null
  stars?: number[]
  types?: HotelType[]
  amenities?: string[]
  sort?: string
  page?: number
  pageSize?: number
}
