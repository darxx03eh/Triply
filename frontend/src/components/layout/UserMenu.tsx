import { useEffect, useRef, useState } from 'react'
import { Link, useNavigate } from 'react-router'
import { ChevronDown, LayoutDashboard, LogOut, ReceiptText, ShoppingBag } from 'lucide-react'
import { toast } from 'sonner'
import { useAuth } from '@/features/auth/useAuth'
import { cn } from '@/lib/cn'
import { initials } from '@/lib/format'

export function UserMenu({ light }: { light?: boolean }) {
  const { user, logout } = useAuth()
  const navigate = useNavigate()
  const [open, setOpen] = useState(false)
  const ref = useRef<HTMLDivElement>(null)

  useEffect(() => {
    const close = (event: MouseEvent) => ref.current && !ref.current.contains(event.target as Node) && setOpen(false)
    document.addEventListener('mousedown', close)
    return () => document.removeEventListener('mousedown', close)
  }, [])

  if (!user) return null

  const handleLogout = async () => {
    setOpen(false)
    await logout()
    toast.success('You have been signed out')
    navigate('/')
  }

  return (
    <div ref={ref} className="relative">
      <button
        onClick={() => setOpen((v) => !v)}
        className={cn(
          'flex cursor-pointer items-center gap-2 rounded-full py-1 pr-2.5 pl-1 transition',
          light ? 'hover:bg-white/10' : 'hover:bg-slate-100',
        )}
      >
        <span className="flex size-8 items-center justify-center rounded-full bg-gradient-to-br from-brand-500 to-accent-500 text-xs font-bold text-white">
          {initials(user.name)}
        </span>
        <span className={cn('hidden text-sm font-semibold sm:block', light ? 'text-white' : 'text-slate-700')}>
          {user.name.split(' ')[0]}
        </span>
        <ChevronDown className={cn('size-4', light ? 'text-white/70' : 'text-slate-400')} />
      </button>
      {open && (
        <div className="absolute right-0 z-40 mt-2 w-60 animate-scale-in overflow-hidden rounded-2xl border border-slate-100 bg-white shadow-lift">
          <div className="border-b border-slate-100 px-4 py-3">
            <p className="truncate text-sm font-semibold text-slate-900">{user.name}</p>
            <p className="truncate text-xs text-slate-500">{user.email}</p>
          </div>
          <div className="p-1.5">
            {user.isAdmin && (
              <MenuLink to="/admin" icon={<LayoutDashboard className="size-4" />} onClick={() => setOpen(false)}>
                Admin dashboard
              </MenuLink>
            )}
            <MenuLink to="/checkout" icon={<ShoppingBag className="size-4" />} onClick={() => setOpen(false)}>
              My cart
            </MenuLink>
            <MenuLink to="/bookings" icon={<ReceiptText className="size-4" />} onClick={() => setOpen(false)}>
              My bookings
            </MenuLink>
            <button
              onClick={handleLogout}
              className="flex w-full cursor-pointer items-center gap-2.5 rounded-xl px-3 py-2 text-sm font-medium text-red-600 hover:bg-red-50"
            >
              <LogOut className="size-4" /> Sign out
            </button>
          </div>
        </div>
      )}
    </div>
  )
}

function MenuLink({ to, icon, children, onClick }: { to: string; icon: React.ReactNode; children: React.ReactNode; onClick: () => void }) {
  return (
    <Link to={to} onClick={onClick} className="flex items-center gap-2.5 rounded-xl px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50">
      {icon}
      {children}
    </Link>
  )
}
