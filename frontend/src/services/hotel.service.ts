import { attractionsApi } from '@/api/attractions.api'
import { hotelsApi } from '@/api/hotels.api'
import { reviewsApi } from '@/api/reviews.api'
import { roomsApi } from '@/api/rooms.api'
import { ApiError } from '@/lib/http'
import type { HotelDetails } from '@/types/hotel'

export const REVIEWS_PAGE_SIZE = 6

export const hotelService = {
  /** Public. For a signed-in user the backend also records the visit (recently visited). */
  async getDetails(hotelId: string): Promise<HotelDetails | null> {
    try {
      return await hotelsApi.get(hotelId)
    } catch (error) {
      if (error instanceof ApiError && error.status === 404) return null
      throw error
    }
  },

  getRooms: (hotelId: string) => roomsApi.byHotel(hotelId, { sorts: 'price', pageSize: 50 }),

  getReviews: (hotelId: string, page: number, sorts: string) =>
    reviewsApi.byHotel(hotelId, { sorts, page, pageSize: REVIEWS_PAGE_SIZE }),

  getNearbyAttractions: (hotelId: string) => attractionsApi.byHotel(hotelId),
}
