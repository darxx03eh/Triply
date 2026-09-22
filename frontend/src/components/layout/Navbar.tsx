import { useEffect, useState } from 'react'
import { Link, NavLink, useLocation } from 'react-router'
import { ShoppingBag } from 'lucide-react'
import { useAuth } from '@/features/auth/useAuth'
import { useCart } from '@/features/cart/useCart'
import { cn } from '@/lib/cn'
import { Button } from '../ui/Button'
import { Logo } from './Logo'
import { UserMenu } from './UserMenu'

export function Navbar() {
  const { isAuthenticated } = useAuth()
  const { count } = useCart()
  const location = useLocation()
  const isHome = location.pathname === '/'
  const [scrolled, setScrolled] = useState(false)

  useEffect(() => {
    const onScroll = () => setScrolled(window.scrollY > 24)
    onScroll()
    window.addEventListener('scroll', onScroll, { passive: true })
    return () => window.removeEventListener('scroll', onScroll)
  }, [])

  const transparent = isHome && !scrolled

  return (
    <header
      className={cn(
        'no-print fixed inset-x-0 top-0 z-40 transition-all duration-300',
        transparent ? 'bg-transparent' : 'border-b border-slate-200/70 bg-white/85 backdrop-blur-xl',
      )}
    >
      <div className="container-page flex h-16 items-center justify-between gap-4">
        <Logo light={transparent} />

        <nav className="hidden items-center gap-1 md:flex">
          {[
            { to: '/', label: 'Home' },
            { to: '/search', label: 'Explore hotels' },
          ].map((link) => (
            <NavLink
              key={link.to}
              to={link.to}
              end
              className={({ isActive }) =>
                cn(
                  'rounded-xl px-3.5 py-2 text-sm font-semibold transition',
                  transparent
                    ? isActive ? 'bg-white/15 text-white' : 'text-white/80 hover:text-white'
                    : isActive ? 'bg-brand-50 text-brand-700' : 'text-slate-600 hover:text-slate-900',
                )
              }
            >
              {link.label}
            </NavLink>
          ))}
        </nav>

        <div className="flex items-center gap-2">
          <Link
            to="/checkout"
            className={cn(
              'relative flex size-10 items-center justify-center rounded-full transition',
              transparent ? 'text-white hover:bg-white/10' : 'text-slate-600 hover:bg-slate-100',
            )}
            aria-label="Cart"
          >
            <ShoppingBag className="size-5" />
            {count > 0 && (
              <span className="absolute -top-0.5 -right-0.5 flex size-5 items-center justify-center rounded-full bg-accent-500 text-[10px] font-bold text-white ring-2 ring-white">
                {count}
              </span>
            )}
          </Link>
          {isAuthenticated ? (
            <UserMenu light={transparent} />
          ) : (
            <>
              <Link to="/login">
                <Button variant="ghost" size="sm" className={cn('h-9', transparent && 'text-white hover:bg-white/10 hover:text-white')}>
                  Sign in
                </Button>
              </Link>
              <Link to="/register" className="hidden sm:block">
                <Button variant={transparent ? 'accent' : 'primary'} size="sm" className="h-9">
                  Create account
                </Button>
              </Link>
            </>
          )}
        </div>
      </div>
    </header>
  )
}
