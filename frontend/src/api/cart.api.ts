import { http, unwrap } from '@/lib/http'
import type { ApiResponse } from '@/types/api'
import type { AddCartItemInput, CartItem, CartResponse } from '@/types/booking'

type ApiCartItem = Omit<CartItem, 'imageUrl' | 'hotelAddress'> & { thumbnailUrl: string | null }
type ApiCartResponse = Omit<CartResponse, 'items'> & { items: ApiCartItem[] }

const toCart = (cart: ApiCartResponse): CartResponse => ({
  ...cart,
  items: cart.items.map(({ thumbnailUrl, ...item }) => ({ ...item, imageUrl: thumbnailUrl, hotelAddress: null })),
})

/** Triply.Api/Endpoints/CartEndpoints.cs */
export const cartApi = {
  get: () => unwrap(http.get<ApiResponse<ApiCartResponse>>('/cart')).then(toCart),

  add: (body: AddCartItemInput) => unwrap(http.post<ApiResponse<ApiCartResponse>>('/cart/items', body)).then(toCart),

  remove: (cartItemId: string) => http.delete(`/cart/items/${cartItemId}`).then(() => undefined),

  clear: () => http.delete('/cart').then(() => undefined),
}
