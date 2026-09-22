import { useEffect, useMemo, useRef, useState } from 'react'
import { useSearchParams } from 'react-router'
import { useInfiniteQuery, useQuery } from '@tanstack/react-query'
import { SearchX, SlidersHorizontal, X } from 'lucide-react'
import { amenitiesApi } from '@/api/amenities.api'
import { citiesApi } from '@/api/cities.api'
import { HotelResultCard } from '@/components/hotel/HotelCards'
import { SearchBar } from '@/components/search/SearchBar'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { Checkbox, Input, Select } from '@/components/ui/Field'
import { EmptyState, ErrorState, Skeleton, Spinner } from '@/components/ui/Feedback'
import { Stars } from '@/components/ui/Stars'
import { useDebounce } from '@/hooks/useDebounce'
import { toSearchParams, useSearchState } from '@/hooks/useSearchState'
import { ApiError } from '@/lib/http'
import { sieve } from '@/lib/sieve'
import { searchService } from '@/services/search.service'
import { HOTEL_TYPES } from '@/types/hotel'
import type { HotelSort, SearchParamsState } from '@/types/search'

const PAGE_SIZE = 8

const sortOptions: { value: HotelSort; label: string }[] = [
  { value: 'recommended', label: 'Recommended' },
  { value: 'price-asc', label: 'Price: low to high' },
  { value: 'price-desc', label: 'Price: high to low' },
  { value: 'stars-desc', label: 'Stars: high to low' },
  { value: 'stars-asc', label: 'Stars: low to high' },
  { value: 'name-asc', label: 'Name: A to Z' },
]

export function SearchPage() {
  const [state, update] = useSearchState()
  const [params, setParams] = useSearchParams()
  const [filtersOpen, setFiltersOpen] = useState(false)
  const cityName = params.get('city')

  // Links such as "Trending destinations" pass a city name — resolve it to an id once.
  const cityLookup = useQuery({
    queryKey: ['cities', 'byName', cityName],
    queryFn: () => citiesApi.list({ filters: [sieve.equals('Name', cityName)], pageSize: 1 }),
    enabled: !!cityName && !state.cityId,
  })
  useEffect(() => {
    if (!cityName || state.cityId || !cityLookup.data) return
    const city = cityLookup.data.items[0]
    const next = toSearchParams({ ...state, cityId: city?.cityId ?? '', q: city ? '' : cityName })
    setParams(next, { replace: true })
  }, [cityName, cityLookup.data, state, setParams])

  const selectedCity = useQuery({
    queryKey: ['cities', 'selected', state.cityId],
    queryFn: () => citiesApi.list({ pageSize: 50 }).then((r) => r.items.find((c) => c.cityId === state.cityId) ?? null),
    enabled: !!state.cityId,
    staleTime: 5 * 60_000,
  })

  const amenities = useQuery({ queryKey: ['amenities'], queryFn: amenitiesApi.list, staleTime: 5 * 60_000 })

  const results = useInfiniteQuery({
    queryKey: ['search', { ...state }],
    queryFn: ({ pageParam }) => searchService.searchHotels(state, pageParam, PAGE_SIZE),
    initialPageParam: 1,
    getNextPageParam: (last) => (last.page < last.totalPages ? last.page + 1 : undefined),
    enabled: !cityName || !!state.cityId || cityLookup.isFetched,
  })

  const sentinel = useRef<HTMLDivElement>(null)
  useEffect(() => {
    const node = sentinel.current
    if (!node) return
    const observer = new IntersectionObserver(
      ([entry]) => entry.isIntersecting && results.hasNextPage && !results.isFetchingNextPage && results.fetchNextPage(),
      { rootMargin: '400px' },
    )
    observer.observe(node)
    return () => observer.disconnect()
  }, [results])

  const hotels = results.data?.pages.flatMap((page) => page.items) ?? []
  const firstPage = results.data?.pages[0]
  const total = firstPage?.totalCount ?? 0
  const nights = firstPage?.nights ?? 1
  const hotelQuery = useMemo(
    () =>
      `?${toSearchParams({
        checkIn: state.checkIn,
        checkOut: state.checkOut,
        adults: state.adults,
        children: state.children,
        rooms: state.rooms,
      })}`,
    [state.checkIn, state.checkOut, state.adults, state.children, state.rooms],
  )

  const toggle = <K extends 'stars' | 'types' | 'amenities'>(key: K, value: SearchParamsState[K][number]) => {
    const list = state[key] as (typeof value)[]
    update({ [key]: list.includes(value) ? list.filter((v) => v !== value) : [...list, value] } as Partial<SearchParamsState>)
  }

  const activeFilters =
    state.stars.length +
    state.types.length +
    state.amenities.length +
    (state.minPrice !== null || state.maxPrice !== null ? 1 : 0)
  const title = selectedCity.data
    ? `Hotels in ${selectedCity.data.name}`
    : state.q
      ? `Results for “${state.q}”`
      : 'All hotels'

  const filters = (
    <div className="space-y-7">
      <FilterGroup title="Star rating">
        {[5, 4, 3, 2, 1].map((star) => (
          <Checkbox
            key={star}
            checked={state.stars.includes(star)}
            onChange={() => toggle('stars', star)}
            label={<Stars value={star} />}
          />
        ))}
      </FilterGroup>

      <FilterGroup title="Hotel type">
        {HOTEL_TYPES.map((type) => (
          <Checkbox key={type} checked={state.types.includes(type)} onChange={() => toggle('types', type)} label={type} />
        ))}
      </FilterGroup>

      <FilterGroup title="Price per night">
        <PriceRangeFilter
          min={state.minPrice}
          max={state.maxPrice}
          onChange={(minPrice, maxPrice) => update({ minPrice, maxPrice })}
        />
      </FilterGroup>

      <FilterGroup title="Amenities">
        <div className="scrollbar-thin max-h-56 space-y-2.5 overflow-y-auto pr-1">
          {amenities.data?.map((amenity) => (
            <Checkbox
              key={amenity.amenityId}
              label={amenity.name}
              checked={state.amenities.includes(amenity.amenityId)}
              onChange={() => toggle('amenities', amenity.amenityId)}
            />
          ))}
        </div>
      </FilterGroup>

      {activeFilters > 0 && (
        <Button
          variant="outline"
          className="w-full"
          onClick={() => update({ stars: [], types: [], amenities: [], minPrice: null, maxPrice: null })}
        >
          Clear filters
        </Button>
      )}
    </div>
  )

  return (
    <div className="pt-16">
      <div className="border-b border-slate-200 bg-white">
        <div className="container-page py-5">
          <SearchBar
            variant="compact"
            initial={{ ...state, cityName: selectedCity.data?.name }}
            onSubmit={(next) =>
              update({
                q: next.q ?? '',
                cityId: next.cityId ?? '',
                checkIn: next.checkIn,
                checkOut: next.checkOut,
                adults: next.adults,
                children: next.children,
                rooms: next.rooms,
              })
            }
          />
        </div>
      </div>

      <div className="container-page grid gap-8 py-8 lg:grid-cols-[17rem_1fr]">
        <aside className="hidden lg:block">
          <Card className="sticky top-24 p-6">
            <div className="mb-6 flex items-center gap-2">
              <SlidersHorizontal className="size-5 text-brand-600" />
              <h2 className="font-bold text-slate-900">Filters</h2>
            </div>
            {filters}
          </Card>
        </aside>

        <div>
          <div className="mb-6 flex flex-wrap items-end justify-between gap-4">
            <div>
              <h1 className="text-2xl font-extrabold tracking-tight text-slate-900">{title}</h1>
              <p className="mt-1 text-sm text-slate-500">
                {results.isLoading
                  ? 'Searching…'
                  : `${total} ${total === 1 ? 'property' : 'properties'} available · ${nights} night${nights > 1 ? 's' : ''} · ${state.adults} adult${state.adults > 1 ? 's' : ''}${state.children ? `, ${state.children} child${state.children > 1 ? 'ren' : ''}` : ''} · ${state.rooms} room${state.rooms > 1 ? 's' : ''}`}
              </p>
            </div>
            <div className="flex items-center gap-2">
              <Button variant="outline" className="lg:hidden" onClick={() => setFiltersOpen(true)}>
                <SlidersHorizontal className="size-4" /> Filters {activeFilters > 0 && `(${activeFilters})`}
              </Button>
              <Select value={state.sort} onChange={(e) => update({ sort: e.target.value as HotelSort })} className="h-10 w-52">
                {sortOptions.map((option) => (
                  <option key={option.value} value={option.value}>{option.label}</option>
                ))}
              </Select>
            </div>
          </div>

          {results.isLoading ? (
            <div className="space-y-5">
              {Array.from({ length: 3 }, (_, i) => <Skeleton key={i} className="h-56 rounded-3xl" />)}
            </div>
          ) : results.isError ? (
            <ErrorState message={searchErrorMessage(results.error)} onRetry={() => results.refetch()} />
          ) : hotels.length === 0 ? (
            <EmptyState
              icon={<SearchX className="size-7" />}
              title="No hotels match your search"
              description="Try another destination or remove some filters."
              action={<Button variant="outline" onClick={() => setParams(new URLSearchParams())}>Reset search</Button>}
            />
          ) : (
            <div className="space-y-5">
              {hotels.map((hotel) => (
                <HotelResultCard key={hotel.hotelId} hotel={hotel} query={hotelQuery} nights={nights} rooms={firstPage?.rooms ?? 1} />
              ))}
              <div ref={sentinel} className="flex justify-center py-6">
                {results.isFetchingNextPage ? (
                  <Spinner />
                ) : !results.hasNextPage ? (
                  <p className="text-sm text-slate-400">You’ve reached the end of the list</p>
                ) : null}
              </div>
            </div>
          )}
        </div>
      </div>

      {filtersOpen && (
        <div className="fixed inset-0 z-50 lg:hidden">
          <div className="absolute inset-0 bg-slate-950/50" onClick={() => setFiltersOpen(false)} />
          <div className="absolute inset-x-0 bottom-0 max-h-[85vh] animate-scale-in overflow-y-auto rounded-t-3xl bg-white p-6">
            <div className="mb-6 flex items-center justify-between">
              <h2 className="text-lg font-bold">Filters</h2>
              <button onClick={() => setFiltersOpen(false)}><X className="size-5" /></button>
            </div>
            {filters}
            <Button className="mt-6 w-full" onClick={() => setFiltersOpen(false)}>Show {total} results</Button>
          </div>
        </div>
      )}
    </div>
  )
}

function FilterGroup({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <div>
      <p className="mb-3 text-sm font-bold text-slate-900">{title}</p>
      <div className="space-y-2.5">{children}</div>
    </div>
  )
}

function PriceRangeFilter({
  min,
  max,
  onChange,
}: {
  min: number | null
  max: number | null
  onChange: (min: number | null, max: number | null) => void
}) {
  const [from, setFrom] = useState(min?.toString() ?? '')
  const [to, setTo] = useState(max?.toString() ?? '')
  const debouncedFrom = useDebounce(from, 600)
  const debouncedTo = useDebounce(to, 600)

  useEffect(() => {
    setFrom(min?.toString() ?? '')
    setTo(max?.toString() ?? '')
  }, [min, max])

  useEffect(() => {
    const nextMin = debouncedFrom === '' ? null : Math.max(0, Number(debouncedFrom))
    const nextMax = debouncedTo === '' ? null : Math.max(0, Number(debouncedTo))
    if (nextMin !== min || nextMax !== max) onChange(nextMin, nextMax)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [debouncedFrom, debouncedTo])

  const invalid = from !== '' && to !== '' && Number(to) < Number(from)

  return (
    <>
      <div className="grid grid-cols-2 gap-2">
        <Input type="number" min={0} placeholder="Min $" value={from} onChange={(e) => setFrom(e.target.value)} invalid={invalid} />
        <Input type="number" min={0} placeholder="Max $" value={to} onChange={(e) => setTo(e.target.value)} invalid={invalid} />
      </div>
      {invalid && <p className="text-xs font-medium text-red-600">Max must be greater than min</p>}
    </>
  )
}

function searchErrorMessage(error: unknown) {
  const apiError = ApiError.from(error)
  return apiError.details.length ? apiError.details.join(' ') : apiError.message
}
