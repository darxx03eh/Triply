import { useCallback, useMemo } from 'react'
import { useSearchParams } from 'react-router'
import { addDays, toIsoDate } from '@/lib/format'
import { HOTEL_TYPES, type HotelType } from '@/types/hotel'
import type { HotelSort, SearchParamsState } from '@/types/search'

const list = (value: string | null) => (value ? value.split(',').filter(Boolean) : [])
const num = (value: string | null, fallback: number) => {
  const parsed = Number(value)
  return value !== null && Number.isFinite(parsed) ? parsed : fallback
}

export const defaultDates = () => {
  const today = new Date()
  return { checkIn: toIsoDate(today), checkOut: toIsoDate(addDays(today, 1)) }
}

export function parseSearch(params: URLSearchParams): SearchParamsState {
  const dates = defaultDates()
  return {
    q: params.get('q') ?? '',
    cityId: params.get('cityId') ?? '',
    checkIn: params.get('checkIn') ?? dates.checkIn,
    checkOut: params.get('checkOut') ?? dates.checkOut,
    adults: Math.max(1, num(params.get('adults'), 2)),
    children: Math.max(0, num(params.get('children'), 0)),
    rooms: Math.max(1, num(params.get('rooms'), 1)),
    stars: list(params.get('stars')).map(Number).filter((s) => s >= 1 && s <= 5),
    types: list(params.get('types')).filter((t): t is HotelType => (HOTEL_TYPES as readonly string[]).includes(t)),
    minPrice: params.get('minPrice') ? num(params.get('minPrice'), 0) : null,
    maxPrice: params.get('maxPrice') ? num(params.get('maxPrice'), 0) : null,
    amenities: list(params.get('amenities')),
    sort: (params.get('sort') as HotelSort) || 'recommended',
  }
}

export function toSearchParams(state: Partial<SearchParamsState>) {
  const params = new URLSearchParams()
  Object.entries(state).forEach(([key, value]) => {
    if (value === null || value === undefined || value === '') return
    if (Array.isArray(value)) {
      if (value.length) params.set(key, value.join(','))
    } else params.set(key, String(value))
  })
  return params
}

/** Search state lives in the URL so results are shareable and survive a reload. */
export function useSearchState() {
  const [params, setParams] = useSearchParams()
  const state = useMemo(() => parseSearch(params), [params])

  const update = useCallback(
    (patch: Partial<SearchParamsState>) => setParams(toSearchParams({ ...parseSearch(params), ...patch }), { replace: true }),
    [params, setParams],
  )

  return [state, update] as const
}
