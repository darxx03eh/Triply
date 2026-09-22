import { forwardRef, type InputHTMLAttributes, type ReactNode, type SelectHTMLAttributes, type TextareaHTMLAttributes } from 'react'
import { ChevronDown } from 'lucide-react'
import { cn } from '@/lib/cn'

const control =
  'w-full rounded-xl border border-slate-200 bg-white px-3.5 text-sm text-slate-800 shadow-xs transition outline-none placeholder:text-slate-400 hover:border-slate-300 focus:border-brand-500 focus:ring-4 focus:ring-brand-500/15 disabled:bg-slate-50 disabled:text-slate-400 aria-invalid:border-red-400 aria-invalid:focus:ring-red-500/15'

interface FieldProps {
  label?: string
  error?: string
  hint?: string
  required?: boolean
  className?: string
  children: ReactNode
}

export function Field({ label, error, hint, required, className, children }: FieldProps) {
  return (
    <label className={cn('block space-y-1.5', className)}>
      {label && (
        <span className="text-sm font-medium text-slate-700">
          {label}
          {required && <span className="ml-0.5 text-accent-500">*</span>}
        </span>
      )}
      {children}
      {error ? (
        <span className="block text-xs font-medium text-red-600">{error}</span>
      ) : hint ? (
        <span className="block text-xs text-slate-500">{hint}</span>
      ) : null}
    </label>
  )
}

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  icon?: ReactNode
  invalid?: boolean
}

export const Input = forwardRef<HTMLInputElement, InputProps>(({ className, icon, invalid, ...props }, ref) => (
  <div className="relative">
    {icon && <span className="pointer-events-none absolute top-1/2 left-3.5 -translate-y-1/2 text-slate-400">{icon}</span>}
    <input ref={ref} aria-invalid={invalid || undefined} className={cn(control, 'h-11', icon && 'pl-10', className)} {...props} />
  </div>
))
Input.displayName = 'Input'

export const Textarea = forwardRef<HTMLTextAreaElement, TextareaHTMLAttributes<HTMLTextAreaElement> & { invalid?: boolean }>(
  ({ className, invalid, ...props }, ref) => (
    <textarea ref={ref} aria-invalid={invalid || undefined} className={cn(control, 'min-h-24 py-2.5', className)} {...props} />
  ),
)
Textarea.displayName = 'Textarea'

export const Select = forwardRef<HTMLSelectElement, SelectHTMLAttributes<HTMLSelectElement> & { invalid?: boolean }>(
  ({ className, invalid, children, ...props }, ref) => (
    <div className="relative">
      <select
        ref={ref}
        aria-invalid={invalid || undefined}
        className={cn(control, 'h-11 cursor-pointer appearance-none pr-10', className)}
        {...props}
      >
        {children}
      </select>
      <ChevronDown className="pointer-events-none absolute top-1/2 right-3.5 size-4 -translate-y-1/2 text-slate-400" />
    </div>
  ),
)
Select.displayName = 'Select'

export function Checkbox({ label, className, ...props }: InputHTMLAttributes<HTMLInputElement> & { label: ReactNode }) {
  return (
    <label className={cn('flex cursor-pointer items-center gap-2.5 text-sm text-slate-700 select-none', className)}>
      <input
        type="checkbox"
        className="size-4 cursor-pointer rounded border-slate-300 accent-brand-600 disabled:cursor-not-allowed"
        {...props}
      />
      {label}
    </label>
  )
}

export function Switch({
  checked,
  onChange,
  label,
}: {
  checked: boolean
  onChange: (value: boolean) => void
  label?: ReactNode
}) {
  return (
    <button
      type="button"
      role="switch"
      aria-checked={checked}
      onClick={() => onChange(!checked)}
      className="inline-flex cursor-pointer items-center gap-3 text-sm text-slate-700"
    >
      <span
        className={cn(
          'relative inline-flex h-6 w-11 items-center rounded-full transition-colors',
          checked ? 'bg-brand-600' : 'bg-slate-300',
        )}
      >
        <span className={cn('inline-block size-5 rounded-full bg-white shadow transition-transform', checked ? 'translate-x-5.5' : 'translate-x-0.5')} />
      </span>
      {label}
    </button>
  )
}
