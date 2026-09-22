/** Triply.Application/DTOs/Deals/DealResponse.cs */
export interface Deal {
  dealId: string
  roomId: string
  title: string
  discountPercentage: number
  startsAt: string
  endsAt: string
  isFeatured: boolean
  createdAt: string
}

export interface DealInput {
  title: string
  discountPercentage: number
  startsAt: string
  endsAt: string
  isFeatured: boolean
}

export interface CreateDealInput extends DealInput {
  roomId: string
}

export type DealStatus = 'Active' | 'Scheduled' | 'Expired'

export const dealStatus = (deal: Pick<Deal, 'startsAt' | 'endsAt'>, now = Date.now()): DealStatus =>
  new Date(deal.endsAt).getTime() <= now ? 'Expired' : new Date(deal.startsAt).getTime() > now ? 'Scheduled' : 'Active'
