import { createContext } from 'react'
import type { AddCartItemInput, CartItem } from '@/types/booking'

export interface CartContextValue {
  items: CartItem[]
  count: number
  subtotal: number
  isLoading: boolean
  add: (item: AddCartItemInput) => Promise<void>
  remove: (roomId: string) => Promise<void>
  clear: () => Promise<void>
  has: (roomId: string) => boolean
}

export const CartContext = createContext<CartContextValue | null>(null)
