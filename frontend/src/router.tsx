import { createBrowserRouter } from 'react-router'
import { PublicLayout } from '@/components/layout/PublicLayout'
import { PageLoader } from '@/components/ui/Feedback'
import { GuestOnly, RequireAdmin, RequireAuth } from '@/features/auth/guards'
import { HomePage } from '@/pages/HomePage'
import { NotFoundPage } from '@/pages/NotFoundPage'
import { RouteErrorPage } from '@/pages/RouteErrorPage'

/** Route-level code splitting: every page except the home page is loaded on demand. */
const page = <T extends Record<string, React.ComponentType>>(load: () => Promise<T>, name: keyof T) => async () => ({
  Component: (await load())[name],
})

export const router = createBrowserRouter([
  {
    errorElement: <RouteErrorPage />,
    HydrateFallback: PageLoader,
    children: [
      {
        element: <PublicLayout />,
        children: [
          { index: true, element: <HomePage /> },
          { path: 'search', lazy: page(() => import('@/pages/SearchPage'), 'SearchPage') },
          { path: 'hotels/:hotelId', lazy: page(() => import('@/pages/HotelPage'), 'HotelPage') },
          {
            element: <RequireAuth />,
            children: [
              { path: 'checkout', lazy: page(() => import('@/pages/CheckoutPage'), 'CheckoutPage') },
              { path: 'bookings', lazy: page(() => import('@/pages/BookingsPage'), 'BookingsPage') },
              { path: 'booking/:confirmationNumber', lazy: page(() => import('@/pages/ConfirmationPage'), 'ConfirmationPage') },
            ],
          },
          { path: '*', element: <NotFoundPage /> },
        ],
      },
      {
        element: <GuestOnly />,
        children: [
          { path: 'login', lazy: page(() => import('@/pages/auth/LoginPage'), 'LoginPage') },
          { path: 'register', lazy: page(() => import('@/pages/auth/RegisterPage'), 'RegisterPage') },
        ],
      },
      { path: 'confirm-email', lazy: page(() => import('@/pages/auth/ConfirmEmailPage'), 'ConfirmEmailPage') },
      {
        path: 'admin',
        element: <RequireAdmin />,
        children: [
          {
            lazy: page(() => import('@/components/layout/AdminLayout'), 'AdminLayout'),
            children: [
              { index: true, lazy: page(() => import('@/pages/admin/AdminDashboardPage'), 'AdminDashboardPage') },
              { path: 'cities', lazy: page(() => import('@/pages/admin/AdminCitiesPage'), 'AdminCitiesPage') },
              { path: 'hotels', lazy: page(() => import('@/pages/admin/AdminHotelsPage'), 'AdminHotelsPage') },
              { path: 'rooms', lazy: page(() => import('@/pages/admin/AdminRoomsPage'), 'AdminRoomsPage') },
              { path: 'amenities', lazy: page(() => import('@/pages/admin/AdminAmenitiesPage'), 'AdminAmenitiesPage') },
              { path: 'deals', lazy: page(() => import('@/pages/admin/AdminDealsPage'), 'AdminDealsPage') },
              { path: 'users', lazy: page(() => import('@/pages/admin/AdminUsersPage'), 'AdminUsersPage') },
            ],
          },
        ],
      },
    ],
  },
])
