import { http, unwrap } from '@/lib/http'
import { toSieveParams, type SieveQuery } from '@/lib/sieve'
import type { ApiResponse, PagedResult } from '@/types/api'
import type { User } from '@/types/user'

/** Triply.Api/Endpoints/UserEndpoints.cs */
export const usersApi = {
  list: (query: SieveQuery = {}) =>
    unwrap(http.get<ApiResponse<PagedResult<User>>>('/users', { params: toSieveParams(query) })),
}
