import { Link, useParams } from 'react-router'
import { useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { CalendarDays, CheckCircle2, Download, Mail, MapPin, Printer, Users } from 'lucide-react'
import { toast } from 'sonner'
import { Logo } from '@/components/layout/Logo'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { Badge, EmptyState, PageLoader } from '@/components/ui/Feedback'
import { ConfirmDialog } from '@/components/ui/Overlay'
import { formatDate, formatDateTime, formatMoney } from '@/lib/format'
import { toastError } from '@/lib/forms'
import { bookingService } from '@/services/booking.service'

export function ConfirmationPage() {
  const { confirmationNumber = '' } = useParams()
  const queryClient = useQueryClient()
  const [cancelConfirmationOpen, setCancelConfirmationOpen] = useState(false)
  const booking = useQuery({
    queryKey: ['booking', confirmationNumber],
    queryFn: () => bookingService.getByConfirmationNumber(confirmationNumber),
  })
  const cancel = useMutation({
    mutationFn: () => bookingService.cancel(confirmationNumber),
    onSuccess: (result) => {
      setCancelConfirmationOpen(false)
      queryClient.setQueryData(['booking', confirmationNumber], result)
    },
    onError: toastError,
  })
  const pay = useMutation({
    mutationFn: () => bookingService.pay(confirmationNumber),
    onSuccess: async (payment) => {
      if (payment.checkoutUrl) {
        window.location.assign(payment.checkoutUrl)
        return
      }
      await queryClient.invalidateQueries({ queryKey: ['booking', confirmationNumber] })
      toast.success('Payment completed — your booking is confirmed.')
    },
    onError: toastError,
  })
  const invoice = useMutation({
    mutationFn: () => bookingService.downloadInvoice(confirmationNumber),
    onError: toastError,
  })

  if (booking.isLoading) return <div className="pt-16"><PageLoader /></div>
  if (!booking.data)
    return (
      <div className="container-page pt-28">
        <EmptyState title="Booking not found" description="Check the confirmation number and try again." action={<Link to="/"><Button>Go home</Button></Link>} />
      </div>
    )

  const b = booking.data

  return (
    <div className="container-page max-w-4xl pt-24 pb-8">
      <div className="no-print mb-8 text-center">
        <span className="mx-auto flex size-16 items-center justify-center rounded-full bg-emerald-100 text-emerald-600">
          <CheckCircle2 className="size-9" />
        </span>
        <h1 className="mt-4 text-3xl font-extrabold tracking-tight text-slate-900">
          {b.status === 'Pending' ? 'Your booking has been submitted!' : b.status === 'Cancelled' ? 'Your booking was cancelled' : 'Your booking is confirmed!'}
        </h1>
        <p className="mt-2 text-slate-500">
          <Mail className="mr-1.5 inline size-4 align-[-2px]" />
          We sent your booking details to <span className="font-semibold break-all text-slate-700">{b.guestEmail}</span>
        </p>
        <div className="mt-6 flex justify-center gap-3">
          <Button variant="outline" onClick={() => window.print()}><Printer className="size-4" /> Print</Button>
          {(b.status === 'Confirmed' || b.status === 'Completed') && (
            <Button loading={invoice.isPending} onClick={() => invoice.mutate()}><Download className="size-4" /> Download invoice</Button>
          )}
        </div>
      </div>

      <Card className="overflow-hidden print:border-0 print:shadow-none">
        <div className="flex flex-wrap items-center justify-between gap-4 bg-slate-950 px-8 py-6 text-white print:bg-white print:text-slate-900">
          <Logo light />
          <div className="text-right">
            <p className="text-xs tracking-widest text-white/60 uppercase print:text-slate-500">Confirmation number</p>
            <p className="font-mono text-xl font-bold">{b.confirmationNumber}</p>
          </div>
        </div>

        <div className="grid gap-6 border-b border-slate-100 p-8 sm:grid-cols-3">
          <Info label="Guest" value={b.guestFullName} />
          <Info label="Booked on" value={formatDateTime(b.createdAt)} />
          <div>
            <p className="text-xs font-bold tracking-wide text-slate-400 uppercase">Status</p>
            <div className="mt-1.5 flex gap-2">
              <Badge tone="green">{b.status}</Badge>
            </div>
          </div>
        </div>

        <div className="space-y-4 p-8">
          {b.bookings.map((line) => (
            <div key={line.bookingId} className="rounded-2xl border border-slate-200 p-5">
              <div className="flex flex-wrap items-start justify-between gap-3">
                <div>
                  <h3 className="text-lg font-bold text-slate-900">{line.hotelName}</h3>
                  <p className="mt-1 flex items-center gap-1.5 text-sm text-slate-500"><MapPin className="size-4" /> {line.hotelAddress}</p>
                </div>
                <p className="text-lg font-extrabold text-slate-900">{formatMoney(line.totalPrice, true)}</p>
              </div>
              <div className="mt-4 grid gap-3 text-sm text-slate-600 sm:grid-cols-3">
                <span className="flex items-center gap-1.5"><CalendarDays className="size-4 text-brand-600" /> {formatDate(line.checkIn)} → {formatDate(line.checkOut)}</span>
                <span>{line.roomType} · Room {line.roomNumber} · {line.nights} night{line.nights > 1 ? 's' : ''}</span>
                <span className="flex items-center gap-1.5"><Users className="size-4 text-brand-600" /> {line.adults} adults{line.children > 0 && `, ${line.children} children`}</span>
              </div>
            </div>
          ))}
          {b.specialRequests && (
            <div className="rounded-2xl bg-slate-50 p-5 text-sm">
              <p className="font-semibold text-slate-900">Special requests</p>
              <p className="mt-1 text-slate-600">{b.specialRequests}</p>
            </div>
          )}
        </div>

        <div className="ml-auto max-w-sm space-y-2 px-8 pb-8 text-sm">
          {b.discountAmount > 0 && <div className="flex justify-between text-slate-600"><span>Discount</span><span className="font-semibold text-emerald-700">−{formatMoney(b.discountAmount, true)}</span></div>}
          <div className="flex justify-between border-t border-slate-200 pt-3 text-lg font-extrabold text-slate-900"><span>Total</span><span>{formatMoney(b.totalPrice, true)}</span></div>
        </div>
      </Card>

      <div className="no-print mt-8 text-center">
        {b.status === 'Pending' && (
          <Button loading={pay.isPending} onClick={() => pay.mutate()}>Pay now</Button>
        )}
        {(b.status === 'Pending' || b.status === 'Confirmed') && (
          <Button variant="danger" loading={cancel.isPending} onClick={() => setCancelConfirmationOpen(true)}>Cancel booking</Button>
        )}
        <Link to="/search"><Button variant="ghost">Continue exploring</Button></Link>
      </div>
      <ConfirmDialog
        open={cancelConfirmationOpen}
        onClose={() => setCancelConfirmationOpen(false)}
        onConfirm={() => cancel.mutate()}
        loading={cancel.isPending}
        title="Cancel booking?"
        description="This booking will be cancelled and cannot be restored."
        confirmLabel="Cancel booking"
      />
    </div>
  )
}

function Info({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <p className="text-xs font-bold tracking-wide text-slate-400 uppercase">{label}</p>
      <p className="mt-1 font-semibold text-slate-900">{value}</p>
    </div>
  )
}
