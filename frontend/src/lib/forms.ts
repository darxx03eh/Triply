import type { FieldValues, Path, UseFormSetError } from 'react-hook-form'
import { toast } from 'sonner'
import { ApiError } from './http'

/** Maps backend validation errors (camelCase field names) onto react-hook-form fields and toasts the rest. */
export function applyServerErrors<T extends FieldValues>(error: unknown, setError: UseFormSetError<T>, fields: string[]) {
  const apiError = ApiError.from(error)
  const unmatched: string[] = [...apiError.general]
  Object.entries(apiError.fields).forEach(([key, messages]) => {
    const name = key.charAt(0).toLowerCase() + key.slice(1)
    if (fields.includes(name)) setError(name as Path<T>, { type: 'server', message: messages[0] })
    else unmatched.push(...messages)
  })
  if (unmatched.length) toast.error(apiError.message, { description: unmatched.join(' ') })
  else if (!Object.keys(apiError.fields).length) toast.error(apiError.message)
}

export function toastError(error: unknown) {
  const apiError = ApiError.from(error)
  toast.error(apiError.message, apiError.details.length ? { description: apiError.details.join(' ') } : undefined)
}
