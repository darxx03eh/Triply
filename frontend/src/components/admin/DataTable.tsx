import type { ReactNode } from 'react'
import { cn } from '@/lib/cn'
import { EmptyState, ErrorState, Skeleton } from '../ui/Feedback'

export interface Column<T> {
  key: string
  header: string
  cell: (row: T) => ReactNode
  className?: string
}

export function DataTable<T>({
  columns,
  rows,
  rowKey,
  loading,
  error,
  onRetry,
  onRowClick,
  emptyTitle = 'Nothing here yet',
  emptyDescription,
  rowClassName,
}: {
  columns: Column<T>[]
  rows: T[] | undefined
  rowKey: (row: T) => string
  loading?: boolean
  error?: unknown
  onRetry?: () => void
  onRowClick?: (row: T) => void
  emptyTitle?: string
  emptyDescription?: string
  rowClassName?: (row: T) => string | undefined
}) {
  if (error) return <div className="p-6"><ErrorState message={(error as Error).message} onRetry={onRetry} /></div>

  return (
    <div className="scrollbar-thin overflow-x-auto">
      <table className="w-full min-w-[760px] text-left text-sm">
        <thead>
          <tr className="border-b border-slate-100 bg-slate-50/70">
            {columns.map((column) => (
              <th key={column.key} className={cn('px-5 py-3 text-xs font-bold tracking-wide whitespace-nowrap text-slate-500 uppercase', column.className)}>
                {column.header}
              </th>
            ))}
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100">
          {loading
            ? Array.from({ length: 6 }, (_, i) => (
                <tr key={i}>
                  {columns.map((column) => (
                    <td key={column.key} className="px-5 py-4"><Skeleton className="h-4 w-full max-w-40" /></td>
                  ))}
                </tr>
              ))
            : rows?.map((row) => (
                <tr
                  key={rowKey(row)}
                  onClick={onRowClick ? () => onRowClick(row) : undefined}
                  className={cn('transition-colors', onRowClick && 'cursor-pointer hover:bg-brand-50/40', rowClassName?.(row))}
                >
                  {columns.map((column) => (
                    <td key={column.key} className={cn('px-5 py-3.5 whitespace-nowrap text-slate-700', column.className)}>
                      {column.cell(row)}
                    </td>
                  ))}
                </tr>
              ))}
        </tbody>
      </table>
      {!loading && rows?.length === 0 && (
        <div className="p-6"><EmptyState title={emptyTitle} description={emptyDescription} className="border-0" /></div>
      )}
    </div>
  )
}
