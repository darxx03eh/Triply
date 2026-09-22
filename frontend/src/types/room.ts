export const ROOM_TYPES = ['Single', 'Double', 'Suite'] as const
export type RoomType = (typeof ROOM_TYPES)[number]

export interface Room {
  roomId: string
  hotelId: string
  hotelName: string
  number: string
  roomType: RoomType
  adultCapacity: number
  childCapacity: number
  pricePerNight: number
  isAvailable: boolean
  description: string | null
  isDeleted: boolean
  createdAt: string
  modifiedAt: string | null
  rowVersion: string
}

export interface RoomInput {
  number: string
  roomType: RoomType
  adultCapacity: number
  childCapacity: number
  pricePerNight: number
  isAvailable: boolean
  description?: string | null
}

export interface CreateRoomInput extends RoomInput {
  hotelId: string
}

export interface UpdateRoomInput extends RoomInput {
  rowVersion: string
}
