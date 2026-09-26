/**
 * Builds Sieve query strings (filters / sorts / page / pageSize) used by every paged backend endpoint.
 * Filter syntax: `Name@=value` (contains, case-insensitive with `@=*`), `StarRating>=4`, `CityId==guid`.
 */
export interface SieveQuery {
  filters?: (string | false | null | undefined)[]
  sorts?: string
  page?: number
  pageSize?: number
  checkIn?: string
  checkOut?: string
}

const escape = (value: string) => value.replace(/([,|\\])/g, '\\$1')

export const sieve = {
  contains: (field: string, value?: string) => (value?.trim() ? `${field}@=*${escape(value.trim())}` : null),
  equals: (field: string, value?: string | number | boolean | null) =>
    value === undefined || value === null || value === '' ? null : `${field}==${escape(String(value))}`,
  gte: (field: string, value?: number | null) => (value || value === 0 ? `${field}>=${value}` : null),
  lte: (field: string, value?: number | null) => (value || value === 0 ? `${field}<=${value}` : null),
  /** `field==a|b|c` — Sieve treats `|` in the value as OR. */
  oneOf: (field: string, values?: (string | number)[]) =>
    values?.length ? `${field}==${values.map((v) => escape(String(v))).join('|')}` : null,
}

export function toSieveParams({ filters, sorts, page, pageSize, checkIn, checkOut }: SieveQuery) {
  const params: Record<string, string | number> = {}
  const active = (filters ?? []).filter(Boolean) as string[]
  if (active.length) params.filters = active.join(',')
  if (sorts) params.sorts = sorts
  if (page) params.page = page
  if (pageSize) params.pageSize = pageSize
  if (checkIn) params.checkIn = checkIn
  if (checkOut) params.checkOut = checkOut
  return params
}
