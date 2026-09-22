import type { HTMLAttributes, ReactNode } from 'react'
import { cn } from '@/lib/cn'

export function Card({ className, ...props }: HTMLAttributes<HTMLDivElement>) {
  return <div className={cn('rounded-2xl border border-slate-200/70 bg-white shadow-soft', className)} {...props} />
}

export function SectionHeader({
  eyebrow,
  title,
  description,
  action,
}: {
  eyebrow?: string
  title: string
  description?: string
  action?: ReactNode
}) {
  return (
    <div className="mb-6 flex flex-col items-start justify-between gap-3 sm:flex-row sm:items-end">
      <div>
        {eyebrow && <p className="mb-1 text-xs font-bold tracking-widest text-accent-500 uppercase">{eyebrow}</p>}
        <h2 className="text-2xl font-extrabold tracking-tight text-slate-900 sm:text-3xl">{title}</h2>
        {description && <p className="mt-1.5 max-w-2xl text-slate-500">{description}</p>}
      </div>
      {action}
    </div>
  )
}
