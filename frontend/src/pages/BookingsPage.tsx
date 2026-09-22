import { Link } from 'react-router'
import { useQuery } from '@tanstack/react-query'
import { CalendarDays, MapPin, ReceiptText } from 'lucide-react'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { Badge, EmptyState, ErrorState, PageLoader } from '@/components/ui/Feedback'
import { Pagination } from '@/components/ui/Pagination'
import { formatDate, formatMoney } from '@/lib/format'
import { bookingService } from '@/services/booking.service'
import type { BookingStatus } from '@/types/booking'
import { useState } from 'react'

const statusTone: Record<BookingStatus, 'amber' | 'green' | 'red' | 'brand'> = {
  Pending: 'amber',
  Confirmed: 'green',
  Cancelled: 'red',
  Completed: 'brand',
}

export function BookingsPage() {
  const [page, setPage] = useState(1)
  const bookings = useQuery({
    queryKey: ['bookings', page],
    queryFn: () => bookingService.listMine({ page, pageSize: 10, sorts: '-CreatedAt' }),
  })

  if (bookings.isLoading) return <div className="pt-16"><PageLoader /></div>
  if (bookings.isError) return <div className="container-page pt-28"><ErrorState message={(bookings.error as Error).message} onRetry={() => bookings.refetch()} /></div>
  if (!bookings.data?.items.length)
    return (
      <div className="container-page pt-28">
        <EmptyState
          icon={<ReceiptText className="size-7" />}
          title="No bookings yet"
          description="When you book a room, it will appear here."
          action={<Link to="/search"><Button>Explore hotels</Button></Link>}
        />
      </div>
    )

  const data = bookings.data
  return (
    <div className="container-page max-w-5xl pt-24 pb-10">
      <div className="mb-8">
        <h1 className="text-3xl font-extrabold tracking-tight text-slate-900">My bookings</h1>
        <p className="mt-1 text-slate-500">Review, pay for, or download invoices for your stays.</p>
      </div>
      <Card className="overflow-hidden">
        <ul className="divide-y divide-slate-100">
          {data.items.map((booking) => (
            <li key={booking.bookingId} className="flex flex-col gap-4 p-5 sm:flex-row sm:items-center sm:justify-between">
              <div className="min-w-0">
                <div className="flex flex-wrap items-center gap-2">
                  <p className="font-bold text-slate-900">{booking.hotelName}</p>
                  <Badge tone={statusTone[booking.status]}>{booking.status}</Badge>
                </div>
                <p className="mt-1 flex items-center gap-1.5 text-sm text-slate-500"><MapPin className="size-4" /> {booking.hotelAddress ?? booking.cityName}</p>
                <p className="mt-2 flex flex-wrap items-center gap-x-4 gap-y-1 text-sm text-slate-600">
                  <span className="flex items-center gap-1.5"><CalendarDays className="size-4 text-brand-600" /> {formatDate(booking.checkIn)} – {formatDate(booking.checkOut)}</span>
                  <span>{booking.roomType} · Room {booking.roomNumber}</span>
                </p>
                <p className="mt-2 font-mono text-xs text-slate-400">{booking.confirmationNumber}</p>
              </div>
              <div className="flex items-center justify-between gap-4 sm:flex-col sm:items-end">
                <p className="font-extrabold text-slate-900">{formatMoney(booking.totalPrice, true)}</p>
                <Link to={`/booking/${encodeURIComponent(booking.confirmationNumber)}`}><Button variant="outline" size="sm">View booking</Button></Link>
              </div>
            </li>
          ))}
        </ul>
        <Pagination {...data} onChange={setPage} />
      </Card>
    </div>
  )
}
