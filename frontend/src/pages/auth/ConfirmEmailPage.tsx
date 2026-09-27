import { useEffect, useRef } from 'react'
import { Link, useSearchParams } from 'react-router'
import { useMutation } from '@tanstack/react-query'
import { CheckCircle2, Loader2, XCircle } from 'lucide-react'
import { authApi } from '@/api/auth.api'
import { Button } from '@/components/ui/Button'
import { ApiError } from '@/lib/http'
import { AuthShell } from './AuthShell'

/** Opened from the confirmation e-mail; this page completes confirmation through the API. */
export function ConfirmEmailPage() {
  const [params] = useSearchParams()
  const email = params.get('email') ?? ''
  const token = params.get('token') ?? ''
  const started = useRef(false)
  const confirm = useMutation({ mutationFn: () => authApi.confirmEmail(email, token) })

  useEffect(() => {
    if (started.current || !email || !token) return
    started.current = true
    confirm.mutate()
  }, [email, token, confirm])

  const failed = confirm.isError || !email || !token

  return (
    <AuthShell title="E-mail confirmation" subtitle={email || 'Confirming your account'}>
      <div className="rounded-3xl border border-slate-200 bg-white p-8 text-center shadow-soft">
        {confirm.isPending || (!confirm.isSuccess && !failed) ? (
          <>
            <Loader2 className="mx-auto size-12 animate-spin text-brand-600" />
            <p className="mt-4 text-slate-600">Confirming your e-mail…</p>
          </>
        ) : confirm.isSuccess ? (
          <>
            <CheckCircle2 className="mx-auto size-14 text-emerald-500" />
            <p className="mt-4 text-lg font-bold text-slate-900">Your e-mail is confirmed</p>
            <p className="mt-1 text-sm text-slate-500">You can now sign in to your account.</p>
            <Link to="/login"><Button className="mt-6 w-full">Sign in</Button></Link>
          </>
        ) : (
          <>
            <XCircle className="mx-auto size-14 text-red-500" />
            <p className="mt-4 text-lg font-bold text-slate-900">We couldn’t confirm your e-mail</p>
            <p className="mt-1 text-sm text-slate-500">
              {confirm.error ? ApiError.from(confirm.error).message : 'The confirmation link is incomplete.'}
            </p>
            <Link to="/login"><Button variant="outline" className="mt-6 w-full">Back to sign in</Button></Link>
          </>
        )}
      </div>
    </AuthShell>
  )
}
