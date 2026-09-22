import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { QueryClientProvider } from '@tanstack/react-query'
import { RouterProvider } from 'react-router'
import { Toaster } from 'sonner'
import '@fontsource-variable/plus-jakarta-sans'
import './styles/index.css'
import { AuthProvider } from '@/features/auth/AuthProvider'
import { CartProvider } from '@/features/cart/CartProvider'
import { queryClient } from '@/lib/queryClient'
import { router } from './router'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <CartProvider>
          <RouterProvider router={router} />
          <Toaster position="top-right" richColors closeButton toastOptions={{ className: 'font-sans' }} />
        </CartProvider>
      </AuthProvider>
    </QueryClientProvider>
  </StrictMode>,
)
