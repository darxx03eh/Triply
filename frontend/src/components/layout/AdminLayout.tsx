import { useState } from 'react'
import { NavLink, Outlet, useLocation } from 'react-router'
import { BadgePercent, BedDouble, Building2, ChevronsLeft, ChevronsRight, ExternalLink, LayoutDashboard, MapPin, Menu, Sparkles, X } from 'lucide-react'
import { Link } from 'react-router'
import { cn } from '@/lib/cn'
import { storage } from '@/lib/storage'
import { Logo } from './Logo'
import { UserMenu } from './UserMenu'

const links = [
  { to: '/admin', label: 'Dashboard', icon: LayoutDashboard, end: true },
  { to: '/admin/cities', label: 'Cities', icon: MapPin },
  { to: '/admin/hotels', label: 'Hotels', icon: Building2 },
  { to: '/admin/rooms', label: 'Rooms', icon: BedDouble },
  { to: '/admin/amenities', label: 'Amenities', icon: Sparkles },
  { to: '/admin/deals', label: 'Deals', icon: BadgePercent },
]

export function AdminLayout() {
  const [collapsed, setCollapsed] = useState(() => storage.get('triply.admin.collapsed', false))
  const [mobileOpen, setMobileOpen] = useState(false)
  const location = useLocation()
  const current = links.find((l) => (l.end ? location.pathname === l.to : location.pathname.startsWith(l.to)))

  const toggle = () => {
    setCollapsed((value) => {
      storage.set('triply.admin.collapsed', !value)
      return !value
    })
  }

  const nav = (compact: boolean) => (
    <nav className="space-y-1 px-3">
      {links.map(({ to, label, icon: Icon, end }) => (
        <NavLink
          key={to}
          to={to}
          end={end}
          onClick={() => setMobileOpen(false)}
          title={compact ? label : undefined}
          className={({ isActive }) =>
            cn(
              'group flex items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-semibold transition',
              compact && 'justify-center px-0',
              isActive ? 'bg-white/10 text-white' : 'text-slate-400 hover:bg-white/5 hover:text-white',
            )
          }
        >
          {({ isActive }) => (
            <>
              <Icon className={cn('size-5 shrink-0', isActive && 'text-accent-400')} />
              {!compact && label}
            </>
          )}
        </NavLink>
      ))}
    </nav>
  )

  return (
    <div className="flex min-h-screen bg-slate-50">
      <aside
        className={cn(
          'sticky top-0 hidden h-screen shrink-0 flex-col bg-slate-950 py-5 transition-[width] duration-300 lg:flex',
          collapsed ? 'w-20' : 'w-64',
        )}
      >
        <div className={cn('mb-8 flex items-center px-5', collapsed && 'justify-center px-0')}>
          {collapsed ? (
            <span className="flex size-9 items-center justify-center rounded-xl bg-gradient-to-br from-brand-500 to-accent-500 font-extrabold text-white">T</span>
          ) : (
            <Logo light />
          )}
        </div>
        {!collapsed && <p className="mb-2 px-6 text-[11px] font-bold tracking-widest text-slate-500 uppercase">Manage</p>}
        {nav(collapsed)}
        <div className="mt-auto space-y-1 px-3">
          <Link
            to="/"
            className={cn('flex items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-semibold text-slate-400 hover:bg-white/5 hover:text-white', collapsed && 'justify-center px-0')}
          >
            <ExternalLink className="size-5" /> {!collapsed && 'View website'}
          </Link>
          <button
            onClick={toggle}
            className={cn('flex w-full cursor-pointer items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-semibold text-slate-400 hover:bg-white/5 hover:text-white', collapsed && 'justify-center px-0')}
          >
            {collapsed ? <ChevronsRight className="size-5" /> : <><ChevronsLeft className="size-5" /> Collapse</>}
          </button>
        </div>
      </aside>

      {mobileOpen && (
        <div className="fixed inset-0 z-50 lg:hidden">
          <div className="absolute inset-0 bg-slate-950/50" onClick={() => setMobileOpen(false)} />
          <aside className="relative flex h-full w-72 animate-fade-in flex-col bg-slate-950 py-5">
            <div className="mb-8 flex items-center justify-between px-5">
              <Logo light />
              <button onClick={() => setMobileOpen(false)} className="text-slate-400"><X className="size-5" /></button>
            </div>
            {nav(false)}
          </aside>
        </div>
      )}

      <div className="flex min-w-0 flex-1 flex-col">
        <header className="sticky top-0 z-30 flex h-16 items-center justify-between gap-4 border-b border-slate-200/70 bg-white/85 px-4 backdrop-blur-xl sm:px-8">
          <div className="flex items-center gap-3">
            <button onClick={() => setMobileOpen(true)} className="rounded-lg p-2 text-slate-600 hover:bg-slate-100 lg:hidden">
              <Menu className="size-5" />
            </button>
            <div>
              <p className="text-xs font-medium text-slate-400">Admin</p>
              <h1 className="text-base font-bold text-slate-900">{current?.label ?? 'Dashboard'}</h1>
            </div>
          </div>
          <UserMenu />
        </header>
        <main className="flex-1 px-4 py-6 sm:px-8 sm:py-8">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
