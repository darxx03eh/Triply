import { useState } from 'react'
import { Building2 } from 'lucide-react'
import { cn } from '@/lib/cn'

const gradients = [
  'from-brand-500 via-violet-500 to-accent-500',
  'from-sky-500 via-brand-500 to-violet-600',
  'from-emerald-500 via-teal-500 to-sky-600',
  'from-amber-400 via-orange-500 to-accent-500',
  'from-fuchsia-500 via-brand-600 to-slate-900',
]

const pick = (seed: string) => gradients[[...seed].reduce((sum, c) => sum + c.charCodeAt(0), 0) % gradients.length]

/** Image with a designed gradient fallback when there is no URL or it fails to load. */
export function SmartImage({
  src,
  alt,
  seed,
  className,
  label,
}: {
  src?: string | null
  alt: string
  seed?: string
  className?: string
  label?: string
}) {
  const [failed, setFailed] = useState(false)

  if (!src || failed) {
    return (
      <div className={cn('relative flex items-center justify-center overflow-hidden bg-gradient-to-br text-white', pick(seed ?? alt), className)}>
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_30%_20%,rgba(255,255,255,0.35),transparent_55%)]" />
        <div className="relative flex flex-col items-center gap-1 px-3 text-center">
          <Building2 className="size-8 opacity-90" />
          {label && <span className="line-clamp-2 text-sm font-semibold opacity-95">{label}</span>}
        </div>
      </div>
    )
  }

  return <img src={src} alt={alt} loading="lazy" onError={() => setFailed(true)} className={cn('object-cover', className)} />
}
