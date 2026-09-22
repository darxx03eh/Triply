import { useState } from 'react'
import { Link, useLocation, useNavigate } from 'react-router'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Eye, EyeOff, Lock, User } from 'lucide-react'
import { toast } from 'sonner'
import { Button } from '@/components/ui/Button'
import { Field, Input } from '@/components/ui/Field'
import { useAuth } from '@/features/auth/useAuth'
import { applyServerErrors } from '@/lib/forms'
import { AuthShell } from './AuthShell'

const schema = z.object({
  identifier: z.string().trim().min(1, 'Enter your username, e-mail or phone number'),
  password: z.string().min(1, 'Enter your password'),
})
type Values = z.infer<typeof schema>

export function LoginPage() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const [show, setShow] = useState(false)
  const { register, handleSubmit, setError, formState: { errors, isSubmitting } } = useForm<Values>({ resolver: zodResolver(schema) })

  const onSubmit = async (values: Values) => {
    try {
      const user = await login(values)
      toast.success(`Welcome back, ${user.name.split(' ')[0]}!`)
      const from = (location.state as { from?: string } | null)?.from
      navigate(from ?? (user.isAdmin ? '/admin' : '/'), { replace: true })
    } catch (error) {
      applyServerErrors(error, setError, ['identifier', 'password'])
    }
  }

  return (
    <AuthShell title="Welcome back" subtitle={<>New to Triply? <Link to="/register" className="font-semibold text-brand-600 hover:text-brand-700">Create an account</Link></>}>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
        <Field label="Username or e-mail" error={errors.identifier?.message}>
          <Input autoFocus autoComplete="username" icon={<User className="size-4" />} placeholder="admin" {...register('identifier')} invalid={!!errors.identifier} />
        </Field>
        <Field label="Password" error={errors.password?.message}>
          <div className="relative">
            <Input type={show ? 'text' : 'password'} autoComplete="current-password" icon={<Lock className="size-4" />} placeholder="••••••••" {...register('password')} invalid={!!errors.password} />
            <button type="button" onClick={() => setShow((v) => !v)} className="absolute top-1/2 right-3.5 -translate-y-1/2 text-slate-400 hover:text-slate-600" aria-label="Toggle password">
              {show ? <EyeOff className="size-4" /> : <Eye className="size-4" />}
            </button>
          </div>
        </Field>
        <Button type="submit" size="lg" className="w-full" loading={isSubmitting}>Sign in</Button>
      </form>
    </AuthShell>
  )
}
