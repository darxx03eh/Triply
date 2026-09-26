import { http, unwrap } from '@/lib/http'
import { toSieveParams, type SieveQuery } from '@/lib/sieve'
import type { ApiResponse, PagedResult } from '@/types/api'
import type { CreateRoomInput, Room, RoomImage, UpdateRoomInput } from '@/types/room'

/**
 * Triply.Api/Endpoints/RoomEndpoints.cs
 * Sieve names: Number, HotelId, room (type), adults, children, price, available, CreatedAt, ModifiedAt.
 */
export const roomsApi = {
  list: (query: SieveQuery = {}) =>
    unwrap(http.get<ApiResponse<PagedResult<Room>>>('/rooms', { params: toSieveParams(query) })),

  byHotel: (hotelId: string, query: SieveQuery = {}) =>
    unwrap(
      http.get<ApiResponse<PagedResult<Room>>>(`/hotels/${hotelId}/rooms`, { params: toSieveParams(query) }),
    ),

  get: (id: string) => unwrap(http.get<ApiResponse<Room>>(`/rooms/${id}`)),

  create: (body: CreateRoomInput) => unwrap(http.post<ApiResponse<Room>>('/rooms', body)),

  update: (id: string, body: UpdateRoomInput) => unwrap(http.put<ApiResponse<Room>>(`/rooms/${id}`, body)),

  remove: (id: string) => http.delete(`/rooms/${id}`).then(() => undefined),

  images: (id: string) => unwrap(http.get<ApiResponse<RoomImage[]>>(`/rooms/${id}/images`)),

  uploadImage: (id: string, file: File, displayOrder?: number) => {
    const form = new FormData()
    form.append('file', file)
    if (displayOrder) form.append('displayOrder', String(displayOrder))
    return unwrap(http.post<ApiResponse<RoomImage>>(`/rooms/${id}/images`, form))
  },

  removeImage: (id: string, imageId: string) =>
    http.delete(`/rooms/${id}/images/${imageId}`).then(() => undefined),
}
