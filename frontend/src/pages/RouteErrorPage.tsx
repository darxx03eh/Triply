import { isRouteErrorResponse, Link, useRouteError } from 'react-router'
import { AlertTriangle } from 'lucide-react'
import { Button } from '@/components/ui/Button'

export function RouteErrorPage() {
  const error = useRouteError()
  const message = isRouteErrorResponse(error)
    ? `${error.status} — ${error.statusText}`
    : error instanceof Error
      ? error.message
      : 'Unexpected error'

  return (
    <div className="flex min-h-screen flex-col items-center justify-center bg-slate-50 px-4 text-center">
      <span className="flex size-16 items-center justify-center rounded-2xl bg-amber-50 text-amber-500">
        <AlertTriangle className="size-8" />
      </span>
      <h1 className="mt-6 text-3xl font-extrabold tracking-tight text-slate-900">Something went wrong</h1>
      <p className="mt-2 max-w-md text-slate-500">{message}</p>
      <div className="mt-8 flex gap-3">
        <Button variant="outline" onClick={() => window.location.reload()}>Reload page</Button>
        <Link to="/"><Button>Back to home</Button></Link>
      </div>
    </div>
  )
}
