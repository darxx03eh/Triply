import type { RoomType } from './room'

/** A room the guest added to the cart from the hotel page. */
export interface CartItem {
  /** Identifier of the persisted cart row; use it when removing the item. */
  cartItemId: string
  roomId: string
  hotelId: string
  hotelName: string
  cityName: string
  hotelAddress: string | null
  roomNumber: string
  roomType: RoomType
  pricePerNight: number
  adults: number
  children: number
  checkIn: string
  checkOut: string
  imageUrl: string | null
  nights: number
  originalPrice: number
  discountAmount: number
  totalPrice: number
  isAvailable: boolean
}

/** Payload accepted by POST /api/v1/cart/items. */
export interface AddCartItemInput {
  roomId: string
  checkIn: string
  checkOut: string
  adults: number
  children: number
}

/** Server-owned cart totals and availability. */
export interface CartResponse {
  items: CartItem[]
  itemsCount: number
  originalPrice: number
  discountAmount: number
  totalPrice: number
}

export interface CheckoutInput {
  firstName: string
  lastName: string
  email: string
  phoneNumber: string
  specialRequests?: string
}

export type BookingStatus = 'Pending' | 'Confirmed' | 'Cancelled' | 'Completed'

/** Session returned when a payment is started for a pending booking. */
export interface PaymentSession {
  confirmationNumber: string
  provider: string
  sessionId: string
  checkoutUrl: string | null
  amount: number
  currency: string
  bookingStatus: BookingStatus
}

/** A booking row returned by the bookings API. */
export interface BookingLine {
  bookingId: string
  confirmationNumber: string
  hotelId: string
  hotelName: string
  cityName: string
  hotelAddress: string | null
  roomId: string
  roomNumber: string
  roomType: RoomType
  checkIn: string
  checkOut: string
  nights: number
  adults: number
  children: number
  discountAmount: number
  totalPrice: number
  status: BookingStatus
  createdAt: string
}

/** Response returned by checkout, booking lookup and cancellation endpoints. */
export interface BookingConfirmation {
  confirmationNumber: string
  status: BookingStatus
  guestFullName: string
  guestEmail: string
  guestPhoneNumber: string | null
  specialRequests: string | null
  discountAmount: number
  totalPrice: number
  createdAt: string
  bookings: BookingLine[]
}
