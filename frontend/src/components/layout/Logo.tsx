import { Link } from 'react-router'
import { cn } from '@/lib/cn'

export function Logo({ light, className }: { light?: boolean; className?: string }) {
  return (
    <Link to="/" className={cn('group inline-flex items-center gap-2.5', className)}>
      <span className="flex size-9 items-center justify-center rounded-xl bg-gradient-to-br from-brand-500 to-accent-500 shadow-lg shadow-brand-500/30 transition group-hover:rotate-6">
        <svg viewBox="0 0 64 64" className="size-6" aria-hidden>
          <path d="M18 40c6-14 22-14 28 0" stroke="#fff" strokeWidth="6" fill="none" strokeLinecap="round" />
          <circle cx="32" cy="22" r="6" fill="#fff" />
        </svg>
      </span>
      <span className={cn('text-xl font-extrabold tracking-tight', light ? 'text-white' : 'text-slate-900')}>
        Triply
      </span>
    </Link>
  )
}
