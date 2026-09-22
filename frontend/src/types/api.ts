/** Envelope returned by every Triply API endpoint (Triply.Api/Responses/ApiResponse.cs). */
export interface ApiResponse<T> {
  data: T | null
  message: string
  code: string
  errors: ApiErrors | null
}

export interface ApiErrors {
  fields: Record<string, string[]>
  general: string[]
}

/** Triply.Application/Common/Models/PagedResult.cs */
export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}
