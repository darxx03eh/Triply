import { ChevronLeft, ChevronRight } from 'lucide-react'
import { Button } from './Button'

export function Pagination({
  page,
  totalPages,
  totalCount,
  pageSize,
  onChange,
}: {
  page: number
  totalPages: number
  totalCount: number
  pageSize: number
  onChange: (page: number) => void
}) {
  if (totalCount === 0) return null
  const from = (page - 1) * pageSize + 1
  const to = Math.min(page * pageSize, totalCount)
  return (
    <div className="flex flex-col items-center justify-between gap-3 border-t border-slate-100 px-5 py-3.5 sm:flex-row">
      <p className="text-sm text-slate-500">
        Showing <span className="font-semibold text-slate-800">{from}</span>–
        <span className="font-semibold text-slate-800">{to}</span> of{' '}
        <span className="font-semibold text-slate-800">{totalCount}</span>
      </p>
      <div className="flex items-center gap-1.5">
        <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => onChange(page - 1)}>
          <ChevronLeft className="size-4" /> Prev
        </Button>
        <span className="px-2 text-sm font-medium text-slate-600">
          {page} / {Math.max(1, totalPages)}
        </span>
        <Button variant="outline" size="sm" disabled={page >= totalPages} onClick={() => onChange(page + 1)}>
          Next <ChevronRight className="size-4" />
        </Button>
      </div>
    </div>
  )
}
