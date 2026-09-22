export interface LoginRequest {
  identifier: string
  password: string
}

export interface LoginResponse {
  name: string
  access: string
}

export interface RegisterRequest {
  firstName: string
  lastName: string
  email: string
  username: string
  password: string
  confirmPassword: string
  phoneNumber: string
  dateOfBirth?: string | null
}

export interface RegisterResponse {
  name: string
  id: string
  email: string
  username: string
}

export interface CurrentUser {
  id: string
  name: string
  username: string
  email: string
  phoneNumber: string
  roles: string[]
  isAdmin: boolean
}
