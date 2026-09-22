import type { RoomType } from './room'

export const HOTEL_TYPES = ['Budget', 'Boutique', 'Luxury'] as const
export type HotelType = (typeof HOTEL_TYPES)[number]

export interface HotelSummary {
  hotelId: string
  name: string
  cityName: string
  ownerId: string | null
  ownerName: string | null
  roomsCount: number
  starRating: number
  hotelType: HotelType
  address: string | null
  thumbnailUrl: string | null
  latitude: number | null
  longitude: number | null
  createdAt: string
  modifiedAt: string | null
}

export interface HotelDetails {
  hotelId: string
  name: string
  cityId: string
  cityName: string
  ownerId: string | null
  starRating: number
  hotelType: HotelType
  address: string | null
  description: string | null
  latitude: number | null
  longitude: number | null
  imageUrls: string[]
  amenities: string[]
  averageRating: number | null
  reviewsCount: number
  createdAt: string
  modifiedAt: string | null
  rowVersion: string
}

export interface HotelInput {
  name: string
  cityId: string
  starRating: number
  hotelType: HotelType
  address: string
  description?: string | null
  latitude?: number | null
  longitude?: number | null
}

export interface UpdateHotelInput extends HotelInput {
  ownerId?: string | null
  rowVersion: string
}

export type HotelImageStatus = 'Pending' | 'Uploaded' | 'Failed'

export interface HotelImage {
  imageId: string
  hotelId: string
  url: string | null
  displayOrder: number
  status: HotelImageStatus
}

/** Triply.Application/DTOs/Deals/FeaturedDealResponse.cs */
export interface FeaturedDeal {
  dealId: string
  title: string
  hotelId: string
  hotelName: string
  cityName: string
  country: string
  starRating: number
  thumbnailUrl: string | null
  roomId: string
  roomType: RoomType
  originalPrice: number
  discountedPrice: number
  discountPercentage: number
  endsAt: string
}

/** Triply.Application/DTOs/Home/RecentHotelResponse.cs */
export interface RecentHotel {
  hotelId: string
  name: string
  cityName: string
  country: string
  starRating: number
  thumbnailUrl: string | null
  minPricePerNight: number | null
  visitedAt: string
}

/** Triply.Application/DTOs/Attractions/AttractionResponse.cs */
export interface Attraction {
  attractionId: string
  hotelId: string
  name: string
  category: string
  distanceKm: number
}

export interface AttractionInput {
  name: string
  category: string
  distanceKm: number
}
