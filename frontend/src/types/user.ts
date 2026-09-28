export interface User {
  userId: string
  firstName: string
  lastName: string
  userName: string | null
  email: string | null
  phoneNumber: string | null
  emailConfirmed: boolean
  roles: string[]
  isActive: boolean
  isDeleted: boolean
  createdAt: string
  lastLoginAt: string | null
}
