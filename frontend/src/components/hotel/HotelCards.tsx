import { Link } from 'react-router'
import { ArrowUpRight, BedDouble, Clock, MapPin, TrendingUp } from 'lucide-react'
import { cn } from '@/lib/cn'
import { formatDate, formatMoney } from '@/lib/format'
import { stockImages } from '@/mocks/images'
import type { TrendingCity } from '@/types/city'
import type { FeaturedDeal, RecentHotel } from '@/types/hotel'
import type { HotelSearchItem } from '@/types/search'
import { Badge } from '../ui/Feedback'
import { SmartImage } from '../ui/SmartImage'
import { Stars } from '../ui/Stars'

const typeTone = { Luxury: 'accent', Boutique: 'brand', Budget: 'green' } as const

export function HotelResultCard({ hotel, query, nights, rooms }: {
  hotel: HotelSearchItem
  query: string
  nights: number
  rooms: number
}) {
  return (
    <Link
      to={`/hotels/${hotel.hotelId}${query}`}
      className="group flex flex-col overflow-hidden rounded-3xl border border-slate-200/70 bg-white shadow-soft transition duration-300 hover:-translate-y-0.5 hover:shadow-lift sm:flex-row"
    >
      <div className="relative h-52 shrink-0 overflow-hidden sm:h-auto sm:w-72">
        <SmartImage
          src={hotel.thumbnailUrl}
          alt={hotel.name}
          seed={hotel.hotelId}
          label={hotel.name}
          className="size-full transition duration-500 group-hover:scale-105"
        />
        <Badge tone={typeTone[hotel.hotelType]} className="absolute top-3 left-3 bg-white/95 backdrop-blur">
          {hotel.hotelType}
        </Badge>
      </div>
      <div className="flex flex-1 flex-col p-5 sm:p-6">
        <div className="flex items-start justify-between gap-3">
          <div className="min-w-0">
            <Stars value={hotel.starRating} />
            <h3 className="mt-1.5 truncate text-lg font-bold text-slate-900 transition group-hover:text-brand-700">{hotel.name}</h3>
            <p className="mt-1 flex items-center gap-1.5 text-sm text-slate-500">
              <MapPin className="size-4 shrink-0" />
              <span className="truncate">{hotel.address ?? `${hotel.cityName}, ${hotel.country}`}</span>
            </p>
          </div>
          <ArrowUpRight className="size-5 shrink-0 text-slate-300 transition group-hover:text-brand-600" />
        </div>
        <p className="mt-3 line-clamp-2 text-sm leading-relaxed text-slate-600">
          {hotel.description ??
            `${hotel.hotelType} stay in ${hotel.cityName}, rated ${hotel.starRating} stars by our guests.`}
        </p>
        {hotel.amenities.length > 0 && (
          <div className="mt-3 flex flex-wrap gap-1.5">
            {hotel.amenities.slice(0, 4).map((amenity) => (
              <span key={amenity} className="rounded-full bg-slate-100 px-2.5 py-0.5 text-xs font-medium text-slate-600">
                {amenity}
              </span>
            ))}
            {hotel.amenities.length > 4 && (
              <span className="rounded-full px-1.5 py-0.5 text-xs font-medium text-slate-400">+{hotel.amenities.length - 4}</span>
            )}
          </div>
        )}
        <div className="mt-auto flex items-end justify-between gap-4 pt-5">
          <span
            className={cn(
              'inline-flex items-center gap-1.5 text-xs font-semibold',
              hotel.availableRooms <= 2 ? 'text-accent-600' : 'text-emerald-600',
            )}
          >
            <BedDouble className="size-4" />
            {hotel.availableRooms <= 2 ? `Only ${hotel.availableRooms} left` : `${hotel.availableRooms} rooms available`}
          </span>
          <div className="text-right">
            <p className="text-xs text-slate-500">per night from</p>
            <p className="text-2xl font-extrabold text-slate-900">{formatMoney(hotel.minPricePerNight)}</p>
            <p className="text-xs text-slate-500">
              {formatMoney(hotel.totalPrice)} for {nights} night{nights > 1 ? 's' : ''}
              {rooms > 1 && ` · ${rooms} rooms`}
            </p>
          </div>
        </div>
      </div>
    </Link>
  )
}

export function DealCard({ deal }: { deal: FeaturedDeal }) {
  return (
    <Link
      to={`/hotels/${deal.hotelId}`}
      className="group relative block overflow-hidden rounded-3xl bg-white shadow-soft ring-1 ring-slate-200/70 transition duration-300 hover:-translate-y-1 hover:shadow-lift"
    >
      <div className="relative h-56 overflow-hidden">
        <SmartImage
          src={deal.thumbnailUrl}
          alt={deal.hotelName}
          seed={deal.hotelId}
          label={deal.hotelName}
          className="size-full transition duration-700 group-hover:scale-110"
        />
        <div className="absolute inset-0 bg-gradient-to-t from-slate-950/70 via-transparent to-transparent" />
        <span className="absolute top-4 left-4 rounded-full bg-accent-500 px-3 py-1 text-xs font-bold text-white shadow-lg">
          -{Math.round(deal.discountPercentage)}%
        </span>
        <span className="absolute top-4 right-4 rounded-full bg-white/95 px-2.5 py-1 text-[11px] font-semibold text-slate-700 shadow backdrop-blur">
          {deal.title}
        </span>
        <div className="absolute right-4 bottom-4 left-4 text-white">
          <Stars value={deal.starRating} />
          <h3 className="mt-1 truncate text-lg font-bold">{deal.hotelName}</h3>
        </div>
      </div>
      <div className="space-y-2 p-4">
        <p className="flex items-center gap-1.5 text-sm text-slate-500">
          <MapPin className="size-4 shrink-0" />
          <span className="truncate">{deal.cityName}, {deal.country}</span>
        </p>
        <div className="flex items-baseline justify-between gap-2">
          <span className="text-xs text-slate-400">{deal.roomType} · per night</span>
          <span>
            <span className="mr-1.5 text-sm text-slate-400 line-through">{formatMoney(deal.originalPrice)}</span>
            <span className="text-lg font-extrabold text-slate-900">{formatMoney(deal.discountedPrice)}</span>
          </span>
        </div>
        <p className="flex items-center gap-1.5 text-xs font-medium text-accent-600">
          <Clock className="size-3.5" /> Ends {formatDate(deal.endsAt)}
        </p>
      </div>
    </Link>
  )
}

export function RecentHotelCard({ hotel }: { hotel: RecentHotel }) {
  return (
    <Link
      to={`/hotels/${hotel.hotelId}`}
      className="group flex items-center gap-4 rounded-2xl bg-white p-3 shadow-soft ring-1 ring-slate-200/70 transition hover:shadow-lift"
    >
      <SmartImage src={hotel.thumbnailUrl} alt={hotel.name} seed={hotel.hotelId} className="size-20 shrink-0 rounded-xl" />
      <div className="min-w-0">
        <Stars value={hotel.starRating} />
        <p className="mt-1 truncate font-bold text-slate-900 group-hover:text-brand-700">{hotel.name}</p>
        <p className="truncate text-sm text-slate-500">{hotel.cityName}, {hotel.country}</p>
        {hotel.minPricePerNight !== null && (
          <p className="mt-0.5 text-sm">
            <span className="font-bold text-slate-900">{formatMoney(hotel.minPricePerNight)}</span>
            <span className="text-slate-500"> / night</span>
          </p>
        )}
      </div>
    </Link>
  )
}

export function DestinationCard({ city, index, className }: { city: TrendingCity; index: number; className?: string }) {
  return (
    <Link
      to={`/search?cityId=${city.cityId}`}
      className={cn('group relative block overflow-hidden rounded-3xl shadow-soft', className)}
    >
      <SmartImage src={city.thumbnailUrl ?? stockImages.cities[city.name]} alt={city.name} seed={city.name} label={city.name} className="size-full transition duration-700 group-hover:scale-110" />
      <div className="absolute inset-0 bg-gradient-to-t from-slate-950/80 via-slate-950/10 to-transparent" />
      <span className="absolute top-4 left-4 flex size-9 items-center justify-center rounded-full bg-white/95 text-sm font-extrabold text-slate-900">
        {index + 1}
      </span>
      <div className="absolute right-5 bottom-5 left-5 text-white">
        <h3 className="text-2xl font-extrabold">{city.name}</h3>
        <p className="mt-0.5 flex items-center gap-1.5 text-sm text-white/80">
          <TrendingUp className="size-4" /> {city.visitsCount.toLocaleString()} visit{city.visitsCount === 1 ? '' : 's'} · {city.hotelsCount} hotel{city.hotelsCount === 1 ? '' : 's'} · {city.country}
        </p>
      </div>
    </Link>
  )
}
