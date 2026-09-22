import { useState } from 'react'
import { Link } from 'react-router'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { MailCheck } from 'lucide-react'
import { Button } from '@/components/ui/Button'
import { Field, Input } from '@/components/ui/Field'
import { useAuth } from '@/features/auth/useAuth'
import { applyServerErrors } from '@/lib/forms'
import type { RegisterRequest } from '@/types/auth'
import { AuthShell } from './AuthShell'

// Mirrors Triply.Application/Features/Authentications/Commands/Register/RegisterUserRequestValidator.cs
const schema = z
  .object({
    firstName: z.string().trim().min(3, 'At least 3 characters').max(100),
    lastName: z.string().trim().min(3, 'At least 3 characters').max(100),
    username: z.string().trim().min(3, 'At least 3 characters').regex(/^[a-zA-Z0-9._]+$/, 'Letters, numbers, dots and underscores only'),
    email: z.email('Enter a valid e-mail'),
    phoneNumber: z.string().regex(/^\+[1-9][0-9]{7,14}$/, 'Use the international format, e.g. +970591234567'),
    dateOfBirth: z.string().optional(),
    password: z.string().min(8, 'At least 8 characters'),
    confirmPassword: z.string(),
  })
  .refine((v) => v.password === v.confirmPassword, { path: ['confirmPassword'], message: 'Passwords do not match' })
  .refine((v) => !v.dateOfBirth || new Date(v.dateOfBirth) <= new Date(new Date().setFullYear(new Date().getFullYear() - 18)), {
    path: ['dateOfBirth'],
    message: 'You must be at least 18 years old',
  })
type Values = z.infer<typeof schema>

const fields = ['firstName', 'lastName', 'username', 'email', 'phoneNumber', 'dateOfBirth', 'password', 'confirmPassword']

export function RegisterPage() {
  const { register: signUp } = useAuth()
  const [sentTo, setSentTo] = useState<string | null>(null)
  const { register, handleSubmit, setError, formState: { errors, isSubmitting } } = useForm<Values>({ resolver: zodResolver(schema) })

  const onSubmit = async (values: Values) => {
    try {
      const body: RegisterRequest = { ...values, dateOfBirth: values.dateOfBirth || null }
      const result = await signUp(body)
      setSentTo(result.email)
    } catch (error) {
      applyServerErrors(error, setError, fields)
    }
  }

  if (sentTo)
    return (
      <AuthShell title="Check your inbox" subtitle="One last step to activate your account.">
        <div className="rounded-3xl border border-slate-200 bg-white p-8 text-center shadow-soft">
          <span className="mx-auto flex size-16 items-center justify-center rounded-full bg-brand-50 text-brand-600"><MailCheck className="size-8" /></span>
          <p className="mt-5 text-slate-600">We sent a confirmation link to</p>
          <p className="font-bold text-slate-900">{sentTo}</p>
          <p className="mt-4 text-sm text-slate-500">Open the link to confirm your e-mail, then sign in.</p>
          <Link to="/login"><Button className="mt-6 w-full">Go to sign in</Button></Link>
        </div>
      </AuthShell>
    )

  return (
    <AuthShell title="Create your account" subtitle={<>Already have one? <Link to="/login" className="font-semibold text-brand-600 hover:text-brand-700">Sign in</Link></>}>
      <form onSubmit={handleSubmit(onSubmit)} className="grid gap-4 sm:grid-cols-2">
        <Field label="First name" required error={errors.firstName?.message}><Input {...register('firstName')} invalid={!!errors.firstName} /></Field>
        <Field label="Last name" required error={errors.lastName?.message}><Input {...register('lastName')} invalid={!!errors.lastName} /></Field>
        <Field label="Username" required error={errors.username?.message}><Input autoComplete="username" {...register('username')} invalid={!!errors.username} /></Field>
        <Field label="Date of birth" error={errors.dateOfBirth?.message}><Input type="date" {...register('dateOfBirth')} invalid={!!errors.dateOfBirth} /></Field>
        <Field label="E-mail" required className="sm:col-span-2" error={errors.email?.message}><Input type="email" autoComplete="email" {...register('email')} invalid={!!errors.email} /></Field>
        <Field label="Phone number" required className="sm:col-span-2" error={errors.phoneNumber?.message}><Input placeholder="+970591234567" {...register('phoneNumber')} invalid={!!errors.phoneNumber} /></Field>
        <Field label="Password" required error={errors.password?.message}><Input type="password" autoComplete="new-password" {...register('password')} invalid={!!errors.password} /></Field>
        <Field label="Confirm password" required error={errors.confirmPassword?.message}><Input type="password" autoComplete="new-password" {...register('confirmPassword')} invalid={!!errors.confirmPassword} /></Field>
        <Button type="submit" size="lg" className="mt-2 w-full sm:col-span-2" loading={isSubmitting}>Create account</Button>
      </form>
    </AuthShell>
  )
}
