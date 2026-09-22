import { useEffect, useRef, useState, type FormEvent } from 'react'
import { useNavigate } from 'react-router'
import { useQuery } from '@tanstack/react-query'
import { CalendarDays, MapPin, Minus, Plus, Search, Users } from 'lucide-react'
import { citiesApi } from '@/api/cities.api'
import { useDebounce } from '@/hooks/useDebounce'
import { defaultDates, toSearchParams } from '@/hooks/useSearchState'
import { cn } from '@/lib/cn'
import { addDays, toIsoDate } from '@/lib/format'
import { sieve } from '@/lib/sieve'
import type { SearchParamsState } from '@/types/search'
import { Button } from '../ui/Button'

type Initial = Partial<Pick<SearchParamsState, 'q' | 'cityId' | 'checkIn' | 'checkOut' | 'adults' | 'children' | 'rooms'>> & {
  cityName?: string
}

export function SearchBar({ initial, variant = 'hero', onSubmit }: {
  initial?: Initial
  variant?: 'hero' | 'compact'
  onSubmit?: (state: Initial) => void
}) {
  const navigate = useNavigate()
  const dates = defaultDates()
  const [text, setText] = useState(initial?.cityName ?? initial?.q ?? '')
  const [cityId, setCityId] = useState(initial?.cityId ?? '')
  const [checkIn, setCheckIn] = useState(initial?.checkIn ?? dates.checkIn)
  const [checkOut, setCheckOut] = useState(initial?.checkOut ?? dates.checkOut)
  const [adults, setAdults] = useState(initial?.adults ?? 2)
  const [children, setChildren] = useState(initial?.children ?? 0)
  const [rooms, setRooms] = useState(initial?.rooms ?? 1)
  const [suggestOpen, setSuggestOpen] = useState(false)
  const [guestsOpen, setGuestsOpen] = useState(false)
  const boxRef = useRef<HTMLFormElement>(null)
  const debounced = useDebounce(text, 250)

  useEffect(() => {
    if (initial?.cityName !== undefined || initial?.q !== undefined) setText(initial?.cityName || initial?.q || '')
  }, [initial?.cityName, initial?.q])

  useEffect(() => {
    const close = (e: MouseEvent) => {
      if (boxRef.current && !boxRef.current.contains(e.target as Node)) {
        setSuggestOpen(false)
        setGuestsOpen(false)
      }
    }
    document.addEventListener('mousedown', close)
    return () => document.removeEventListener('mousedown', close)
  }, [])

  const cities = useQuery({
    queryKey: ['cities', 'suggest', debounced],
    queryFn: () => citiesApi.list({ filters: [sieve.contains('Name', debounced)], sorts: 'Name', pageSize: 6 }),
    enabled: suggestOpen,
    staleTime: 60_000,
  })

  const submit = (event: FormEvent) => {
    event.preventDefault()
    const state: Initial = { checkIn, checkOut, adults, children, rooms, ...(cityId ? { cityId } : { q: text.trim() }) }
    if (onSubmit) onSubmit({ ...state, cityName: cityId ? text : undefined })
    else navigate(`/search?${toSearchParams(state as Partial<SearchParamsState>)}`)
    setSuggestOpen(false)
    setGuestsOpen(false)
  }

  const hero = variant === 'hero'
  const segment = cn('relative flex min-w-0 flex-1 items-center gap-3 px-4', hero ? 'py-3' : 'py-2')

  return (
    <form
      ref={boxRef}
      onSubmit={submit}
      className={cn(
        'relative flex w-full flex-col divide-y divide-slate-100 rounded-2xl bg-white text-left shadow-lift ring-1 ring-slate-900/5 lg:flex-row lg:items-center lg:divide-x lg:divide-y-0',
        hero ? 'p-2' : 'p-1.5',
      )}
    >
      <div className={cn(segment, 'lg:flex-[1.6]')}>
        <MapPin className="size-5 shrink-0 text-brand-500" />
        <div className="min-w-0 flex-1">
          <p className="text-[11px] font-bold tracking-wide text-slate-400 uppercase">Destination</p>
          <input
            value={text}
            onChange={(e) => {
              setText(e.target.value)
              setCityId('')
              setSuggestOpen(true)
            }}
            onFocus={() => setSuggestOpen(true)}
            placeholder="Search for hotels, cities..."
            className="w-full truncate bg-transparent text-sm font-semibold text-slate-900 outline-none placeholder:font-medium placeholder:text-slate-400"
          />
        </div>
        {suggestOpen && (cities.data?.items.length ?? 0) > 0 && (
          <div className="absolute top-full left-0 z-30 mt-3 w-full min-w-72 animate-scale-in overflow-hidden rounded-2xl border border-slate-100 bg-white p-1.5 shadow-lift">
            <p className="px-3 pt-2 pb-1 text-[11px] font-bold tracking-wide text-slate-400 uppercase">Cities</p>
            {cities.data!.items.map((city) => (
              <button
                key={city.cityId}
                type="button"
                onClick={() => {
                  setText(city.name)
                  setCityId(city.cityId)
                  setSuggestOpen(false)
                }}
                className="flex w-full cursor-pointer items-center gap-3 rounded-xl px-3 py-2.5 text-left hover:bg-slate-50"
              >
                <span className="flex size-9 items-center justify-center rounded-xl bg-brand-50 text-brand-600">
                  <MapPin className="size-4" />
                </span>
                <span>
                  <span className="block text-sm font-semibold text-slate-900">{city.name}</span>
                  <span className="block text-xs text-slate-500">
                    {city.country} · {city.hotelsCount} {city.hotelsCount === 1 ? 'hotel' : 'hotels'}
                  </span>
                </span>
              </button>
            ))}
          </div>
        )}
      </div>

      <div className={cn(segment, 'lg:flex-[1.3]')}>
        <CalendarDays className="size-5 shrink-0 text-brand-500" />
        <div className="grid flex-1 grid-cols-2 gap-3">
          <div>
            <p className="text-[11px] font-bold tracking-wide text-slate-400 uppercase">Check-in</p>
            <input
              type="date"
              value={checkIn}
              min={toIsoDate(new Date())}
              onChange={(e) => {
                setCheckIn(e.target.value)
                if (e.target.value >= checkOut) setCheckOut(toIsoDate(addDays(new Date(e.target.value), 1)))
              }}
              className="w-full min-w-0 bg-transparent text-[13px] font-semibold text-slate-900 outline-none"
            />
          </div>
          <div>
            <p className="text-[11px] font-bold tracking-wide text-slate-400 uppercase">Check-out</p>
            <input
              type="date"
              value={checkOut}
              min={toIsoDate(addDays(new Date(checkIn), 1))}
              onChange={(e) => setCheckOut(e.target.value)}
              className="w-full min-w-0 bg-transparent text-[13px] font-semibold text-slate-900 outline-none"
            />
          </div>
        </div>
      </div>

      <div className={segment}>
        <Users className="size-5 shrink-0 text-brand-500" />
        <button type="button" onClick={() => setGuestsOpen((v) => !v)} className="min-w-0 flex-1 cursor-pointer text-left">
          <p className="text-[11px] font-bold tracking-wide text-slate-400 uppercase">Guests & rooms</p>
          <p className="truncate text-sm font-semibold text-slate-900">
            {adults} {adults === 1 ? 'adult' : 'adults'} · {children} {children === 1 ? 'child' : 'children'} · {rooms}{' '}
            {rooms === 1 ? 'room' : 'rooms'}
          </p>
        </button>
        {guestsOpen && (
          <div className="absolute top-full right-0 z-30 mt-3 w-72 animate-scale-in space-y-4 rounded-2xl border border-slate-100 bg-white p-5 shadow-lift">
            <Stepper
              label="Adults"
              hint="Ages 13+"
              value={adults}
              min={1}
              max={20}
              onChange={(value) => {
                setAdults(value)
                setRooms((current) => Math.min(current, value))
              }}
            />
            <Stepper label="Children" hint="Ages 0–12" value={children} min={0} max={20} onChange={setChildren} />
            <Stepper label="Rooms" hint="At least one adult per room" value={rooms} min={1} max={Math.min(10, adults)} onChange={setRooms} />
            <Button type="button" variant="secondary" className="w-full" onClick={() => setGuestsOpen(false)}>
              Done
            </Button>
          </div>
        )}
      </div>

      <div className="p-1.5 lg:pl-2">
        <Button type="submit" variant="accent" size={hero ? 'lg' : 'md'} className="w-full lg:w-auto">
          <Search className="size-5" />
          Search
        </Button>
      </div>
    </form>
  )
}

function Stepper({ label, hint, value, min, max = 10, onChange }: {
  label: string
  hint?: string
  value: number
  min: number
  max?: number
  onChange: (value: number) => void
}) {
  return (
    <div className="flex items-center justify-between">
      <div>
        <p className="text-sm font-semibold text-slate-900">{label}</p>
        {hint && <p className="text-xs text-slate-500">{hint}</p>}
      </div>
      <div className="flex items-center gap-3">
        <button
          type="button"
          disabled={value <= min}
          onClick={() => onChange(value - 1)}
          className="flex size-8 cursor-pointer items-center justify-center rounded-full border border-slate-200 text-slate-600 transition hover:border-brand-500 hover:text-brand-600 disabled:cursor-not-allowed disabled:opacity-40"
        >
          <Minus className="size-4" />
        </button>
        <span className="w-5 text-center text-sm font-bold">{value}</span>
        <button
          type="button"
          disabled={value >= max}
          onClick={() => onChange(value + 1)}
          className="flex size-8 cursor-pointer items-center justify-center rounded-full border border-slate-200 text-slate-600 transition hover:border-brand-500 hover:text-brand-600 disabled:cursor-not-allowed disabled:opacity-40"
        >
          <Plus className="size-4" />
        </button>
      </div>
    </div>
  )
}
