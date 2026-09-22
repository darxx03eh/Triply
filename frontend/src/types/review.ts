/** Triply.Application/DTOs/Reviews/ReviewResponse.cs */
export interface Review {
  reviewId: string
  hotelId: string
  userId: string
  authorName: string
  rating: number
  title: string | null
  comment: string
  createdAt: string
  modifiedAt: string | null
}

export interface ReviewInput {
  rating: number
  title?: string | null
  comment: string
}

export const REVIEW_SORTS = [
  { value: '-CreatedAt', label: 'Newest first' },
  { value: '-Rating', label: 'Highest rated' },
  { value: 'Rating', label: 'Lowest rated' },
] as const
