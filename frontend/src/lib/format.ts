const currency = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', maximumFractionDigits: 0 })
const currencyExact = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' })
const date = new Intl.DateTimeFormat('en-GB', { day: '2-digit', month: 'short', year: 'numeric' })
const dateTime = new Intl.DateTimeFormat('en-GB', {
  day: '2-digit',
  month: 'short',
  year: 'numeric',
  hour: '2-digit',
  minute: '2-digit',
})

export const formatMoney = (value: number, exact = false) => (exact ? currencyExact : currency).format(value)

export const formatDate = (value?: string | Date | null) => (value ? date.format(new Date(value)) : '—')

export const formatDateTime = (value?: string | Date | null) => (value ? dateTime.format(new Date(value)) : '—')

/** yyyy-MM-dd in local time, the format used by <input type="date"> and the search URL. */
export const toIsoDate = (value: Date) => {
  const offset = value.getTimezoneOffset() * 60_000
  return new Date(value.getTime() - offset).toISOString().slice(0, 10)
}

export const addDays = (value: Date, days: number) => {
  const next = new Date(value)
  next.setDate(next.getDate() + days)
  return next
}

export const nightsBetween = (checkIn: string, checkOut: string) => {
  const diff = new Date(checkOut).getTime() - new Date(checkIn).getTime()
  return Math.max(1, Math.round(diff / 86_400_000))
}

export const initials = (value: string) =>
  value
    .split(/\s+/)
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part[0]?.toUpperCase())
    .join('')
