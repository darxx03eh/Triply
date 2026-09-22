import { useCallback, useMemo, type ReactNode } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { cartApi } from '@/api/cart.api'
import { useAuth } from '@/features/auth/useAuth'
import type { AddCartItemInput, CartItem, CartResponse } from '@/types/booking'
import { CartContext } from './CartContext'

const emptyCart: CartResponse = { items: [], itemsCount: 0, originalPrice: 0, discountAmount: 0, totalPrice: 0 }

export function CartProvider({ children }: { children: ReactNode }) {
  const { user } = useAuth()
  const queryClient = useQueryClient()
  const key = ['cart', user?.id] as const
  const cart = useQuery({ queryKey: key, queryFn: cartApi.get, enabled: !!user })
  const data = user ? (cart.data ?? emptyCart) : emptyCart

  const replace = useCallback((next: CartResponse) => queryClient.setQueryData(key, next), [key, queryClient])
  const addMutation = useMutation({ mutationFn: cartApi.add, onSuccess: replace })
  const removeMutation = useMutation({
    mutationFn: cartApi.remove,
    onSuccess: (_, cartItemId) => {
      replace({ ...data, items: data.items.filter((item) => item.cartItemId !== cartItemId), itemsCount: data.items.length - 1 })
      void queryClient.invalidateQueries({ queryKey: key })
    },
  })
  const clearMutation = useMutation({ mutationFn: cartApi.clear, onSuccess: () => replace(emptyCart) })

  const add = useCallback(async (item: AddCartItemInput) => { await addMutation.mutateAsync(item) }, [addMutation])
  const remove = useCallback(async (roomId: string) => {
    const item = data.items.find((entry) => entry.roomId === roomId)
    if (item) await removeMutation.mutateAsync(item.cartItemId)
  }, [data.items, removeMutation])
  const clear = useCallback(async () => { if (data.items.length) await clearMutation.mutateAsync() }, [clearMutation, data.items.length])

  const value = useMemo(
    () => ({
      items: data.items,
      count: data.itemsCount,
      subtotal: data.totalPrice,
      isLoading: cart.isLoading,
      add,
      remove,
      clear,
      has: (roomId: string) => data.items.some((i: CartItem) => i.roomId === roomId),
    }),
    [data, cart.isLoading, add, remove, clear],
  )

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>
}
