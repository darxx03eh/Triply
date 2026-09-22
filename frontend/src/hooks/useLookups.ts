import { useQuery } from '@tanstack/react-query'
import { amenitiesApi } from '@/api/amenities.api'
import { citiesApi } from '@/api/cities.api'
import { hotelsApi } from '@/api/hotels.api'
import { roomsApi } from '@/api/rooms.api'

/** Small reference lists used by admin selects (the API caps pageSize at 50). */
export const useCityOptions = () =>
  useQuery({
    queryKey: ['lookup', 'cities'],
    queryFn: () => citiesApi.list({ sorts: 'Name', pageSize: 50 }).then((r) => r.items.filter((c) => !c.isDeleted)),
    staleTime: 60_000,
  })

export const useHotelOptions = () =>
  useQuery({
    queryKey: ['lookup', 'hotels'],
    queryFn: () => hotelsApi.list({ sorts: 'Name', pageSize: 50 }).then((r) => r.items),
    staleTime: 60_000,
  })

export const useAmenityOptions = () =>
  useQuery({ queryKey: ['lookup', 'amenities'], queryFn: amenitiesApi.list, staleTime: 60_000 })

/** Every room (all pages) — used to pick and label the room of a deal. */
export const useRoomOptions = () =>
  useQuery({
    queryKey: ['lookup', 'rooms'],
    queryFn: async () => {
      const first = await roomsApi.list({ sorts: 'Number', pageSize: 50 })
      const rest = await Promise.all(
        Array.from({ length: first.totalPages - 1 }, (_, i) => roomsApi.list({ sorts: 'Number', pageSize: 50, page: i + 2 })),
      )
      return [first, ...rest].flatMap((page) => page.items).filter((room) => !room.isDeleted)
    },
    staleTime: 60_000,
  })
