import { Link, useNavigate } from 'react-router'
import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { useMutation } from '@tanstack/react-query'
import { z } from 'zod'
import { CalendarDays, Lock, ShoppingBag, Trash2 } from 'lucide-react'
import { toast } from 'sonner'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { Field, Input, Textarea } from '@/components/ui/Field'
import { EmptyState, SoonBadge } from '@/components/ui/Feedback'
import { SmartImage } from '@/components/ui/SmartImage'
import { useAuth } from '@/features/auth/useAuth'
import { useCart } from '@/features/cart/useCart'
import { toastError } from '@/lib/forms'
import { formatDate, formatMoney } from '@/lib/format'
import { bookingService } from '@/services/booking.service'
import type { CheckoutInput } from '@/types/booking'

const schema = z
  .object({
    firstName: z.string().trim().min(2, 'First name is required'),
    lastName: z.string().trim().min(2, 'Last name is required'),
    email: z.email('Enter a valid e-mail'),
    phoneNumber: z.string().regex(/^\+[1-9][0-9]{7,14}$/, 'Use the international format, e.g. +970591234567'),
    specialRequests: z.string().max(500, 'Keep it under 500 characters').optional(),
  })

export function CheckoutPage() {
  const cart = useCart()
  const { user } = useAuth()
  const navigate = useNavigate()
  const [isClearing, setIsClearing] = useState(false)
  const [first = '', ...rest] = (user?.name ?? '').split(' ')

  const { register, handleSubmit, formState: { errors } } = useForm<CheckoutInput>({
    resolver: zodResolver(schema),
    defaultValues: {
      firstName: first,
      lastName: rest.join(' '),
      email: user?.email ?? '',
      phoneNumber: user?.phoneNumber ?? '',
    },
  })
  const booking = useMutation({
    mutationFn: (input: CheckoutInput) => bookingService.create(input),
    onSuccess: (result) => {
      cart.clear()
      toast.success('Booking confirmed!')
      navigate(`/booking/${result.confirmationNumber}`)
    },
    onError: toastError,
  })

  const clearCart = async () => {
    if (!window.confirm('Remove all rooms from your cart?')) return
    setIsClearing(true)
    try {
      await cart.clear()
      toast.success('Cart cleared')
    } catch (error) {
      toastError(error)
    } finally {
      setIsClearing(false)
    }
  }

  if (cart.count === 0)
    return (
      <div className="container-page pt-28">
        <EmptyState
          icon={<ShoppingBag className="size-7" />}
          title="Your cart is empty"
          description="Find a hotel you love and add a room to start your booking."
          action={<Link to="/search"><Button>Explore hotels</Button></Link>}
        />
      </div>
    )

  return (
    <div className="container-page pt-24 pb-8">
      <div className="mb-8 flex flex-wrap items-center justify-between gap-3">
        <div>
          <h1 className="text-3xl font-extrabold tracking-tight text-slate-900">Secure checkout</h1>
          <p className="mt-1 flex items-center gap-1.5 text-sm text-slate-500"><Lock className="size-4 text-emerald-600" /> Your details are encrypted and never shared.</p>
        </div>
        <SoonBadge />
      </div>

      <form onSubmit={handleSubmit((values) => booking.mutate(values))} className="grid gap-8 lg:grid-cols-[1fr_24rem]">
        <div className="space-y-6">
          <Card className="p-6 sm:p-8">
            <Step number={1} title="Guest details" />
            <div className="mt-6 grid gap-4 sm:grid-cols-2">
              <Field label="First name" required error={errors.firstName?.message}><Input autoComplete="given-name" {...register('firstName')} invalid={!!errors.firstName} /></Field>
              <Field label="Last name" required error={errors.lastName?.message}><Input autoComplete="family-name" {...register('lastName')} invalid={!!errors.lastName} /></Field>
              <Field label="E-mail" required error={errors.email?.message} hint="We’ll send the invoice here"><Input type="email" autoComplete="email" {...register('email')} invalid={!!errors.email} /></Field>
              <Field label="Phone number" required error={errors.phoneNumber?.message}><Input type="tel" autoComplete="tel" placeholder="+970591234567" {...register('phoneNumber')} invalid={!!errors.phoneNumber} /></Field>
            </div>
          </Card>

          <Card className="p-6 sm:p-8">
            <Step number={2} title="Special requests" />
            <Field className="mt-6" error={errors.specialRequests?.message} hint="Late check-in, extra bed, dietary needs… we’ll pass them to the hotel.">
              <Textarea rows={4} placeholder="Anything we should know?" {...register('specialRequests')} />
            </Field>
          </Card>
        </div>

        <aside className="lg:sticky lg:top-24 lg:self-start">
          <Card className="overflow-hidden">
            <div className="border-b border-slate-100 p-5">
              <h2 className="font-bold text-slate-900">Your booking</h2>
            </div>
            <ul className="divide-y divide-slate-100">
              {cart.items.map((item) => {
                const nights = item.nights
                return (
                  <li key={item.roomId} className="flex gap-3 p-5">
                    <SmartImage src={item.imageUrl} alt={item.hotelName} seed={item.hotelId} className="size-16 shrink-0 rounded-xl" />
                    <div className="min-w-0 flex-1">
                      <p className="truncate text-sm font-bold text-slate-900">{item.hotelName}</p>
                      <p className="text-xs text-slate-500">{item.roomType} · Room {item.roomNumber}</p>
                      <p className="mt-1 flex items-center gap-1 text-xs text-slate-500"><CalendarDays className="size-3.5" /> {formatDate(item.checkIn)} – {formatDate(item.checkOut)}</p>
                      <p className="mt-1 text-sm font-semibold text-slate-900">{formatMoney(item.totalPrice, true)} <span className="text-xs font-normal text-slate-500">· {nights} night{nights > 1 ? 's' : ''}</span></p>
                    </div>
                    <button type="button" onClick={() => void cart.remove(item.roomId)} className="self-start rounded-lg p-1.5 text-slate-400 hover:bg-red-50 hover:text-red-600" aria-label="Remove">
                      <Trash2 className="size-4" />
                    </button>
                  </li>
                )
              })}
            </ul>
            <div className="space-y-2 border-t border-slate-100 bg-slate-50/70 p-5 text-sm">
              <Row label="Subtotal" value={formatMoney(cart.subtotal, true)} />
              <div className="flex items-center justify-between border-t border-slate-200 pt-3 text-base font-extrabold text-slate-900">
                <span>Total</span>
                <span>{formatMoney(cart.subtotal, true)}</span>
              </div>
              <Button type="button" variant="ghost" size="sm" className="mt-2 w-full text-red-600 hover:bg-red-50 hover:text-red-700" loading={isClearing} onClick={() => void clearCart()}>
                <Trash2 className="size-4" /> Clear cart
              </Button>
              <Button type="submit" variant="accent" size="lg" className="mt-4 w-full" loading={booking.isPending}>
                <Lock className="size-4" /> Create booking
              </Button>
              <p className="pt-1 text-center text-xs text-slate-400">Free cancellation up to 48 hours before check-in.</p>
            </div>
          </Card>
        </aside>
      </form>
    </div>
  )
}

function Step({ number, title }: { number: number; title: string }) {
  return (
    <h2 className="flex items-center gap-3 text-lg font-bold text-slate-900">
      <span className="flex size-8 items-center justify-center rounded-full bg-brand-600 text-sm text-white">{number}</span>
      {title}
    </h2>
  )
}

function Row({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex items-center justify-between text-slate-600">
      <span>{label}</span>
      <span className="font-semibold text-slate-900">{value}</span>
    </div>
  )
}
