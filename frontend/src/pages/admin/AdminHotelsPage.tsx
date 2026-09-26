import { useEffect, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { AlertCircle, Clock, Landmark, Loader2, Plus, Search, Trash2 } from 'lucide-react'
import { toast } from 'sonner'
import { amenitiesApi } from '@/api/amenities.api'
import { attractionsApi } from '@/api/attractions.api'
import { hotelsApi } from '@/api/hotels.api'
import { AdminPageHeader, AdminTableCard, DateCell } from '@/components/admin/AdminPage'
import { DataTable, type Column } from '@/components/admin/DataTable'
import { ImageUploadBox } from '@/components/admin/ImageUploadBox'
import { RowActions } from '@/components/admin/RowActions'
import { Button } from '@/components/ui/Button'
import { Checkbox, Field, Input, Select, Textarea } from '@/components/ui/Field'
import { Badge, PageLoader } from '@/components/ui/Feedback'
import { ConfirmDialog, Drawer, Modal } from '@/components/ui/Overlay'
import { Pagination } from '@/components/ui/Pagination'
import { SmartImage } from '@/components/ui/SmartImage'
import { Stars } from '@/components/ui/Stars'
import { Tabs } from '@/components/ui/Tabs'
import { useDebounce } from '@/hooks/useDebounce'
import { useAmenityOptions, useCityOptions } from '@/hooks/useLookups'
import { cn } from '@/lib/cn'
import { applyServerErrors, toastError } from '@/lib/forms'
import { sieve } from '@/lib/sieve'
import { HOTEL_TYPES, type Attraction, type HotelDetails, type HotelSummary } from '@/types/hotel'

const PAGE_SIZE = 10

const optionalNumber = (min: number, max: number, label: string) =>
  z.preprocess(
    (v) => (v === '' || v === null || v === undefined || Number.isNaN(v) ? null : Number(v)),
    z.number().min(min, `${label} must be between ${min} and ${max}`).max(max, `${label} must be between ${min} and ${max}`).nullable(),
  )

const schema = z.object({
  name: z.string().trim().min(1, 'Name is required').max(150),
  cityId: z.string().min(1, 'Choose a city'),
  starRating: z.coerce.number().int().min(1).max(5),
  hotelType: z.enum(HOTEL_TYPES),
  address: z.string().trim().min(1, 'Address is required').max(300),
  description: z.string().max(2000).optional(),
  latitude: optionalNumber(-90, 90, 'Latitude'),
  longitude: optionalNumber(-180, 180, 'Longitude'),
})
type Values = z.input<typeof schema>
type Parsed = z.output<typeof schema>

const fieldNames = ['name', 'cityId', 'starRating', 'hotelType', 'address', 'description', 'latitude', 'longitude']
const typeTone = { Luxury: 'accent', Boutique: 'brand', Budget: 'green' } as const

export function AdminHotelsPage() {
  const queryClient = useQueryClient()
  const cities = useCityOptions()
  const [search, setSearch] = useState('')
  const [cityId, setCityId] = useState('')
  const [type, setType] = useState('')
  const [stars, setStars] = useState('')
  const [page, setPage] = useState(1)
  const [creating, setCreating] = useState(false)
  const [editing, setEditing] = useState<HotelSummary | null>(null)
  const [deleting, setDeleting] = useState<HotelSummary | null>(null)
  const term = useDebounce(search)

  useEffect(() => {
    setPage(1)
  }, [term, cityId, type, stars])

  const hotels = useQuery({
    queryKey: ['admin', 'hotels', term, cityId, type, stars, page],
    queryFn: () =>
      hotelsApi.list({
        filters: [sieve.contains('Name', term), sieve.equals('CityId', cityId), sieve.equals('type', type), sieve.equals('StarRating', stars)],
        sorts: '-CreatedAt',
        page,
        pageSize: PAGE_SIZE,
      }),
    placeholderData: (previous) => previous,
  })

  const refresh = () => {
    queryClient.invalidateQueries({ queryKey: ['admin', 'hotels'] })
    queryClient.invalidateQueries({ queryKey: ['lookup', 'hotels'] })
  }

  const remove = useMutation({
    mutationFn: (id: string) => hotelsApi.remove(id),
    onSuccess: () => {
      toast.success('Hotel deleted')
      setDeleting(null)
      refresh()
    },
    onError: toastError,
  })

  const columns: Column<HotelSummary>[] = [
    {
      key: 'name',
      header: 'Hotel',
      cell: (h) => (
        <div className="flex items-center gap-3">
          <SmartImage src={h.thumbnailUrl} alt={h.name} seed={h.hotelId} className="size-11 shrink-0 rounded-xl" />
          <div className="min-w-0">
            <p className="max-w-56 truncate font-semibold text-slate-900">{h.name}</p>
            <p className="text-xs text-slate-500">{h.cityName}</p>
          </div>
        </div>
      ),
    },
    { key: 'stars', header: 'Stars', cell: (h) => <Stars value={h.starRating} /> },
    { key: 'type', header: 'Type', cell: (h) => <Badge tone={typeTone[h.hotelType]}>{h.hotelType}</Badge> },
    { key: 'owner', header: 'Owner', cell: (h) => h.ownerName ?? <span className="text-slate-300">—</span> },
    { key: 'rooms', header: 'Rooms', cell: (h) => <Badge tone="brand">{h.roomsCount}</Badge> },
    { key: 'created', header: 'Created', cell: (h) => <DateCell value={h.createdAt} /> },
    { key: 'modified', header: 'Modified', cell: (h) => <DateCell value={h.modifiedAt} /> },
    { key: 'actions', header: '', className: 'text-right', cell: (h) => <RowActions onEdit={() => setEditing(h)} onDelete={() => setDeleting(h)} /> },
  ]

  return (
    <>
      <AdminPageHeader
        title="Hotels"
        description="Properties listed on Triply. Click a row to edit details, images and amenities."
        actions={<Button onClick={() => setCreating(true)}><Plus className="size-4" /> New hotel</Button>}
      />
      <AdminTableCard
        toolbar={
          <>
            <div className="w-full lg:max-w-xs">
              <Input icon={<Search className="size-4" />} placeholder="Search by name…" value={search} onChange={(e) => setSearch(e.target.value)} />
            </div>
            <div className="grid flex-1 grid-cols-1 gap-3 sm:grid-cols-3 lg:max-w-xl">
              <Select value={cityId} onChange={(e) => setCityId(e.target.value)}>
                <option value="">All cities</option>
                {cities.data?.map((c) => <option key={c.cityId} value={c.cityId}>{c.name}</option>)}
              </Select>
              <Select value={type} onChange={(e) => setType(e.target.value)}>
                <option value="">All types</option>
                {HOTEL_TYPES.map((t) => <option key={t} value={t}>{t}</option>)}
              </Select>
              <Select value={stars} onChange={(e) => setStars(e.target.value)}>
                <option value="">Any rating</option>
                {[5, 4, 3, 2, 1].map((s) => <option key={s} value={s}>{s} stars</option>)}
              </Select>
            </div>
          </>
        }
      >
        <DataTable
          columns={columns}
          rows={hotels.data?.items}
          rowKey={(h) => h.hotelId}
          loading={hotels.isLoading}
          error={hotels.error}
          onRetry={() => hotels.refetch()}
          onRowClick={setEditing}
          emptyTitle="No hotels found"
        />
        {hotels.data && (
          <Pagination page={page} totalPages={hotels.data.totalPages} totalCount={hotels.data.totalCount} pageSize={PAGE_SIZE} onChange={setPage} />
        )}
      </AdminTableCard>

      <HotelFormModal open={creating} onClose={() => setCreating(false)} onSaved={refresh} />
      {editing && <HotelDrawer hotelId={editing.hotelId} name={editing.name} onClose={() => setEditing(null)} onSaved={refresh} />}
      <ConfirmDialog
        open={!!deleting}
        onClose={() => setDeleting(null)}
        onConfirm={() => deleting && remove.mutate(deleting.hotelId)}
        loading={remove.isPending}
        title="Delete hotel?"
        description={<>“{deleting?.name}” and its rooms will no longer be visible to guests.</>}
      />
    </>
  )
}

function HotelFields({ form }: { form: ReturnType<typeof useForm<Values, unknown, Parsed>> }) {
  const cities = useCityOptions()
  const { register, formState: { errors } } = form
  return (
    <div className="grid gap-4 sm:grid-cols-2">
      <Field label="Name" required className="sm:col-span-2" error={errors.name?.message}><Input {...register('name')} invalid={!!errors.name} /></Field>
      <Field label="City" required error={errors.cityId?.message}>
        <Select {...register('cityId')} invalid={!!errors.cityId}>
          <option value="">Select a city</option>
          {cities.data?.map((c) => <option key={c.cityId} value={c.cityId}>{c.name}, {c.country}</option>)}
        </Select>
      </Field>
      <Field label="Hotel type" required error={errors.hotelType?.message}>
        <Select {...register('hotelType')}>{HOTEL_TYPES.map((t) => <option key={t} value={t}>{t}</option>)}</Select>
      </Field>
      <Field label="Star rating" required error={errors.starRating?.message}>
        <Select {...register('starRating')}>{[5, 4, 3, 2, 1].map((s) => <option key={s} value={s}>{'★'.repeat(s)} ({s})</option>)}</Select>
      </Field>
      <Field label="Address" required error={errors.address?.message}><Input {...register('address')} invalid={!!errors.address} /></Field>
      <Field label="Latitude" error={errors.latitude?.message}><Input type="number" step="any" {...register('latitude')} invalid={!!errors.latitude} /></Field>
      <Field label="Longitude" error={errors.longitude?.message}><Input type="number" step="any" {...register('longitude')} invalid={!!errors.longitude} /></Field>
      <Field label="Description" className="sm:col-span-2" error={errors.description?.message}><Textarea rows={4} {...register('description')} /></Field>
    </div>
  )
}

function HotelFormModal({ open, onClose, onSaved }: { open: boolean; onClose: () => void; onSaved: () => void }) {
  const form = useForm<Values, unknown, Parsed>({ resolver: zodResolver(schema), defaultValues: { hotelType: 'Budget', starRating: 3 } })
  const create = useMutation({
    mutationFn: (values: Parsed) => hotelsApi.create({ ...values, description: values.description || null }),
    onSuccess: (hotel) => {
      toast.success(`${hotel.name} created`, { description: 'Open it to add images, amenities and rooms.' })
      form.reset()
      onSaved()
      onClose()
    },
    onError: (error) => applyServerErrors(error, form.setError, fieldNames),
  })

  return (
    <Modal
      open={open}
      onClose={onClose}
      className="max-w-2xl"
      title="New hotel"
      description="You’ll be recorded as the hotel owner."
      footer={
        <>
          <Button variant="outline" onClick={onClose}>Cancel</Button>
          <Button onClick={form.handleSubmit((v) => create.mutate(v))} loading={create.isPending}>Create hotel</Button>
        </>
      }
    >
      <HotelFields form={form} />
    </Modal>
  )
}

type DrawerTab = 'details' | 'images' | 'amenities' | 'attractions'

function HotelDrawer({ hotelId, name, onClose, onSaved }: { hotelId: string; name: string; onClose: () => void; onSaved: () => void }) {
  const [tab, setTab] = useState<DrawerTab>('details')
  const details = useQuery({ queryKey: ['admin', 'hotel', hotelId], queryFn: () => hotelsApi.get(hotelId) })

  return (
    <Drawer open onClose={onClose} title={name} description="Manage this hotel" className="max-w-2xl">
      <Tabs
        className="mb-6"
        value={tab}
        onChange={setTab}
        tabs={[
          { value: 'details', label: 'Details' },
          { value: 'images', label: 'Images' },
          { value: 'amenities', label: 'Amenities', count: details.data?.amenities.length },
          { value: 'attractions', label: 'Nearby' },
        ]}
      />
      {details.isLoading || !details.data ? (
        <PageLoader />
      ) : tab === 'details' ? (
        <HotelDetailsForm hotel={details.data} onSaved={onSaved} onClose={onClose} />
      ) : tab === 'images' ? (
        <HotelImagesManager hotelId={hotelId} onChanged={onSaved} />
      ) : tab === 'amenities' ? (
        <HotelAmenitiesManager hotel={details.data} />
      ) : (
        <HotelAttractionsManager hotelId={hotelId} />
      )}
    </Drawer>
  )
}

function HotelDetailsForm({ hotel, onSaved, onClose }: { hotel: HotelDetails; onSaved: () => void; onClose: () => void }) {
  const queryClient = useQueryClient()
  const form = useForm<Values, unknown, Parsed>({
    resolver: zodResolver(schema),
    defaultValues: {
      name: hotel.name,
      cityId: hotel.cityId,
      starRating: hotel.starRating,
      hotelType: hotel.hotelType,
      address: hotel.address ?? '',
      description: hotel.description ?? '',
      latitude: hotel.latitude ?? '',
      longitude: hotel.longitude ?? '',
    } as Values,
  })

  const update = useMutation({
    mutationFn: (values: Parsed) =>
      hotelsApi.update(hotel.hotelId, { ...values, description: values.description || null, ownerId: hotel.ownerId, rowVersion: hotel.rowVersion }),
    onSuccess: (saved) => {
      toast.success('Hotel updated')
      queryClient.setQueryData(['admin', 'hotel', hotel.hotelId], saved)
      onSaved()
      onClose()
    },
    onError: (error) => applyServerErrors(error, form.setError, fieldNames),
  })

  return (
    <form onSubmit={form.handleSubmit((v) => update.mutate(v))} className="space-y-6">
      <HotelFields form={form} />
      <div className="flex justify-end gap-2 border-t border-slate-100 pt-5">
        <Button type="button" variant="outline" onClick={onClose}>Cancel</Button>
        <Button type="submit" loading={update.isPending}>Save changes</Button>
      </div>
    </form>
  )
}

function HotelImagesManager({ hotelId, onChanged }: { hotelId: string; onChanged: () => void }) {
  const queryClient = useQueryClient()
  const [deletingImageId, setDeletingImageId] = useState<string | null>(null)
  const images = useQuery({
    queryKey: ['admin', 'hotel', hotelId, 'images'],
    queryFn: () => hotelsApi.images(hotelId),
    // Uploads are processed by the background worker — poll while any image is still pending.
    refetchInterval: (query) => (query.state.data?.some((i) => i.status === 'Pending') ? 2500 : false),
  })

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey: ['admin', 'hotel', hotelId] })
    onChanged()
  }

  const upload = useMutation({
    mutationFn: (file: File) => hotelsApi.uploadImage(hotelId, file),
    onSuccess: () => {
      toast.success('Image queued for upload')
      invalidate()
    },
    onError: toastError,
  })

  const remove = useMutation({
    mutationFn: (imageId: string) => hotelsApi.removeImage(hotelId, imageId),
    onSuccess: () => {
      toast.success('Image deleted')
      setDeletingImageId(null)
      invalidate()
    },
    onError: toastError,
  })

  return (
    <>
      <div className="space-y-5">
      <ImageUploadBox onFile={(file) => upload.mutate(file)} loading={upload.isPending} />
      <p className="text-xs text-slate-500">The first image (lowest order) is used as the hotel thumbnail in search results.</p>
      <div className="grid grid-cols-2 gap-3 sm:grid-cols-3">
        {images.data?.map((image) => (
          <div key={image.imageId} className="group relative aspect-[4/3] overflow-hidden rounded-2xl bg-slate-100">
            {image.url ? (
              <img src={image.url} alt="" className="size-full object-cover" />
            ) : (
              <div className="flex size-full flex-col items-center justify-center gap-2 text-slate-400">
                {image.status === 'Pending' ? <Loader2 className="size-6 animate-spin text-brand-500" /> : <AlertCircle className="size-6 text-red-500" />}
              </div>
            )}
            <div className="absolute top-2 left-2 flex gap-1.5">
              <Badge className="bg-white/95">#{image.displayOrder}</Badge>
              {image.status !== 'Uploaded' && (
                <Badge tone={image.status === 'Pending' ? 'amber' : 'red'} className="bg-white/95">
                  {image.status === 'Pending' && <Clock className="size-3" />} {image.status}
                </Badge>
              )}
            </div>
            <button
              onClick={() => setDeletingImageId(image.imageId)}
              className="absolute top-2 right-2 rounded-lg bg-white/95 p-1.5 text-red-600 opacity-0 shadow transition group-hover:opacity-100"
              aria-label="Delete image"
            >
              <Trash2 className="size-4" />
            </button>
          </div>
        ))}
      </div>
      {images.data?.length === 0 && <p className="text-center text-sm text-slate-400">No images yet.</p>}
      </div>
      <ConfirmDialog
        open={deletingImageId !== null}
        onClose={() => setDeletingImageId(null)}
        onConfirm={() => deletingImageId && remove.mutate(deletingImageId)}
        loading={remove.isPending}
        title="Delete image?"
        description="This hotel image will be permanently deleted."
      />
    </>
  )
}

function HotelAmenitiesManager({ hotel }: { hotel: HotelDetails }) {
  const queryClient = useQueryClient()
  const all = useAmenityOptions()
  const current = useQuery({ queryKey: ['admin', 'hotel', hotel.hotelId, 'amenities'], queryFn: () => amenitiesApi.byHotel(hotel.hotelId) })
  const [selected, setSelected] = useState<Set<string>>(new Set())

  useEffect(() => {
    if (current.data) setSelected(new Set(current.data.map((a) => a.amenityId)))
  }, [current.data])

  const save = useMutation({
    mutationFn: () => amenitiesApi.setForHotel(hotel.hotelId, [...selected]),
    onSuccess: () => {
      toast.success('Amenities saved')
      queryClient.invalidateQueries({ queryKey: ['admin', 'hotel', hotel.hotelId] })
    },
    onError: toastError,
  })

  const toggle = (id: string) =>
    setSelected((prev) => {
      const next = new Set(prev)
      if (next.has(id)) next.delete(id)
      else next.add(id)
      return next
    })

  if (all.isLoading || current.isLoading) return <PageLoader />

  return (
    <div className="space-y-6">
      <div className="grid gap-3 sm:grid-cols-2">
        {all.data?.map((amenity) => (
          <div key={amenity.amenityId} className="rounded-xl border border-slate-200 px-4 py-3 transition hover:border-brand-300">
            <Checkbox label={amenity.name} checked={selected.has(amenity.amenityId)} onChange={() => toggle(amenity.amenityId)} />
          </div>
        ))}
      </div>
      <div className="flex items-center justify-between border-t border-slate-100 pt-5">
        <p className="text-sm text-slate-500">{selected.size} selected</p>
        <Button onClick={() => save.mutate()} loading={save.isPending}>Save amenities</Button>
      </div>
    </div>
  )
}

const attractionSchema = z.object({
  name: z.string().trim().min(1, 'Name is required').max(150),
  category: z.string().trim().min(1, 'Category is required').max(50),
  distanceKm: z.coerce.number().min(0, 'Between 0 and 100 km').max(100, 'Between 0 and 100 km'),
})
type AttractionValues = z.input<typeof attractionSchema>
type AttractionParsed = z.output<typeof attractionSchema>
const attractionFields = ['name', 'category', 'distanceKm']
const attractionCategories = ['Landmark', 'Historical', 'Museum', 'Religious', 'Shopping', 'Nature', 'Beach', 'Food', 'Activity', 'Viewpoint', 'Culture']

function HotelAttractionsManager({ hotelId }: { hotelId: string }) {
  const queryClient = useQueryClient()
  const [editing, setEditing] = useState<Attraction | null>(null)
  const [deleting, setDeleting] = useState<Attraction | null>(null)
  const attractions = useQuery({ queryKey: ['hotel', hotelId, 'nearby'], queryFn: () => attractionsApi.byHotel(hotelId) })
  const empty = { name: '', category: 'Landmark', distanceKm: 1 }
  const form = useForm<AttractionValues, unknown, AttractionParsed>({ resolver: zodResolver(attractionSchema), defaultValues: empty })
  const { register, formState: { errors } } = form

  const refresh = () => queryClient.invalidateQueries({ queryKey: ['hotel', hotelId, 'nearby'] })

  const startEdit = (attraction: Attraction) => {
    setEditing(attraction)
    form.reset({ name: attraction.name, category: attraction.category, distanceKm: attraction.distanceKm })
  }

  const cancelEdit = () => {
    setEditing(null)
    form.reset(empty)
  }

  const save = useMutation({
    mutationFn: (values: AttractionParsed) =>
      editing ? attractionsApi.update(editing.attractionId, values) : attractionsApi.create(hotelId, values),
    onSuccess: (attraction) => {
      toast.success(editing ? 'Attraction updated' : `${attraction.name} added`)
      cancelEdit()
      refresh()
    },
    onError: (error) => applyServerErrors(error, form.setError, attractionFields),
  })

  const remove = useMutation({
    mutationFn: (id: string) => attractionsApi.remove(id),
    onSuccess: () => {
      toast.success('Attraction deleted')
      if (deleting?.attractionId === editing?.attractionId) cancelEdit()
      setDeleting(null)
      refresh()
    },
    onError: toastError,
  })

  return (
    <div className="space-y-6">
      <form onSubmit={form.handleSubmit((v) => save.mutate(v))} className="grid gap-3 rounded-2xl border border-slate-200 p-4 sm:grid-cols-[1fr_9rem_7rem]">
        <p className="text-sm font-bold text-slate-900 sm:col-span-3">{editing ? `Edit ${editing.name}` : 'Add a nearby attraction'}</p>
        <Field label="Name" required error={errors.name?.message}><Input placeholder="Old City" {...register('name')} invalid={!!errors.name} /></Field>
        <Field label="Category" required error={errors.category?.message}>
          <Input list="attraction-categories" {...register('category')} invalid={!!errors.category} />
          <datalist id="attraction-categories">{attractionCategories.map((c) => <option key={c} value={c} />)}</datalist>
        </Field>
        <Field label="Distance (km)" required error={errors.distanceKm?.message}>
          <Input type="number" step="0.01" min={0} max={100} {...register('distanceKm')} invalid={!!errors.distanceKm} />
        </Field>
        <div className="flex justify-end gap-2 sm:col-span-3">
          {editing && <Button type="button" variant="outline" size="sm" onClick={cancelEdit}>Cancel</Button>}
          <Button type="submit" size="sm" loading={save.isPending}>{editing ? 'Save changes' : <><Plus className="size-4" /> Add</>}</Button>
        </div>
      </form>

      {attractions.isLoading ? (
        <PageLoader />
      ) : attractions.data?.length ? (
        <ul className="divide-y divide-slate-100 rounded-2xl border border-slate-200">
          {attractions.data.map((attraction) => (
            <li key={attraction.attractionId} className={cn('flex items-center justify-between gap-3 px-4 py-3', editing?.attractionId === attraction.attractionId && 'bg-brand-50/50')}>
              <div className="flex min-w-0 items-center gap-3">
                <span className="flex size-9 shrink-0 items-center justify-center rounded-xl bg-brand-50 text-brand-600"><Landmark className="size-4" /></span>
                <div className="min-w-0">
                  <p className="truncate text-sm font-semibold text-slate-900">{attraction.name}</p>
                  <p className="text-xs text-slate-500">{attraction.category} · {attraction.distanceKm} km</p>
                </div>
              </div>
              <RowActions onEdit={() => startEdit(attraction)} onDelete={() => setDeleting(attraction)} />
            </li>
          ))}
        </ul>
      ) : (
        <p className="text-center text-sm text-slate-400">No nearby attractions yet.</p>
      )}

      <ConfirmDialog
        open={!!deleting}
        onClose={() => setDeleting(null)}
        onConfirm={() => deleting && remove.mutate(deleting.attractionId)}
        loading={remove.isPending}
        title="Delete attraction?"
        description={<>{deleting?.name} will be removed from this hotel’s page.</>}
      />
    </div>
  )
}
