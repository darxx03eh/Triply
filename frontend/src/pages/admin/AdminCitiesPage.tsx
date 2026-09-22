import { useEffect, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Globe2, MapPin, Plus, Search, Trash2 } from 'lucide-react'
import { toast } from 'sonner'
import { citiesApi } from '@/api/cities.api'
import { AdminPageHeader, AdminTableCard, DateCell } from '@/components/admin/AdminPage'
import { DataTable, type Column } from '@/components/admin/DataTable'
import { ImageUploadBox } from '@/components/admin/ImageUploadBox'
import { RowActions } from '@/components/admin/RowActions'
import { Button } from '@/components/ui/Button'
import { Field, Input } from '@/components/ui/Field'
import { Badge } from '@/components/ui/Feedback'
import { ConfirmDialog, Drawer, Modal } from '@/components/ui/Overlay'
import { Pagination } from '@/components/ui/Pagination'
import { SmartImage } from '@/components/ui/SmartImage'
import { useDebounce } from '@/hooks/useDebounce'
import { applyServerErrors, toastError } from '@/lib/forms'
import type { City } from '@/types/city'

const PAGE_SIZE = 10

const schema = z.object({
  name: z.string().trim().min(1, 'Name is required').max(100),
  country: z.string().trim().min(1, 'Country is required').max(100),
  postOffice: z.string().trim().max(20, 'Max 20 characters').optional(),
})
type Values = z.infer<typeof schema>

export function AdminCitiesPage() {
  const queryClient = useQueryClient()
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(1)
  const [creating, setCreating] = useState(false)
  const [editing, setEditing] = useState<City | null>(null)
  const [deleting, setDeleting] = useState<City | null>(null)
  const term = useDebounce(search)

  useEffect(() => {
    setPage(1)
  }, [term])

  const cities = useQuery({
    queryKey: ['admin', 'cities', term, page],
    queryFn: () => citiesApi.list({ filters: [term && `(Name|Country)@=*${term}`], sorts: 'Name', page, pageSize: PAGE_SIZE }),
    placeholderData: (previous) => previous,
  })

  const refresh = () => queryClient.invalidateQueries({ queryKey: ['admin', 'cities'] })

  const remove = useMutation({
    mutationFn: (id: string) => citiesApi.remove(id),
    onSuccess: () => {
      toast.success('City deleted')
      setDeleting(null)
      refresh()
    },
    onError: toastError,
  })

  const columns: Column<City>[] = [
    {
      key: 'name',
      header: 'City',
      cell: (c) => (
        <div className="flex items-center gap-3">
          <SmartImage src={c.thumbnailUrl} alt={c.name} seed={c.cityId} className="size-10 shrink-0 rounded-xl" />
          <div>
            <p className="font-semibold text-slate-900">{c.name}</p>
            {c.isDeleted && <Badge tone="red" className="mt-0.5">Deleted</Badge>}
          </div>
        </div>
      ),
    },
    { key: 'country', header: 'Country', cell: (c) => c.country },
    { key: 'post', header: 'Post office', cell: (c) => c.postOffice ?? <span className="text-slate-300">—</span> },
    { key: 'hotels', header: 'Hotels', cell: (c) => <Badge tone="brand">{c.hotelsCount}</Badge> },
    { key: 'created', header: 'Created', cell: (c) => <DateCell value={c.createdAt} /> },
    { key: 'modified', header: 'Modified', cell: (c) => <DateCell value={c.modifiedAt} /> },
    {
      key: 'actions',
      header: '',
      className: 'text-right',
      cell: (c) => <RowActions onEdit={() => setEditing(c)} onDelete={() => setDeleting(c)} disabled={c.isDeleted} />,
    },
  ]

  return (
    <>
      <AdminPageHeader
        title="Cities"
        description="Destinations available on Triply. Click a row to edit it."
        actions={<Button onClick={() => setCreating(true)}><Plus className="size-4" /> New city</Button>}
      />
      <AdminTableCard
        toolbar={
          <div className="w-full lg:max-w-sm">
            <Input icon={<Search className="size-4" />} placeholder="Search by name or country…" value={search} onChange={(e) => setSearch(e.target.value)} />
          </div>
        }
      >
        <DataTable
          columns={columns}
          rows={cities.data?.items}
          rowKey={(c) => c.cityId}
          loading={cities.isLoading}
          error={cities.error}
          onRetry={() => cities.refetch()}
          onRowClick={(c) => !c.isDeleted && setEditing(c)}
          rowClassName={(c) => (c.isDeleted ? 'opacity-50' : undefined)}
          emptyTitle="No cities found"
        />
        {cities.data && (
          <Pagination page={page} totalPages={cities.data.totalPages} totalCount={cities.data.totalCount} pageSize={PAGE_SIZE} onChange={setPage} />
        )}
      </AdminTableCard>

      <CityFormModal open={creating} onClose={() => setCreating(false)} onSaved={refresh} />
      {editing && <CityDrawer city={editing} onClose={() => setEditing(null)} onSaved={refresh} />}
      <ConfirmDialog
        open={!!deleting}
        onClose={() => setDeleting(null)}
        onConfirm={() => deleting && remove.mutate(deleting.cityId)}
        loading={remove.isPending}
        title="Delete city?"
        description={<>“{deleting?.name}” will be hidden from the website. This cannot be undone from the dashboard.</>}
      />
    </>
  )
}

function CityFields({ form }: { form: ReturnType<typeof useForm<Values>> }) {
  const { register, formState: { errors } } = form
  return (
    <div className="grid gap-4 sm:grid-cols-2">
      <Field label="Name" required error={errors.name?.message}>
        <Input icon={<MapPin className="size-4" />} {...register('name')} invalid={!!errors.name} />
      </Field>
      <Field label="Country" required error={errors.country?.message}>
        <Input icon={<Globe2 className="size-4" />} {...register('country')} invalid={!!errors.country} />
      </Field>
      <Field label="Post office" className="sm:col-span-2" error={errors.postOffice?.message}>
        <Input {...register('postOffice')} invalid={!!errors.postOffice} />
      </Field>
    </div>
  )
}

function CityFormModal({ open, onClose, onSaved }: { open: boolean; onClose: () => void; onSaved: () => void }) {
  const form = useForm<Values>({ resolver: zodResolver(schema) })
  const create = useMutation({
    mutationFn: (values: Values) => citiesApi.create({ ...values, postOffice: values.postOffice || null }),
    onSuccess: (city) => {
      toast.success(`${city.name} created`)
      form.reset()
      onSaved()
      onClose()
    },
    onError: (error) => applyServerErrors(error, form.setError, ['name', 'country', 'postOffice']),
  })

  return (
    <Modal
      open={open}
      onClose={onClose}
      title="New city"
      description="Add a destination travellers can search for."
      footer={
        <>
          <Button variant="outline" onClick={onClose}>Cancel</Button>
          <Button onClick={form.handleSubmit((v) => create.mutate(v))} loading={create.isPending}>Create city</Button>
        </>
      }
    >
      <form onSubmit={form.handleSubmit((v) => create.mutate(v))}>
        <CityFields form={form} />
      </form>
    </Modal>
  )
}

function CityDrawer({ city, onClose, onSaved }: { city: City; onClose: () => void; onSaved: () => void }) {
  const [current, setCurrent] = useState(city)
  const form = useForm<Values>({
    resolver: zodResolver(schema),
    defaultValues: { name: city.name, country: city.country, postOffice: city.postOffice ?? '' },
  })

  const update = useMutation({
    mutationFn: (values: Values) =>
      citiesApi.update(city.cityId, { ...values, postOffice: values.postOffice || null, rowVersion: current.rowVersion }),
    onSuccess: (saved) => {
      toast.success('City updated')
      setCurrent(saved)
      onSaved()
      onClose()
    },
    onError: (error) => applyServerErrors(error, form.setError, ['name', 'country', 'postOffice']),
  })

  const upload = useMutation({
    mutationFn: (file: File) => citiesApi.uploadThumbnail(city.cityId, file),
    onSuccess: () => {
      toast.success('Thumbnail is uploading', { description: 'It will appear in a few seconds.' })
      setTimeout(async () => {
        const fresh = await citiesApi.get(city.cityId)
        setCurrent(fresh)
        onSaved()
      }, 5000)
    },
    onError: toastError,
  })

  const removeThumbnail = useMutation({
    mutationFn: () => citiesApi.removeThumbnail(city.cityId),
    onSuccess: async () => {
      toast.success('Thumbnail removed')
      setCurrent(await citiesApi.get(city.cityId))
      onSaved()
    },
    onError: toastError,
  })

  return (
    <Drawer
      open
      onClose={onClose}
      title={`Edit ${city.name}`}
      description={`${city.hotelsCount} hotels · created ${new Date(city.createdAt).toLocaleDateString()}`}
      footer={
        <>
          <Button variant="outline" onClick={onClose}>Cancel</Button>
          <Button onClick={form.handleSubmit((v) => update.mutate(v))} loading={update.isPending}>Save changes</Button>
        </>
      }
    >
      <div className="space-y-8">
        <section>
          <h3 className="mb-4 text-sm font-bold tracking-wide text-slate-400 uppercase">Details</h3>
          <CityFields form={form} />
        </section>
        <section>
          <h3 className="mb-4 text-sm font-bold tracking-wide text-slate-400 uppercase">Thumbnail</h3>
          {current.thumbnailUrl && (
            <div className="relative mb-4 overflow-hidden rounded-2xl">
              <img src={current.thumbnailUrl} alt={city.name} className="h-48 w-full object-cover" />
              <Button
                variant="danger"
                size="sm"
                className="absolute top-3 right-3"
                onClick={() => removeThumbnail.mutate()}
                loading={removeThumbnail.isPending}
              >
                <Trash2 className="size-3.5" /> Remove
              </Button>
            </div>
          )}
          <ImageUploadBox
            onFile={(file) => upload.mutate(file)}
            loading={upload.isPending}
            label={current.thumbnailUrl ? 'Replace thumbnail' : 'Upload a thumbnail'}
          />
          <p className="mt-2 text-xs text-slate-400">Shown in “Trending destinations” on the home page.</p>
        </section>
      </div>
    </Drawer>
  )
}
