import { Link } from 'react-router'
import { Compass } from 'lucide-react'
import { Button } from '@/components/ui/Button'

export function NotFoundPage() {
  return (
    <div className="flex min-h-[70vh] flex-col items-center justify-center px-4 pt-16 text-center">
      <Compass className="size-14 text-brand-500" />
      <p className="mt-6 text-sm font-bold tracking-widest text-accent-500 uppercase">404</p>
      <h1 className="mt-2 text-4xl font-extrabold tracking-tight text-slate-900">Looks like you’re lost</h1>
      <p className="mt-3 max-w-md text-slate-500">The page you’re looking for doesn’t exist or has moved.</p>
      <Link to="/"><Button className="mt-8" size="lg">Back to home</Button></Link>
    </div>
  )
}
