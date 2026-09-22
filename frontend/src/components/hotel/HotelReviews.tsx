import { useState } from 'react'
import { Link, useLocation } from 'react-router'
import { useInfiniteQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { MessageSquareText, Pencil, Star, Trash2 } from 'lucide-react'
import { toast } from 'sonner'
import { reviewsApi } from '@/api/reviews.api'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { Field, Input, Select, Textarea } from '@/components/ui/Field'
import { Badge, EmptyState, ErrorState, Skeleton } from '@/components/ui/Feedback'
import { ConfirmDialog } from '@/components/ui/Overlay'
import { RatingPill } from '@/components/ui/Stars'
import { useAuth } from '@/features/auth/useAuth'
import { cn } from '@/lib/cn'
import { applyServerErrors, toastError } from '@/lib/forms'
import { formatDate, initials } from '@/lib/format'
import { hotelService, REVIEWS_PAGE_SIZE } from '@/services/hotel.service'
import { REVIEW_SORTS, type Review } from '@/types/review'

const schema = z.object({
  rating: z.number().int().min(1, 'Choose a rating').max(5),
  title: z.string().trim().max(100, 'Title must not exceed 100 characters').optional(),
  comment: z.string().trim().min(10, 'Tell us a bit more (at least 10 characters)').max(1000, 'Comment must not exceed 1000 characters'),
})
type Values = z.infer<typeof schema>
const fieldNames = ['rating', 'title', 'comment']

export function HotelReviews({ hotelId, averageRating, reviewsCount }: {
  hotelId: string
  averageRating: number | null
  reviewsCount: number
}) {
  const { user } = useAuth()
  const location = useLocation()
  const queryClient = useQueryClient()
  const [sorts, setSorts] = useState<string>(REVIEW_SORTS[0].value)
  const [editing, setEditing] = useState<Review | null>(null)
  const [writing, setWriting] = useState(false)
  const [deleting, setDeleting] = useState<Review | null>(null)

  const reviews = useInfiniteQuery({
    queryKey: ['hotel', hotelId, 'reviews', sorts],
    queryFn: ({ pageParam }) => hotelService.getReviews(hotelId, pageParam, sorts),
    initialPageParam: 1,
    getNextPageParam: (last) => (last.page < last.totalPages ? last.page + 1 : undefined),
  })

  const items = reviews.data?.pages.flatMap((page) => page.items) ?? []
  const mine = user ? items.find((review) => review.userId === user.id) : undefined

  const refresh = () => {
    queryClient.invalidateQueries({ queryKey: ['hotel', hotelId, 'reviews'] })
    queryClient.invalidateQueries({ queryKey: ['hotel', hotelId], exact: true })
  }

  const remove = useMutation({
    mutationFn: (id: string) => reviewsApi.remove(id),
    onSuccess: () => {
      toast.success('Review deleted')
      setDeleting(null)
      refresh()
    },
    onError: toastError,
  })

  const closeForm = () => {
    setWriting(false)
    setEditing(null)
  }

  return (
    <Card className="p-6 sm:p-8" id="reviews">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <h2 className="text-xl font-bold text-slate-900">Guest reviews</h2>
          <p className="mt-0.5 text-sm text-slate-500">
            {reviewsCount === 0 ? 'No reviews yet' : `${reviewsCount} verified review${reviewsCount > 1 ? 's' : ''}`}
          </p>
        </div>
        <div className="flex flex-wrap items-center gap-3">
          {averageRating !== null && <RatingPill value={averageRating} />}
          {reviewsCount > 1 && (
            <Select value={sorts} onChange={(e) => setSorts(e.target.value)} className="h-9 w-40 text-xs">
              {REVIEW_SORTS.map((sort) => <option key={sort.value} value={sort.value}>{sort.label}</option>)}
            </Select>
          )}
        </div>
      </div>

      <div className="mt-6">
        {!user ? (
          <div className="flex flex-col items-start justify-between gap-3 rounded-2xl border border-dashed border-slate-200 p-5 sm:flex-row sm:items-center">
            <p className="text-sm text-slate-600">Stayed here? Share your experience with other travellers.</p>
            <Link to="/login" state={{ from: `${location.pathname}${location.search}` }}>
              <Button variant="outline" size="sm">Log in to write a review</Button>
            </Link>
          </div>
        ) : writing || editing ? (
          <ReviewForm hotelId={hotelId} review={editing} onDone={() => { closeForm(); refresh() }} onCancel={closeForm} />
        ) : !mine ? (
          <Button variant="outline" onClick={() => setWriting(true)}>
            <MessageSquareText className="size-4" /> Write a review
          </Button>
        ) : null}
      </div>

      {reviews.isLoading ? (
        <div className="mt-6 grid gap-4 md:grid-cols-2">
          {Array.from({ length: 4 }, (_, i) => <Skeleton key={i} className="h-36 rounded-2xl" />)}
        </div>
      ) : reviews.isError ? (
        <div className="mt-6"><ErrorState message={(reviews.error as Error).message} onRetry={() => reviews.refetch()} /></div>
      ) : items.length === 0 ? (
        <EmptyState
          className="mt-6"
          icon={<Star className="size-7" />}
          title="Be the first to review"
          description="This hotel has no reviews yet."
        />
      ) : (
        <div className="mt-6 grid gap-4 md:grid-cols-2">
          {items.map((review) => {
            const own = review.userId === user?.id
            return (
              <div
                key={review.reviewId}
                className={cn('rounded-2xl p-5', own ? 'bg-brand-50/60 ring-1 ring-brand-200' : 'bg-slate-50', editing?.reviewId === review.reviewId && 'opacity-50')}
              >
                <div className="flex items-center justify-between gap-3">
                  <div className="flex min-w-0 items-center gap-3">
                    <span className="flex size-10 shrink-0 items-center justify-center rounded-full bg-gradient-to-br from-brand-500 to-accent-500 text-sm font-bold text-white">
                      {initials(review.authorName)}
                    </span>
                    <div className="min-w-0">
                      <p className="flex items-center gap-2 truncate text-sm font-bold text-slate-900">
                        {review.authorName} {own && <Badge tone="brand">You</Badge>}
                      </p>
                      <p className="text-xs text-slate-500">
                        {formatDate(review.createdAt)}
                        {review.modifiedAt && ' · edited'}
                      </p>
                    </div>
                  </div>
                  <span className="flex shrink-0 items-center gap-1 rounded-lg bg-white px-2 py-1 text-sm font-bold text-brand-700 shadow-sm">
                    <Star className="size-3.5 fill-amber-400 text-amber-400" /> {review.rating}
                  </span>
                </div>
                {review.title && <p className="mt-3 font-semibold text-slate-900">{review.title}</p>}
                <p className={cn('text-sm leading-relaxed text-slate-600', review.title ? 'mt-1' : 'mt-3')}>{review.comment}</p>
                {(own || user?.isAdmin) && (
                  <div className="mt-3 flex justify-end gap-1">
                    {own && (
                      <button
                        onClick={() => { setWriting(false); setEditing(review) }}
                        className="inline-flex items-center gap-1 rounded-lg px-2 py-1 text-xs font-semibold text-slate-500 hover:bg-white hover:text-brand-600"
                      >
                        <Pencil className="size-3.5" /> Edit
                      </button>
                    )}
                    <button
                      onClick={() => setDeleting(review)}
                      className="inline-flex items-center gap-1 rounded-lg px-2 py-1 text-xs font-semibold text-slate-500 hover:bg-white hover:text-red-600"
                    >
                      <Trash2 className="size-3.5" /> Delete
                    </button>
                  </div>
                )}
              </div>
            )
          })}
        </div>
      )}

      {reviews.hasNextPage && (
        <div className="mt-6 text-center">
          <Button variant="outline" onClick={() => reviews.fetchNextPage()} loading={reviews.isFetchingNextPage}>
            Show more reviews
          </Button>
        </div>
      )}
      {items.length > REVIEWS_PAGE_SIZE && !reviews.hasNextPage && (
        <p className="mt-6 text-center text-xs text-slate-400">You’ve reached the end of the reviews.</p>
      )}

      <ConfirmDialog
        open={!!deleting}
        onClose={() => setDeleting(null)}
        onConfirm={() => deleting && remove.mutate(deleting.reviewId)}
        loading={remove.isPending}
        title="Delete review?"
        description={deleting?.userId === user?.id ? 'Your review will be removed permanently.' : <>The review by {deleting?.authorName} will be removed permanently.</>}
      />
    </Card>
  )
}

function ReviewForm({ hotelId, review, onDone, onCancel }: {
  hotelId: string
  review: Review | null
  onDone: () => void
  onCancel: () => void
}) {
  const form = useForm<Values>({
    resolver: zodResolver(schema),
    defaultValues: { rating: review?.rating ?? 0, title: review?.title ?? '', comment: review?.comment ?? '' },
  })
  const { register, handleSubmit, watch, setValue, setError, formState: { errors } } = form

  const save = useMutation({
    mutationFn: (values: Values) => {
      const body = { ...values, title: values.title || null }
      return review ? reviewsApi.update(review.reviewId, body) : reviewsApi.create(hotelId, body)
    },
    onSuccess: () => {
      toast.success(review ? 'Review updated' : 'Thanks for your review!')
      onDone()
    },
    onError: (error) => applyServerErrors(error, setError, fieldNames),
  })

  return (
    <form onSubmit={handleSubmit((values) => save.mutate(values))} className="space-y-4 rounded-2xl border border-slate-200 p-5">
      <p className="font-semibold text-slate-900">{review ? 'Edit your review' : 'Write a review'}</p>
      <Field label="Your rating" required error={errors.rating?.message}>
        <StarPicker value={watch('rating')} onChange={(value) => setValue('rating', value, { shouldValidate: true })} />
      </Field>
      <Field label="Title" error={errors.title?.message}>
        <Input placeholder="Sum up your stay" {...register('title')} invalid={!!errors.title} />
      </Field>
      <Field label="Your review" required error={errors.comment?.message} hint={`${watch('comment')?.length ?? 0} / 1000`}>
        <Textarea rows={4} placeholder="What did you like? What could be better?" {...register('comment')} invalid={!!errors.comment} />
      </Field>
      <div className="flex justify-end gap-2">
        <Button type="button" variant="outline" onClick={onCancel}>Cancel</Button>
        <Button type="submit" loading={save.isPending}>{review ? 'Save changes' : 'Post review'}</Button>
      </div>
    </form>
  )
}

const ratingLabels = ['', 'Terrible', 'Poor', 'Okay', 'Very good', 'Excellent']

function StarPicker({ value, onChange }: { value: number; onChange: (value: number) => void }) {
  const [hover, setHover] = useState(0)
  const shown = hover || value
  return (
    <div className="flex items-center gap-3" onMouseLeave={() => setHover(0)}>
      <div className="flex gap-1">
        {[1, 2, 3, 4, 5].map((star) => (
          <button
            key={star}
            type="button"
            onClick={() => onChange(star)}
            onMouseEnter={() => setHover(star)}
            aria-label={`${star} star${star > 1 ? 's' : ''}`}
            className="cursor-pointer transition hover:scale-110"
          >
            <Star className={cn('size-7', star <= shown ? 'fill-amber-400 text-amber-400' : 'fill-slate-200 text-slate-200')} />
          </button>
        ))}
      </div>
      {shown > 0 && <span className="text-sm font-semibold text-slate-600">{ratingLabels[shown]}</span>}
    </div>
  )
}
