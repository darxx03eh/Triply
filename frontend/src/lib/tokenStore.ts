import { storage } from './storage'

const KEY = 'triply.session'

export interface Session {
  access: string
  name: string
}

type Listener = (session: Session | null) => void

let current: Session | null = storage.get<Session | null>(KEY, null)
const listeners = new Set<Listener>()

/**
 * Keeps the access token in memory (mirrored to localStorage so a reload keeps the session).
 * The refresh token never touches JS: the API stores it in an HttpOnly cookie.
 */
export const tokenStore = {
  get: () => current,
  set(session: Session | null) {
    current = session
    if (session) storage.set(KEY, session)
    else storage.remove(KEY)
    listeners.forEach((listener) => listener(session))
  },
  subscribe(listener: Listener) {
    listeners.add(listener)
    return () => {
      listeners.delete(listener)
    }
  },
}
