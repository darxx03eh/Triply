import { Star } from 'lucide-react'
import { cn } from '@/lib/cn'

export function Stars({ value, size = 'sm', className }: { value: number; size?: 'sm' | 'md'; className?: string }) {
  return (
    <span className={cn('inline-flex items-center gap-0.5', className)} aria-label={`${value} stars`}>
      {Array.from({ length: 5 }, (_, i) => (
        <Star
          key={i}
          className={cn(
            size === 'sm' ? 'size-3.5' : 'size-4.5',
            i < Math.round(value) ? 'fill-amber-400 text-amber-400' : 'fill-slate-200 text-slate-200',
          )}
        />
      ))}
    </span>
  )
}

export function RatingPill({ value }: { value: number }) {
  const label = value >= 4.6 ? 'Exceptional' : value >= 4.2 ? 'Excellent' : value >= 3.8 ? 'Very good' : 'Good'
  return (
    <span className="inline-flex items-center gap-2">
      <span className="rounded-lg rounded-bl-none bg-brand-600 px-2 py-1 text-sm font-bold text-white">{value.toFixed(1)}</span>
      <span className="text-sm font-semibold text-slate-700">{label}</span>
    </span>
  )
}
