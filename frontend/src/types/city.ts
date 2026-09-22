export interface City {
  cityId: string
  name: string
  country: string
  postOffice: string | null
  hotelsCount: number
  thumbnailUrl: string | null
  isDeleted: boolean
  createdAt: string
  modifiedAt: string | null
  rowVersion: string
}

export interface CityInput {
  name: string
  country: string
  postOffice?: string | null
}

export interface UpdateCityInput extends CityInput {
  rowVersion: string
}

/** Triply.Application/DTOs/Home/TrendingCityResponse.cs */
export interface TrendingCity {
  cityId: string
  name: string
  country: string
  thumbnailUrl: string | null
  visitsCount: number
  hotelsCount: number
}
