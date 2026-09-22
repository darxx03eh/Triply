import { http, unwrap } from '@/lib/http'
import type { ApiResponse } from '@/types/api'
import type { LoginRequest, LoginResponse, RegisterRequest, RegisterResponse } from '@/types/auth'

/** Triply.Api/Endpoints/AuthenticationEndpoints.cs */
export const authApi = {
  login: (body: LoginRequest) => unwrap(http.post<ApiResponse<LoginResponse>>('/auth/login', body)),

  register: (body: RegisterRequest) => unwrap(http.post<ApiResponse<RegisterResponse>>('/auth/register', body)),

  /** Needs the current (still valid) access token; the refresh token travels in the HttpOnly cookie. */
  refresh: () => unwrap(http.post<ApiResponse<LoginResponse>>('/auth/refresh')),

  logout: () => http.delete('/auth/logout').then(() => undefined),

  confirmEmail: (email: string, token: string) =>
    http.get<ApiResponse<unknown>>('/auth/confirm-email', { params: { email, token } }).then((r) => r.data),
}
