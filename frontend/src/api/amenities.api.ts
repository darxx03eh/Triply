import { http, unwrap } from '@/lib/http'
import type { ApiResponse } from '@/types/api'
import type { Amenity } from '@/types/amenity'

/** Triply.Api/Endpoints/AmenityEndpoints.cs */
export const amenitiesApi = {
  list: () => unwrap(http.get<ApiResponse<Amenity[]>>('/amenities')),

  create: (name: string) => unwrap(http.post<ApiResponse<Amenity>>('/amenities', { name })),

  update: (id: string, name: string) => unwrap(http.put<ApiResponse<Amenity>>(`/amenities/${id}`, { name })),

  remove: (id: string) => http.delete(`/amenities/${id}`).then(() => undefined),

  byHotel: (hotelId: string) => unwrap(http.get<ApiResponse<Amenity[]>>(`/hotels/${hotelId}/amenities`)),

  setForHotel: (hotelId: string, amenityIds: string[]) =>
    unwrap(http.put<ApiResponse<Amenity[]>>(`/hotels/${hotelId}/amenities`, { amenityIds })),
}
