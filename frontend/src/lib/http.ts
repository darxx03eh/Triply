import axios, { AxiosError } from 'axios'
import type { ApiResponse } from '@/types/api'
import { tokenStore } from './tokenStore'

export const API_BASE_URL = '/api/v1'

export const http = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true,
  headers: { Accept: 'application/json' },
})

http.interceptors.request.use((config) => {
  const session = tokenStore.get()
  if (session?.access) config.headers.Authorization = `Bearer ${session.access}`
  return config
})

http.interceptors.response.use(
  (response) => response,
  (error: AxiosError<ApiResponse<unknown>>) => {
    const url = error.config?.url ?? ''
    // An expired or revoked access token: drop the session so the UI returns to a guest state.
    if (error.response?.status === 401 && tokenStore.get() && !url.startsWith('/auth/')) {
      tokenStore.set(null)
    }
    return Promise.reject(ApiError.from(error))
  },
)

/** Normalized error thrown by every API call. */
export class ApiError extends Error {
  readonly status: number
  readonly code: string
  readonly fields: Record<string, string[]>
  readonly general: string[]

  constructor(message: string, status = 0, code = 'UNKNOWN', fields = {}, general: string[] = []) {
    super(message)
    this.status = status
    this.code = code
    this.fields = fields
    this.general = general
  }

  static from(error: unknown): ApiError {
    if (error instanceof ApiError) return error
    if (axios.isAxiosError(error)) {
      const body = error.response?.data as ApiResponse<unknown> | undefined
      if (!error.response) return new ApiError('Cannot reach the server. Check your connection.', 0, 'NETWORK')
      return new ApiError(
        body?.message || error.message,
        error.response.status,
        body?.code || 'HTTP_ERROR',
        body?.errors?.fields ?? {},
        body?.errors?.general ?? [],
      )
    }
    return new ApiError(error instanceof Error ? error.message : 'Unexpected error')
  }

  /** First message of every field, flattened — handy for toasts. */
  get details(): string[] {
    return [...this.general, ...Object.values(this.fields).map((messages) => messages[0])].filter(Boolean)
  }
}

/** Unwraps the ApiResponse envelope. */
export async function unwrap<T>(request: Promise<{ data: ApiResponse<T> }>): Promise<T> {
  const response = await request
  return response.data?.data as T
}
