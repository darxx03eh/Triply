export interface TokenClaims {
  id: string
  username: string
  email: string
  phoneNumber: string
  roles: string[]
  /** Expiry as a unix timestamp in seconds. */
  exp: number
}

export function decodeToken(token: string): TokenClaims | null {
  try {
    const payload = token.split('.')[1]
    const json = decodeURIComponent(
      atob(payload.replace(/-/g, '+').replace(/_/g, '/'))
        .split('')
        .map((c) => `%${c.charCodeAt(0).toString(16).padStart(2, '0')}`)
        .join(''),
    )
    const claims = JSON.parse(json) as Record<string, unknown>
    const role = claims.role
    return {
      id: String(claims.id ?? ''),
      username: String(claims.username ?? ''),
      email: String(claims.email ?? ''),
      phoneNumber: String(claims.phoneNumber ?? ''),
      roles: Array.isArray(role) ? role.map(String) : role ? [String(role)] : [],
      exp: Number(claims.exp ?? 0),
    }
  } catch {
    return null
  }
}

export const secondsUntilExpiry = (claims: TokenClaims) => claims.exp - Math.floor(Date.now() / 1000)
