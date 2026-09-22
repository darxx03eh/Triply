import { Link } from 'react-router'
import { useQuery } from '@tanstack/react-query'
import { ArrowRight, BedDouble, Building2, MapPin, Sparkles } from 'lucide-react'
import { amenitiesApi } from '@/api/amenities.api'
import { citiesApi } from '@/api/cities.api'
import { hotelsApi } from '@/api/hotels.api'
import { roomsApi } from '@/api/rooms.api'
import { Card } from '@/components/ui/Card'
import { Skeleton } from '@/components/ui/Feedback'
import { SmartImage } from '@/components/ui/SmartImage'
import { Stars } from '@/components/ui/Stars'
import { useAuth } from '@/features/auth/useAuth'
import { cn } from '@/lib/cn'
import { HOTEL_TYPES } from '@/types/hotel'

export function AdminDashboardPage() {
  const { user } = useAuth()
  const counts = useQuery({
    queryKey: ['admin', 'dashboard', 'counts'],
    queryFn: async () => {
      const [cities, hotels, rooms, available, amenities, ...types] = await Promise.all([
        citiesApi.list({ pageSize: 1 }),
        hotelsApi.list({ pageSize: 1 }),
        roomsApi.list({ pageSize: 1 }),
        roomsApi.list({ filters: ['available==true'], pageSize: 1 }),
        amenitiesApi.list(),
        ...HOTEL_TYPES.map((type) => hotelsApi.list({ filters: [`type==${type}`], pageSize: 1 })),
      ])
      return {
        cities: cities.totalCount,
        hotels: hotels.totalCount,
        rooms: rooms.totalCount,
        availableRooms: available.totalCount,
        amenities: amenities.length,
        byType: HOTEL_TYPES.map((type, i) => ({ type, count: types[i].totalCount })),
      }
    },
  })
  const latest = useQuery({ queryKey: ['admin', 'dashboard', 'latest'], queryFn: () => hotelsApi.list({ sorts: '-CreatedAt', pageSize: 5 }) })
  const topCities = useQuery({ queryKey: ['admin', 'dashboard', 'cities'], queryFn: () => citiesApi.list({ pageSize: 50 }) })

  const stats = [
    { label: 'Cities', value: counts.data?.cities, icon: MapPin, to: '/admin/cities', tone: 'from-sky-500 to-brand-600' },
    { label: 'Hotels', value: counts.data?.hotels, icon: Building2, to: '/admin/hotels', tone: 'from-brand-500 to-violet-600' },
    { label: 'Rooms', value: counts.data?.rooms, icon: BedDouble, to: '/admin/rooms', tone: 'from-violet-500 to-accent-500' },
    { label: 'Amenities', value: counts.data?.amenities, icon: Sparkles, to: '/admin/amenities', tone: 'from-amber-400 to-accent-500' },
  ]
  const maxType = Math.max(1, ...(counts.data?.byType.map((t) => t.count) ?? [1]))
  const cities = [...(topCities.data?.items ?? [])].filter((c) => !c.isDeleted).sort((a, b) => b.hotelsCount - a.hotelsCount).slice(0, 6)
  const maxCity = Math.max(1, ...cities.map((c) => c.hotelsCount))

  return (
    <div className="space-y-8">
      <div className="relative overflow-hidden rounded-3xl bg-hero p-8 text-white">
        <div className="absolute -right-10 -bottom-20 size-64 rounded-full bg-accent-500/30 blur-3xl" />
        <p className="text-sm text-white/70">Welcome back</p>
        <h2 className="mt-1 text-3xl font-extrabold tracking-tight">{user?.name}</h2>
        <p className="mt-2 max-w-lg text-white/70">Here’s what’s happening on Triply. Manage destinations, hotels and rooms from the menu.</p>
      </div>

      <div className="grid gap-5 sm:grid-cols-2 xl:grid-cols-4">
        {stats.map(({ label, value, icon: Icon, to, tone }) => (
          <Link key={label} to={to}>
            <Card className="group p-5 transition hover:-translate-y-0.5 hover:shadow-lift">
              <div className="flex items-center justify-between">
                <span className={cn('flex size-11 items-center justify-center rounded-2xl bg-gradient-to-br text-white shadow-lg', tone)}>
                  <Icon className="size-5" />
                </span>
                <ArrowRight className="size-4 text-slate-300 transition group-hover:translate-x-0.5 group-hover:text-brand-600" />
              </div>
              <p className="mt-4 text-sm font-medium text-slate-500">{label}</p>
              {value === undefined ? <Skeleton className="mt-1 h-8 w-16" /> : <p className="text-3xl font-extrabold text-slate-900">{value}</p>}
            </Card>
          </Link>
        ))}
      </div>

      <div className="grid gap-6 xl:grid-cols-3">
        <Card className="p-6">
          <h3 className="font-bold text-slate-900">Hotels by type</h3>
          <div className="mt-6 space-y-5">
            {counts.data?.byType.map(({ type, count }) => (
              <div key={type}>
                <div className="mb-1.5 flex justify-between text-sm"><span className="font-medium text-slate-700">{type}</span><span className="font-bold text-slate-900">{count}</span></div>
                <div className="h-2.5 overflow-hidden rounded-full bg-slate-100">
                  <div className="h-full rounded-full bg-gradient-to-r from-brand-500 to-accent-500" style={{ width: `${(count / maxType) * 100}%` }} />
                </div>
              </div>
            )) ?? <Skeleton className="h-32" />}
            {counts.data && (
              <p className="border-t border-slate-100 pt-4 text-sm text-slate-500">
                <span className="font-bold text-slate-900">{counts.data.availableRooms}</span> of {counts.data.rooms} rooms currently available
              </p>
            )}
          </div>
        </Card>

        <Card className="p-6">
          <h3 className="font-bold text-slate-900">Top cities by hotels</h3>
          <div className="mt-6 space-y-4">
            {cities.map((city) => (
              <div key={city.cityId} className="flex items-center gap-3">
                <span className="w-24 truncate text-sm font-medium text-slate-700">{city.name}</span>
                <div className="h-2.5 flex-1 overflow-hidden rounded-full bg-slate-100">
                  <div className="h-full rounded-full bg-brand-500" style={{ width: `${(city.hotelsCount / maxCity) * 100}%` }} />
                </div>
                <span className="w-6 text-right text-sm font-bold text-slate-900">{city.hotelsCount}</span>
              </div>
            ))}
          </div>
        </Card>

        <Card className="p-6">
          <div className="flex items-center justify-between">
            <h3 className="font-bold text-slate-900">Latest hotels</h3>
            <Link to="/admin/hotels" className="text-sm font-semibold text-brand-600">View all</Link>
          </div>
          <ul className="mt-5 space-y-4">
            {latest.data?.items.map((hotel) => (
              <li key={hotel.hotelId} className="flex items-center gap-3">
                <SmartImage src={hotel.thumbnailUrl} alt={hotel.name} seed={hotel.hotelId} className="size-11 shrink-0 rounded-xl" />
                <div className="min-w-0">
                  <p className="truncate text-sm font-semibold text-slate-900">{hotel.name}</p>
                  <div className="flex items-center gap-2 text-xs text-slate-500"><Stars value={hotel.starRating} /> {hotel.cityName}</div>
                </div>
              </li>
            ))}
          </ul>
        </Card>
      </div>
    </div>
  )
}
