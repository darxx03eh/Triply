import { http, unwrap } from '@/lib/http'
import { toSieveParams, type SieveQuery } from '@/lib/sieve'
import type { ApiResponse, PagedResult } from '@/types/api'
import type { CreateDealInput, Deal, DealInput } from '@/types/deal'

/**
 * Triply.Api/Endpoints/DealEndpoints.cs (admin only)
 * Sieve names: RoomId, Title, discount, featured, StartsAt, EndsAt, CreatedAt.
 */
export const dealsApi = {
  list: (query: SieveQuery = {}) =>
    unwrap(http.get<ApiResponse<PagedResult<Deal>>>('/deals', { params: toSieveParams(query) })),

  get: (id: string) => unwrap(http.get<ApiResponse<Deal>>(`/deals/${id}`)),

  create: (body: CreateDealInput) => unwrap(http.post<ApiResponse<Deal>>('/deals', body)),

  update: (id: string, body: DealInput) => unwrap(http.put<ApiResponse<Deal>>(`/deals/${id}`, body)),

  remove: (id: string) => http.delete(`/deals/${id}`).then(() => undefined),
}
