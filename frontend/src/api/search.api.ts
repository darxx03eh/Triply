import { http, unwrap } from '@/lib/http'
import type { ApiResponse } from '@/types/api'
import type { SearchHotelsQuery, SearchHotelsResponse } from '@/types/search'

/** Arrays are sent as repeated keys (stars=4&stars=5), which is what ASP.NET Core binds to int[]. */
function toQueryString(query: SearchHotelsQuery) {
  const params = new URLSearchParams()
  Object.entries(query).forEach(([key, value]) => {
    if (value === undefined || value === null || value === '') return
    if (Array.isArray(value)) value.forEach((item) => params.append(key, String(item)))
    else params.append(key, String(value))
  })
  return params.toString()
}

/** Triply.Api/Endpoints/SearchEndpoints.cs */
export const searchApi = {
  hotels: (query: SearchHotelsQuery) =>
    unwrap(http.get<ApiResponse<SearchHotelsResponse>>(`/search?${toQueryString(query)}`)),
}
