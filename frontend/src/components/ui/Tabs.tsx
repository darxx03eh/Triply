import { cn } from '@/lib/cn'

export function Tabs<T extends string>({
  tabs,
  value,
  onChange,
  className,
}: {
  tabs: { value: T; label: string; count?: number }[]
  value: T
  onChange: (value: T) => void
  className?: string
}) {
  return (
    <div className={cn('inline-flex rounded-xl bg-slate-100 p-1', className)}>
      {tabs.map((tab) => (
        <button
          key={tab.value}
          type="button"
          onClick={() => onChange(tab.value)}
          className={cn(
            'cursor-pointer rounded-lg px-3.5 py-1.5 text-sm font-semibold transition',
            value === tab.value ? 'bg-white text-slate-900 shadow-sm' : 'text-slate-500 hover:text-slate-800',
          )}
        >
          {tab.label}
          {tab.count !== undefined && <span className="ml-1.5 text-xs text-slate-400">{tab.count}</span>}
        </button>
      ))}
    </div>
  )
}
