import { http, unwrap } from '@/lib/http'
import { toSieveParams, type SieveQuery } from '@/lib/sieve'
import type { ApiResponse, PagedResult } from '@/types/api'
import type { City, CityInput, UpdateCityInput } from '@/types/city'

/** Triply.Api/Endpoints/CityEndpoints.cs */
export const citiesApi = {
  list: (query: SieveQuery = {}) =>
    unwrap(http.get<ApiResponse<PagedResult<City>>>('/cities', { params: toSieveParams(query) })),

  get: (id: string) => unwrap(http.get<ApiResponse<City>>(`/cities/${id}`)),

  create: (body: CityInput) => unwrap(http.post<ApiResponse<City>>('/cities', body)),

  update: (id: string, body: UpdateCityInput) => unwrap(http.put<ApiResponse<City>>(`/cities/${id}`, body)),

  remove: (id: string) => http.delete(`/cities/${id}`).then(() => undefined),

  uploadThumbnail: (id: string, file: File) => {
    const form = new FormData()
    form.append('file', file)
    return unwrap(http.post<ApiResponse<City>>(`/cities/${id}/thumbnail`, form))
  },

  removeThumbnail: (id: string) => http.delete(`/cities/${id}/thumbnail`).then(() => undefined),
}
