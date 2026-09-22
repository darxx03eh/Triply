import { Link } from 'react-router'
import { Logo } from './Logo'

export function Footer() {
  return (
    <footer className="no-print mt-24 border-t border-slate-200 bg-white">
      <div className="container-page grid gap-10 py-14 md:grid-cols-4">
        <div className="md:col-span-2">
          <Logo />
          <p className="mt-4 max-w-sm text-sm leading-relaxed text-slate-500">
            Discover handpicked hotels, from boutique stays in old cities to beachfront resorts. Book in minutes, travel with confidence.
          </p>
        </div>
        <div>
          <h4 className="text-sm font-bold text-slate-900">Explore</h4>
          <ul className="mt-4 space-y-2.5 text-sm text-slate-500">
            <li><Link to="/search" className="hover:text-brand-600">All hotels</Link></li>
            <li><Link to="/search?types=Luxury" className="hover:text-brand-600">Luxury stays</Link></li>
            <li><Link to="/search?types=Boutique" className="hover:text-brand-600">Boutique hotels</Link></li>
            <li><Link to="/search?types=Budget" className="hover:text-brand-600">Budget friendly</Link></li>
          </ul>
        </div>
        <div>
          <h4 className="text-sm font-bold text-slate-900">Account</h4>
          <ul className="mt-4 space-y-2.5 text-sm text-slate-500">
            <li><Link to="/login" className="hover:text-brand-600">Sign in</Link></li>
            <li><Link to="/register" className="hover:text-brand-600">Create account</Link></li>
            <li><Link to="/checkout" className="hover:text-brand-600">My cart</Link></li>
          </ul>
        </div>
      </div>
      <div className="border-t border-slate-100 py-6 text-center text-xs text-slate-400">
        © {new Date().getFullYear()} Triply. All rights reserved.
      </div>
    </footer>
  )
}
