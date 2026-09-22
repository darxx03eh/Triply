import type { ReactNode } from 'react'
import { Card } from '../ui/Card'

export function AdminPageHeader({ title, description, actions }: { title: string; description: string; actions?: ReactNode }) {
  return (
    <div className="mb-6 flex flex-col justify-between gap-4 sm:flex-row sm:items-end">
      <div>
        <h2 className="text-2xl font-extrabold tracking-tight text-slate-900">{title}</h2>
        <p className="mt-1 text-sm text-slate-500">{description}</p>
      </div>
      {actions && <div className="flex gap-2">{actions}</div>}
    </div>
  )
}

export function AdminTableCard({ toolbar, children }: { toolbar?: ReactNode; children: ReactNode }) {
  return (
    <Card className="overflow-hidden">
      {toolbar && <div className="flex flex-col gap-3 border-b border-slate-100 p-4 lg:flex-row lg:items-center">{toolbar}</div>}
      {children}
    </Card>
  )
}

export function DateCell({ value }: { value: string | null }) {
  if (!value) return <span className="text-slate-300">—</span>
  const date = new Date(value)
  return (
    <span className="text-slate-600">
      {date.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' })}
      <span className="block text-xs text-slate-400">{date.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' })}</span>
    </span>
  )
}
