import { useCallback, useEffect, useMemo, useState, type ReactNode } from 'react'
import { useQueryClient } from '@tanstack/react-query'
import { authApi } from '@/api/auth.api'
import { decodeToken, secondsUntilExpiry } from '@/lib/jwt'
import { tokenStore, type Session } from '@/lib/tokenStore'
import type { CurrentUser, LoginRequest, RegisterRequest } from '@/types/auth'
import { AuthContext } from './AuthContext'

/** Refresh this many seconds before the access token expires (the API rejects expired tokens on /refresh). */
const REFRESH_MARGIN_SECONDS = 60

function toUser(session: Session | null): CurrentUser | null {
  if (!session) return null
  const claims = decodeToken(session.access)
  if (!claims || secondsUntilExpiry(claims) <= 0) return null
  return {
    id: claims.id,
    name: session.name,
    username: claims.username,
    email: claims.email,
    phoneNumber: claims.phoneNumber,
    roles: claims.roles,
    isAdmin: claims.roles.includes('Admin'),
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const queryClient = useQueryClient()
  const [session, setSession] = useState<Session | null>(() => {
    const stored = tokenStore.get()
    if (stored && !toUser(stored)) {
      tokenStore.set(null)
      return null
    }
    return stored
  })

  useEffect(() => tokenStore.subscribe(setSession), [])

  // Silent refresh shortly before the access token expires.
  useEffect(() => {
    const claims = session ? decodeToken(session.access) : null
    if (!claims) return
    const wait = Math.max(5, secondsUntilExpiry(claims) - REFRESH_MARGIN_SECONDS) * 1000
    const timer = window.setTimeout(async () => {
      try {
        const refreshed = await authApi.refresh()
        tokenStore.set({ access: refreshed.access, name: refreshed.name })
      } catch {
        tokenStore.set(null)
      }
    }, wait)
    return () => window.clearTimeout(timer)
  }, [session])

  const login = useCallback(
    async (body: LoginRequest) => {
      const response = await authApi.login(body)
      const next = { access: response.access, name: response.name }
      tokenStore.set(next)
      queryClient.clear()
      return toUser(next)!
    },
    [queryClient],
  )

  const logout = useCallback(async () => {
    try {
      await authApi.logout()
    } finally {
      tokenStore.set(null)
      queryClient.clear()
    }
  }, [queryClient])

  const register = useCallback((body: RegisterRequest) => authApi.register(body), [])

  const value = useMemo(() => {
    const user = toUser(session)
    return { user, isAuthenticated: !!user, login, logout, register }
  }, [session, login, logout, register])

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
