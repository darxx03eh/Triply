/** Simulates network latency so loading states look the same as with the real API. */
export const delay = <T>(value: T, ms = 350) => new Promise<T>((resolve) => setTimeout(() => resolve(value), ms))

/** Stable pseudo-random numbers from a string, so mock data does not change between renders. */
export function seeded(key: string) {
  let hash = 2166136261
  for (let i = 0; i < key.length; i++) hash = Math.imul(hash ^ key.charCodeAt(i), 16777619)
  return () => {
    hash = Math.imul(hash ^ (hash >>> 15), 2246822507)
    hash = Math.imul(hash ^ (hash >>> 13), 3266489909)
    return ((hash ^= hash >>> 16) >>> 0) / 4294967296
  }
}
