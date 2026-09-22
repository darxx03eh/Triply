import { QueryClient } from '@tanstack/react-query'
import { ApiError } from './http'

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 30_000,
      refetchOnWindowFocus: false,
      retry: (count, error) => {
        const status = error instanceof ApiError ? error.status : 0
        return status >= 500 || status === 0 ? count < 2 : false
      },
    },
  },
})
