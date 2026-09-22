import { http, unwrap } from '@/lib/http'
import { toSieveParams, type SieveQuery } from '@/lib/sieve'
import type { ApiResponse, PagedResult } from '@/types/api'
import type { HotelDetails, HotelImage, HotelInput, HotelSummary, UpdateHotelInput } from '@/types/hotel'

/** Triply.Api/Endpoints/HotelEndpoints.cs */
export const hotelsApi = {
  list: (query: SieveQuery = {}) =>
    unwrap(http.get<ApiResponse<PagedResult<HotelSummary>>>('/hotels', { params: toSieveParams(query) })),

  /** Admin only for now (backend Phase 4 makes it public). */
  get: (id: string) => unwrap(http.get<ApiResponse<HotelDetails>>(`/hotels/${id}`)),

  create: (body: HotelInput) => unwrap(http.post<ApiResponse<HotelDetails>>('/hotels', body)),

  update: (id: string, body: UpdateHotelInput) =>
    unwrap(http.put<ApiResponse<HotelDetails>>(`/hotels/${id}`, body)),

  remove: (id: string) => http.delete(`/hotels/${id}`).then(() => undefined),

  images: (id: string) => unwrap(http.get<ApiResponse<HotelImage[]>>(`/hotels/${id}/images`)),

  uploadImage: (id: string, file: File, displayOrder?: number) => {
    const form = new FormData()
    form.append('file', file)
    if (displayOrder) form.append('displayOrder', String(displayOrder))
    return unwrap(http.post<ApiResponse<HotelImage>>(`/hotels/${id}/images`, form))
  },

  removeImage: (id: string, imageId: string) =>
    http.delete(`/hotels/${id}/images/${imageId}`).then(() => undefined),
}
