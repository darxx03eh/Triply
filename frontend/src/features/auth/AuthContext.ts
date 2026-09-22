import { createContext } from 'react'
import type { CurrentUser, LoginRequest, RegisterRequest, RegisterResponse } from '@/types/auth'

export interface AuthContextValue {
  user: CurrentUser | null
  isAuthenticated: boolean
  login: (body: LoginRequest) => Promise<CurrentUser>
  register: (body: RegisterRequest) => Promise<RegisterResponse>
  logout: () => Promise<void>
}

export const AuthContext = createContext<AuthContextValue | null>(null)
