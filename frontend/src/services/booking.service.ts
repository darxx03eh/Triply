import { http, unwrap } from '@/lib/http'
import { toSieveParams, type SieveQuery } from '@/lib/sieve'
import type { ApiResponse, PagedResult } from '@/types/api'
import type { BookingConfirmation, BookingLine, CheckoutInput, PaymentSession } from '@/types/booking'

export const bookingService = {
  /** POST /api/v1/bookings — turns the signed-in user's server cart into bookings. */
  create: (input: CheckoutInput) =>
    unwrap(http.post<ApiResponse<BookingConfirmation>>('/bookings', {
      guestFullName: `${input.firstName.trim()} ${input.lastName.trim()}`,
      guestEmail: input.email,
      guestPhoneNumber: input.phoneNumber || null,
      specialRequests: input.specialRequests?.trim() || null,
    })),

  /** GET /api/v1/users/me/bookings. */
  listMine: (query: SieveQuery = {}) =>
    unwrap(http.get<ApiResponse<PagedResult<BookingLine>>>('/users/me/bookings', { params: toSieveParams(query) })),

  /** GET /api/v1/bookings/{confirmationNumber}. */
  getByConfirmationNumber: (confirmationNumber: string) =>
    unwrap(http.get<ApiResponse<BookingConfirmation>>(`/bookings/${encodeURIComponent(confirmationNumber)}`)),

  /** POST /api/v1/bookings/{confirmationNumber}/cancel. */
  cancel: (confirmationNumber: string) =>
    unwrap(http.post<ApiResponse<BookingConfirmation>>(`/bookings/${encodeURIComponent(confirmationNumber)}/cancel`)),

  /** POST /api/v1/bookings/{confirmationNumber}/pay. */
  pay: (confirmationNumber: string) =>
    unwrap(http.post<ApiResponse<PaymentSession>>(`/bookings/${encodeURIComponent(confirmationNumber)}/pay`)),

  /** GET /api/v1/bookings/{confirmationNumber}/invoice — downloads the generated PDF. */
  downloadInvoice: async (confirmationNumber: string) => {
    const response = await http.get(`/bookings/${encodeURIComponent(confirmationNumber)}/invoice`, { responseType: 'blob' })
    const disposition = response.headers['content-disposition'] as string | undefined
    const fileName = disposition?.match(/filename="?([^";]+)"?/i)?.[1] ?? `invoice-${confirmationNumber}.pdf`
    const url = URL.createObjectURL(new Blob([response.data], { type: 'application/pdf' }))
    const link = document.createElement('a')
    link.href = url
    link.download = fileName
    document.body.appendChild(link)
    link.click()
    link.remove()
    URL.revokeObjectURL(url)
  },
}
