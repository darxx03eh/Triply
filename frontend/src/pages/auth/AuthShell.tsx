import type { ReactNode } from 'react'
import { Quote } from 'lucide-react'
import { Logo } from '@/components/layout/Logo'
import { stockImages } from '@/mocks/images'

export function AuthShell({ title, subtitle, children }: { title: string; subtitle: ReactNode; children: ReactNode }) {
  return (
    <div className="grid min-h-screen lg:grid-cols-2">
      <div className="relative hidden overflow-hidden bg-hero lg:block">
        <img src={stockImages.hotels[3]} alt="" className="absolute inset-0 size-full object-cover opacity-40 mix-blend-luminosity" />
        <div className="absolute inset-0 bg-gradient-to-t from-slate-950 via-slate-950/40 to-transparent" />
        <div className="relative flex h-full flex-col justify-between p-12 text-white">
          <Logo light />
          <div className="max-w-md">
            <Quote className="size-10 text-accent-400" />
            <p className="mt-4 text-2xl leading-snug font-semibold">
              “Booked a boutique hotel in Istanbul in two minutes. The easiest trip planning I’ve ever done.”
            </p>
            <p className="mt-4 text-sm text-white/70">Lina H. — travelled to 6 cities with Triply</p>
          </div>
        </div>
      </div>
      <div className="flex items-center justify-center px-4 py-12 sm:px-8">
        <div className="w-full max-w-md">
          <div className="mb-8 lg:hidden"><Logo /></div>
          <h1 className="text-3xl font-extrabold tracking-tight text-slate-900">{title}</h1>
          <p className="mt-2 text-slate-500">{subtitle}</p>
          <div className="mt-8">{children}</div>
        </div>
      </div>
    </div>
  )
}
