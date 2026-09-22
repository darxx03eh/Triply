import { http, unwrap } from '@/lib/http'
import type { ApiResponse } from '@/types/api'
import type { Attraction, AttractionInput } from '@/types/hotel'

/** Triply.Api/Endpoints/AttractionEndpoints.cs — ordered by distance. */
export const attractionsApi = {
  byHotel: (hotelId: string) => unwrap(http.get<ApiResponse<Attraction[]>>(`/hotels/${hotelId}/attractions`)),

  create: (hotelId: string, body: AttractionInput) =>
    unwrap(http.post<ApiResponse<Attraction>>(`/hotels/${hotelId}/attractions`, body)),

  update: (id: string, body: AttractionInput) =>
    unwrap(http.put<ApiResponse<Attraction>>(`/attractions/${id}`, body)),

  remove: (id: string) => http.delete(`/attractions/${id}`).then(() => undefined),
}
