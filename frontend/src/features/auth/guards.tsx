import { Navigate, Outlet, useLocation } from 'react-router'
import { useAuth } from './useAuth'

export function RequireAuth() {
  const { isAuthenticated } = useAuth()
  const location = useLocation()
  if (!isAuthenticated) return <Navigate to="/login" replace state={{ from: location.pathname + location.search }} />
  return <Outlet />
}

export function RequireAdmin() {
  const { user } = useAuth()
  const location = useLocation()
  if (!user) return <Navigate to="/login" replace state={{ from: location.pathname }} />
  if (!user.isAdmin) return <Navigate to="/" replace />
  return <Outlet />
}

export function GuestOnly() {
  const { user } = useAuth()
  if (user) return <Navigate to={user.isAdmin ? '/admin' : '/'} replace />
  return <Outlet />
}
