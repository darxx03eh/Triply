import { http, unwrap } from '@/lib/http'
import { toSieveParams, type SieveQuery } from '@/lib/sieve'
import type { ApiResponse, PagedResult } from '@/types/api'
import type { Review, ReviewInput } from '@/types/review'

/**
 * Triply.Api/Endpoints/ReviewEndpoints.cs
 * Sieve names: Rating (filter / sort), CreatedAt (sort). Newest first by default.
 */
export const reviewsApi = {
  byHotel: (hotelId: string, query: SieveQuery = {}) =>
    unwrap(http.get<ApiResponse<PagedResult<Review>>>(`/hotels/${hotelId}/reviews`, { params: toSieveParams(query) })),

  create: (hotelId: string, body: ReviewInput) =>
    unwrap(http.post<ApiResponse<Review>>(`/hotels/${hotelId}/reviews`, body)),

  update: (id: string, body: ReviewInput) => unwrap(http.put<ApiResponse<Review>>(`/reviews/${id}`, body)),

  remove: (id: string) => http.delete(`/reviews/${id}`).then(() => undefined),
}
