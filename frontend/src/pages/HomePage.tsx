import { Link } from 'react-router'
import { useQuery } from '@tanstack/react-query'
import { ArrowRight, BadgePercent, Clock, Headphones, ShieldCheck, Sparkles } from 'lucide-react'
import { DealCard, DestinationCard, RecentHotelCard } from '@/components/hotel/HotelCards'
import { SearchBar } from '@/components/search/SearchBar'
import { Button } from '@/components/ui/Button'
import { SectionHeader } from '@/components/ui/Card'
import { EmptyState, Skeleton } from '@/components/ui/Feedback'
import { stockImages } from '@/mocks/images'
import { useAuth } from '@/features/auth/useAuth'
import { homeService } from '@/services/home.service'

export function HomePage() {
  const deals = useQuery({ queryKey: ['home', 'deals'], queryFn: homeService.getFeaturedDeals })
  const trending = useQuery({ queryKey: ['home', 'trending'], queryFn: homeService.getTrendingDestinations })
  const { user } = useAuth()
  const recent = useQuery({
    queryKey: ['home', 'recent', user?.id],
    queryFn: homeService.getRecentlyVisited,
    enabled: !!user,
    staleTime: 0,
  })

  return (
    <>
      <section className="relative isolate overflow-hidden bg-hero pt-32 pb-28 text-white sm:pt-40 sm:pb-36">
        <img src={stockImages.hero} alt="" className="absolute inset-0 -z-10 size-full object-cover opacity-25 mix-blend-luminosity" />
        <div className="absolute inset-x-0 bottom-0 -z-10 h-40 bg-gradient-to-t from-slate-50 to-transparent" />
        <div className="container-page">
          <div className="mx-auto max-w-3xl text-center">
            <span className="inline-flex items-center gap-2 rounded-full border border-white/15 bg-white/10 px-4 py-1.5 text-sm font-medium text-white/90 backdrop-blur">
              <Sparkles className="size-4 text-accent-400" /> Over 20 handpicked hotels across 10 cities
            </span>
            <h1 className="mt-6 text-4xl leading-[1.08] font-extrabold tracking-tight sm:text-6xl">
              Stay somewhere{' '}
              <span className="bg-gradient-to-r from-brand-300 via-white to-accent-400 bg-clip-text text-transparent">
                unforgettable
              </span>
            </h1>
            <p className="mx-auto mt-5 max-w-xl text-lg text-white/75">
              From boutique rooms in old cities to beachfront resorts — find the perfect place and book it in minutes.
            </p>
          </div>
          <div className="mx-auto mt-10 max-w-5xl">
            <SearchBar />
          </div>
          <div className="mx-auto mt-10 flex max-w-3xl flex-wrap items-center justify-center gap-x-8 gap-y-3 text-sm text-white/70">
            <span className="flex items-center gap-2"><ShieldCheck className="size-4 text-emerald-400" /> Secure payments</span>
            <span className="flex items-center gap-2"><Clock className="size-4 text-sky-300" /> Instant confirmation</span>
            <span className="flex items-center gap-2"><Headphones className="size-4 text-accent-400" /> 24/7 support</span>
          </div>
        </div>
      </section>

      <section className="container-page -mt-6 py-12">
        <SectionHeader
          eyebrow="Limited time"
          title="Featured Deals"
          description="Special offers from our partner hotels — book now before they are gone."
          action={
            <Link to="/search" className="inline-flex items-center gap-1.5 text-sm font-semibold text-brand-600 hover:text-brand-700">
              View all hotels <ArrowRight className="size-4" />
            </Link>
          }
        />
        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-5">
          {deals.isLoading
            ? Array.from({ length: 5 }, (_, i) => <Skeleton key={i} className="h-80 rounded-3xl" />)
            : deals.data?.map((deal) => <DealCard key={deal.dealId} deal={deal} />)}
        </div>
        {deals.data?.length === 0 && (
          <EmptyState icon={<BadgePercent className="size-7" />} title="No deals right now" description="New offers are added every week — check back soon." />
        )}
      </section>

      {user && (recent.data?.length ?? 0) > 0 && (
        <section className="container-page py-12">
          <SectionHeader eyebrow="Welcome back" title="Recently visited" description="Pick up where you left off." />
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-5">
            {recent.data!.map((hotel) => (
              <RecentHotelCard key={hotel.hotelId} hotel={hotel} />
            ))}
          </div>
        </section>
      )}

      <section className="container-page py-12">
        <SectionHeader
          eyebrow="Where everyone is going"
          title="Trending destinations"
          description="The most visited cities on Triply in the last 30 days."
        />
        {trending.isLoading ? (
          <div className="grid gap-5 md:grid-cols-6">
            {Array.from({ length: 5 }, (_, i) => <Skeleton key={i} className={i < 2 ? 'h-80 md:col-span-3' : 'h-64 md:col-span-2'} />)}
          </div>
        ) : (
          <div className="grid gap-5 md:grid-cols-6">
            {trending.data?.map((city, index) => (
              <DestinationCard
                key={city.cityId}
                city={city}
                index={index}
                className={index < 2 ? 'h-80 md:col-span-3' : 'h-64 md:col-span-2'}
              />
            ))}
          </div>
        )}
      </section>

      <section className="container-page py-12">
        <div className="relative overflow-hidden rounded-[2rem] bg-slate-950 px-6 py-14 text-white sm:px-14">
          <div className="absolute -top-24 -right-24 size-80 rounded-full bg-brand-600/40 blur-3xl" />
          <div className="absolute -bottom-24 -left-10 size-72 rounded-full bg-accent-500/30 blur-3xl" />
          <div className="relative grid items-center gap-10 lg:grid-cols-2">
            <div>
              <BadgePercent className="size-10 text-accent-400" />
              <h2 className="mt-4 text-3xl font-extrabold tracking-tight sm:text-4xl">Members save up to 25% on every stay</h2>
              <p className="mt-3 max-w-md text-white/70">
                Create a free account to unlock member prices, keep track of your trips and check out faster.
              </p>
              <Link to="/register">
                <Button variant="accent" size="lg" className="mt-7">
                  Join Triply — it’s free <ArrowRight className="size-5" />
                </Button>
              </Link>
            </div>
            <div className="grid grid-cols-2 gap-4">
              {[
                ['10+', 'Destinations'],
                ['20+', 'Partner hotels'],
                ['120+', 'Rooms to book'],
                ['4.7', 'Average rating'],
              ].map(([value, label]) => (
                <div key={label} className="rounded-2xl border border-white/10 bg-white/5 p-5 backdrop-blur">
                  <p className="text-3xl font-extrabold">{value}</p>
                  <p className="mt-1 text-sm text-white/60">{label}</p>
                </div>
              ))}
            </div>
          </div>
        </div>
      </section>
    </>
  )
}
