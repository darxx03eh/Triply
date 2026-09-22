import { Outlet, useLocation } from 'react-router'
import { useEffect } from 'react'
import { Footer } from './Footer'
import { Navbar } from './Navbar'

export function PublicLayout() {
  const { pathname } = useLocation()
  useEffect(() => {
    window.scrollTo({ top: 0 })
  }, [pathname])

  return (
    <div className="flex min-h-screen flex-col">
      <Navbar />
      <main className="flex-1">
        <Outlet />
      </main>
      <Footer />
    </div>
  )
}
