import { useEffect, useState } from 'react'
import { Link, useLocation, useNavigate, useParams, useSearchParams } from 'react-router'
import { useQuery, useQueryClient } from '@tanstack/react-query'
import {
  ArrowLeft, BedDouble, Check, ChevronLeft, ChevronRight, Expand, Landmark, MapPin, Navigation, ShoppingBag, Sparkles, Users, X,
} from 'lucide-react'
import { toast } from 'sonner'
import { HotelReviews } from '@/components/hotel/HotelReviews'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { Badge, EmptyState, ErrorState, PageLoader, Skeleton } from '@/components/ui/Feedback'
import { SmartImage } from '@/components/ui/SmartImage'
import { RatingPill, Stars } from '@/components/ui/Stars'
import { useCart } from '@/features/cart/useCart'
import { useAuth } from '@/features/auth/useAuth'
import { parseSearch } from '@/hooks/useSearchState'
import { cn } from '@/lib/cn'
import { formatDate, formatMoney, nightsBetween } from '@/lib/format'
import { stockImages } from '@/mocks/images'
import { hotelService } from '@/services/hotel.service'
import type { HotelDetails } from '@/types/hotel'
import type { Room } from '@/types/room'

export function HotelPage() {
  const { hotelId = '' } = useParams()
  const [params] = useSearchParams()
  const search = parseSearch(params)
  const nights = nightsBetween(search.checkIn, search.checkOut)

  const hotel = useQuery({ queryKey: ['hotel', hotelId], queryFn: () => hotelService.getDetails(hotelId) })
  const rooms = useQuery({
    queryKey: ['hotel', hotelId, 'rooms', search.checkIn, search.checkOut],
    queryFn: () => hotelService.getRooms(hotelId, search.checkIn, search.checkOut),
  })
  const nearby = useQuery({ queryKey: ['hotel', hotelId, 'nearby'], queryFn: () => hotelService.getNearbyAttractions(hotelId) })

  const priceFrom = rooms.data?.items.find((r) => r.isAvailable)?.pricePerNight ?? null
  const queryClient = useQueryClient()

  // The backend records the visit when a signed-in user loads the hotel, so the home page list is stale now.
  useEffect(() => {
    if (hotel.data) queryClient.invalidateQueries({ queryKey: ['home', 'recent'] })
  }, [hotel.data, queryClient])

  if (hotel.isLoading) return <div className="pt-16"><PageLoader /></div>
  if (hotel.isError) return <div className="container-page pt-28"><ErrorState message={(hotel.error as Error).message} onRetry={() => hotel.refetch()} /></div>
  if (!hotel.data)
    return (
      <div className="container-page pt-28">
        <EmptyState title="Hotel not found" description="It may have been removed." action={<Link to="/search"><Button>Browse hotels</Button></Link>} />
      </div>
    )

  const h = hotel.data

  return (
    <div className="pt-16">
      <div className="container-page py-6">
        <Link to={`/search?${params}`} className="inline-flex items-center gap-1.5 text-sm font-semibold text-slate-500 hover:text-slate-900">
          <ArrowLeft className="size-4" /> Back to results
        </Link>

        <div className="mt-4 flex flex-col justify-between gap-4 md:flex-row md:items-end">
          <div>
            <div className="flex flex-wrap items-center gap-3">
              <Stars value={h.averageRating ?? h.starRating} size="md" />
              <Badge tone="brand">{h.hotelType}</Badge>
            </div>
            <h1 className="mt-2 text-3xl font-extrabold tracking-tight text-slate-900 sm:text-4xl">{h.name}</h1>
            <p className="mt-2 flex items-center gap-1.5 text-slate-500">
              <MapPin className="size-4" /> {h.address ?? h.cityName}
            </p>
          </div>
          <div className="flex items-center gap-4">
            {h.averageRating !== null && (
              <a href="#reviews" className="flex flex-col items-end gap-0.5">
                <RatingPill value={h.averageRating} />
                <span className="text-xs text-slate-500">{h.reviewsCount} review{h.reviewsCount > 1 ? 's' : ''}</span>
              </a>
            )}
            {priceFrom !== null && (
              <div className="text-right">
                <p className="text-xs text-slate-500">from</p>
                <p className="text-2xl font-extrabold text-slate-900">
                  {formatMoney(priceFrom)}<span className="text-sm font-medium text-slate-500"> / night</span>
                </p>
              </div>
            )}
          </div>
        </div>

        <Gallery hotel={h} />
      </div>

      <div className="container-page grid gap-8 pb-8 lg:grid-cols-[1fr_22rem]">
        <div className="min-w-0 space-y-8">
          <Card className="p-6 sm:p-8">
            <h2 className="text-xl font-bold text-slate-900">About this hotel</h2>
            <p className="mt-3 leading-relaxed text-slate-600">
              {h.description ??
                `${h.name} is a ${h.starRating}-star ${h.hotelType.toLowerCase()} hotel in ${h.cityName}. Enjoy comfortable rooms, friendly service and a great location to explore the city.`}
            </p>
            {h.amenities.length > 0 && (
              <>
                <h3 className="mt-7 flex items-center gap-2 font-bold text-slate-900"><Sparkles className="size-5 text-brand-600" /> Popular amenities</h3>
                <div className="mt-4 grid grid-cols-2 gap-3 sm:grid-cols-3">
                  {h.amenities.map((name) => (
                    <span key={name} className="flex items-center gap-2 text-sm text-slate-700">
                      <span className="flex size-6 items-center justify-center rounded-full bg-emerald-50 text-emerald-600"><Check className="size-3.5" /></span>
                      {name}
                    </span>
                  ))}
                </div>
              </>
            )}
          </Card>

          <section id="rooms">
            <div className="mb-4 flex flex-wrap items-end justify-between gap-2">
              <div>
                <h2 className="text-2xl font-extrabold tracking-tight text-slate-900">Choose your room</h2>
                <p className="mt-1 text-sm text-slate-500">
                  {formatDate(search.checkIn)} → {formatDate(search.checkOut)} · {nights} night{nights > 1 ? 's' : ''} ·{' '}
                  {search.adults} adult{search.adults > 1 ? 's' : ''}{search.children > 0 && `, ${search.children} children`}
                </p>
              </div>
            </div>
            {rooms.isLoading ? (
              <div className="space-y-4">{Array.from({ length: 3 }, (_, i) => <Skeleton key={i} className="h-44 rounded-3xl" />)}</div>
            ) : rooms.data?.items.length ? (
              <div className="space-y-4">
                {rooms.data.items.map((room) => (
                  <RoomCard key={room.roomId} room={room} checkIn={search.checkIn} checkOut={search.checkOut} adults={search.adults} children={search.children} />
                ))}
              </div>
            ) : (
              <EmptyState icon={<BedDouble className="size-7" />} title="No rooms listed yet" description="This hotel has not published any rooms." />
            )}
          </section>

          <HotelReviews hotelId={h.hotelId} averageRating={h.averageRating} reviewsCount={h.reviewsCount} />
        </div>

        <aside className="space-y-6 lg:sticky lg:top-24 lg:self-start">
          <Card className="overflow-hidden">
            {h.latitude !== null && h.longitude !== null ? (
              <iframe
                title="Hotel location"
                className="h-56 w-full border-0"
                loading="lazy"
                src={`https://www.openstreetmap.org/export/embed.html?bbox=${h.longitude - 0.01},${h.latitude - 0.006},${h.longitude + 0.01},${h.latitude + 0.006}&layer=mapnik&marker=${h.latitude},${h.longitude}`}
              />
            ) : (
              <div className="flex h-56 items-center justify-center bg-slate-100 text-sm text-slate-500">Location not available</div>
            )}
            <div className="p-5">
              <p className="flex items-start gap-2 text-sm font-medium text-slate-700"><MapPin className="mt-0.5 size-4 shrink-0 text-brand-600" /> {h.address ?? h.cityName}</p>
              {h.latitude !== null && (
                <a
                  href={`https://www.google.com/maps?q=${h.latitude},${h.longitude}`}
                  target="_blank"
                  rel="noreferrer"
                  className="mt-3 inline-flex items-center gap-1.5 text-sm font-semibold text-brand-600 hover:text-brand-700"
                >
                  <Navigation className="size-4" /> Get directions
                </a>
              )}
              <h3 className="mt-5 text-sm font-bold text-slate-900">Nearby attractions</h3>
              {nearby.isLoading ? (
                <div className="mt-3 space-y-2.5">{Array.from({ length: 3 }, (_, i) => <Skeleton key={i} className="h-9" />)}</div>
              ) : nearby.data?.length ? (
                <ul className="mt-3 space-y-2.5">
                  {nearby.data.map((place) => (
                    <li key={place.attractionId} className="flex items-center justify-between gap-3 text-sm">
                      <span className="flex min-w-0 items-center gap-2.5">
                        <span className="flex size-8 shrink-0 items-center justify-center rounded-lg bg-brand-50 text-brand-600"><Landmark className="size-4" /></span>
                        <span className="min-w-0">
                          <span className="block truncate font-medium text-slate-700">{place.name}</span>
                          <span className="block text-xs text-slate-400">{place.category}</span>
                        </span>
                      </span>
                      <span className="shrink-0 text-xs font-semibold text-slate-500">{formatDistance(place.distanceKm)}</span>
                    </li>
                  ))}
                </ul>
              ) : (
                <p className="mt-2 text-sm text-slate-400">No nearby attractions listed yet.</p>
              )}
            </div>
          </Card>
          <CartSummary />
        </aside>
      </div>
    </div>
  )
}

function Gallery({ hotel }: { hotel: HotelDetails }) {
  const images = hotel.imageUrls
  const [open, setOpen] = useState<number | null>(null)

  useEffect(() => {
    if (open === null) return
    const onKey = (e: KeyboardEvent) => {
      if (e.key === 'Escape') setOpen(null)
      if (e.key === 'ArrowRight') setOpen((i) => (i === null ? i : (i + 1) % images.length))
      if (e.key === 'ArrowLeft') setOpen((i) => (i === null ? i : (i - 1 + images.length) % images.length))
    }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  }, [open, images.length])

  if (images.length === 0)
    return <SmartImage alt={hotel.name} seed={hotel.hotelId} label={hotel.name} className="mt-6 h-72 w-full rounded-3xl sm:h-96" />

  const tiles = images.slice(0, 5)
  return (
    <>
      <div className={cn('mt-6 grid h-72 gap-2 overflow-hidden rounded-3xl sm:h-[26rem]', tiles.length > 1 && 'sm:grid-cols-4 sm:grid-rows-2')}>
        {tiles.map((url, index) => (
          <button
            key={url}
            onClick={() => setOpen(index)}
            className={cn('group relative cursor-zoom-in overflow-hidden', index === 0 ? 'sm:col-span-2 sm:row-span-2' : 'hidden sm:block')}
          >
            <img src={url} alt={`${hotel.name} ${index + 1}`} className="size-full object-cover transition duration-500 group-hover:scale-105" />
            {index === tiles.length - 1 && (
              <span className="absolute right-3 bottom-3 inline-flex items-center gap-1.5 rounded-xl bg-white/95 px-3 py-1.5 text-xs font-bold text-slate-800 shadow">
                <Expand className="size-3.5" /> {images.length} photos
              </span>
            )}
          </button>
        ))}
      </div>
      {open !== null && (
        <div className="fixed inset-0 z-50 flex animate-fade-in items-center justify-center bg-slate-950/95">
          <button onClick={() => setOpen(null)} className="absolute top-5 right-5 rounded-full bg-white/10 p-2 text-white hover:bg-white/20"><X className="size-6" /></button>
          {images.length > 1 && (
            <>
              <button onClick={() => setOpen((open - 1 + images.length) % images.length)} className="absolute left-4 rounded-full bg-white/10 p-3 text-white hover:bg-white/20"><ChevronLeft className="size-6" /></button>
              <button onClick={() => setOpen((open + 1) % images.length)} className="absolute right-4 rounded-full bg-white/10 p-3 text-white hover:bg-white/20"><ChevronRight className="size-6" /></button>
            </>
          )}
          <img src={images[open]} alt="" className="max-h-[85vh] max-w-[90vw] animate-scale-in rounded-2xl object-contain" />
          <p className="absolute bottom-6 text-sm font-medium text-white/70">{open + 1} / {images.length}</p>
        </div>
      )}
    </>
  )
}

function RoomCard({ room, checkIn, checkOut, adults, children }: {
  room: Room
  checkIn: string
  checkOut: string
  adults: number
  children: number
}) {
  const cart = useCart()
  const { isAuthenticated } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const nights = nightsBetween(checkIn, checkOut)
  const inCart = cart.has(room.roomId)
  const fits = room.adultCapacity >= adults && room.childCapacity >= children

  const add = async () => {
    if (!isAuthenticated) {
      navigate('/login', { state: { from: location.pathname + location.search } })
      return
    }
    try {
      await cart.add({
      roomId: room.roomId,
      adults: Math.min(adults, room.adultCapacity),
      children: Math.min(children, room.childCapacity),
      checkIn,
      checkOut,
      })
      toast.success(`Room ${room.number} added to your cart`)
    } catch (error) {
      toast.error(error instanceof Error ? error.message : 'Could not add this room to your cart')
    }
  }

  return (
    <div className={cn('flex flex-col overflow-hidden rounded-3xl border bg-white shadow-soft sm:flex-row', inCart ? 'border-brand-300 ring-2 ring-brand-500/20' : 'border-slate-200/70')}>
      <RoomImagesGallery room={room} />
      <div className="flex flex-1 flex-col gap-4 p-5 sm:flex-row sm:items-center">
        <div className="min-w-0 flex-1">
          <div className="flex flex-wrap items-center gap-2">
            <h3 className="text-lg font-bold text-slate-900">{room.roomType} room</h3>
            <Badge>#{room.number}</Badge>
            {!room.isAvailable && <Badge tone="red">Unavailable</Badge>}
            {room.isBooked && <Badge tone="amber">Booked</Badge>}
          </div>
          <p className="mt-1.5 line-clamp-2 text-sm text-slate-600">{room.description ?? 'Comfortable room with everything you need.'}</p>
          <p className="mt-3 flex items-center gap-1.5 text-sm text-slate-500">
            <Users className="size-4" /> Up to {room.adultCapacity} adult{room.adultCapacity > 1 ? 's' : ''}
            {room.childCapacity > 0 && ` · ${room.childCapacity} child${room.childCapacity > 1 ? 'ren' : ''}`}
          </p>
          {!fits && room.isAvailable && !room.isBooked && <p className="mt-1 text-xs font-medium text-amber-600">Smaller than your party — you may need another room.</p>}
        </div>
        <div className="flex items-center justify-between gap-4 sm:flex-col sm:items-end">
          <div className="sm:text-right">
            <p className="text-2xl font-extrabold text-slate-900">{formatMoney(room.pricePerNight)}</p>
            <p className="text-xs text-slate-500">per night · {formatMoney(room.pricePerNight * nights)} total</p>
          </div>
          {inCart ? (
            <Button variant="outline" onClick={() => void cart.remove(room.roomId)}><Check className="size-4 text-emerald-600" /> In cart</Button>
          ) : room.isBooked ? (
            <Button disabled variant="outline">Booked</Button>
          ) : (
            <Button onClick={add} disabled={!room.isAvailable}><ShoppingBag className="size-4" /> Add to cart</Button>
          )}
        </div>
      </div>
    </div>
  )
}

function RoomImagesGallery({ room }: { room: Room }) {
  const images = room.imageUrls
  const [active, setActive] = useState(0)
  const fallback = stockImages.rooms[room.roomType === 'Suite' ? 1 : 0]

  if (images.length === 0)
    return <SmartImage src={fallback} alt={room.roomType} seed={room.roomId} className="h-44 shrink-0 sm:h-auto sm:w-56" />

  const previous = () => setActive((index) => (index - 1 + images.length) % images.length)
  const next = () => setActive((index) => (index + 1) % images.length)

  return (
    <div className="relative h-44 shrink-0 overflow-hidden bg-slate-100 sm:h-auto sm:w-56">
      <img src={images[active]} alt={`${room.roomType} room ${active + 1}`} className="size-full object-cover" />
      {images.length > 1 && (
        <>
          <button type="button" onClick={previous} aria-label="Previous room image" className="absolute top-1/2 left-2 -translate-y-1/2 rounded-full bg-slate-950/60 p-1.5 text-white transition hover:bg-slate-950/80">
            <ChevronLeft className="size-4" />
          </button>
          <button type="button" onClick={next} aria-label="Next room image" className="absolute top-1/2 right-2 -translate-y-1/2 rounded-full bg-slate-950/60 p-1.5 text-white transition hover:bg-slate-950/80">
            <ChevronRight className="size-4" />
          </button>
          <span className="absolute right-2 bottom-2 rounded-lg bg-slate-950/70 px-2 py-1 text-xs font-semibold text-white">{active + 1} / {images.length}</span>
        </>
      )}
    </div>
  )
}

function CartSummary() {
  const cart = useCart()
  if (cart.count === 0) return null
  return (
    <Card className="p-5">
      <p className="text-sm font-bold text-slate-900">Your cart</p>
      <p className="mt-1 text-sm text-slate-500">{cart.count} room{cart.count > 1 ? 's' : ''} · {formatMoney(cart.subtotal)}</p>
      <Link to="/checkout"><Button variant="accent" className="mt-4 w-full">Continue to checkout</Button></Link>
    </Card>
  )
}

const formatDistance = (km: number) => (km < 1 ? `${Math.round(km * 1000)} m` : `${km.toFixed(1)} km`)
