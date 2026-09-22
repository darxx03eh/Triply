import { useEffect, useMemo, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Plus, Search } from 'lucide-react'
import { toast } from 'sonner'
import { dealsApi } from '@/api/deals.api'
import { AdminPageHeader, AdminTableCard, DateCell } from '@/components/admin/AdminPage'
import { DataTable, type Column } from '@/components/admin/DataTable'
import { RowActions } from '@/components/admin/RowActions'
import { Button } from '@/components/ui/Button'
import { Field, Input, Select, Switch } from '@/components/ui/Field'
import { Badge, PageLoader } from '@/components/ui/Feedback'
import { ConfirmDialog, Drawer, Modal } from '@/components/ui/Overlay'
import { Pagination } from '@/components/ui/Pagination'
import { useDebounce } from '@/hooks/useDebounce'
import { useHotelOptions, useRoomOptions } from '@/hooks/useLookups'
import { applyServerErrors, toastError } from '@/lib/forms'
import { addDays, formatMoney } from '@/lib/format'
import { sieve } from '@/lib/sieve'
import { dealStatus, type Deal, type DealStatus } from '@/types/deal'
import type { Room } from '@/types/room'

const PAGE_SIZE = 10

const schema = z
  .object({
    hotelId: z.string().min(1, 'Choose a hotel'),
    roomId: z.string().min(1, 'Choose a room'),
    title: z.string().trim().min(1, 'Title is required').max(100, 'Title must not exceed 100 characters'),
    discountPercentage: z.coerce.number().gt(0, 'Between 1 and 90').max(90, 'Between 1 and 90'),
    startsAt: z.string().min(1, 'Start date is required'),
    endsAt: z.string().min(1, 'End date is required'),
    isFeatured: z.boolean(),
  })
  .refine((v) => new Date(v.endsAt) > new Date(v.startsAt), { path: ['endsAt'], message: 'Must be after the start date' })
  .refine((v) => new Date(v.endsAt) > new Date(), { path: ['endsAt'], message: 'Cannot be in the past' })
type Values = z.input<typeof schema>
type Parsed = z.output<typeof schema>
const fieldNames = ['roomId', 'title', 'discountPercentage', 'startsAt', 'endsAt', 'isFeatured']

const statusTone: Record<DealStatus, 'green' | 'amber' | 'slate'> = { Active: 'green', Scheduled: 'amber', Expired: 'slate' }

/** <input type="datetime-local"> works in local time; the API stores UTC. */
const toLocalInput = (value: string | Date) => {
  const date = new Date(value)
  return new Date(date.getTime() - date.getTimezoneOffset() * 60_000).toISOString().slice(0, 16)
}
const toUtc = (value: string) => new Date(value).toISOString()

export function AdminDealsPage() {
  const queryClient = useQueryClient()
  const rooms = useRoomOptions()
  const [search, setSearch] = useState('')
  const [featured, setFeatured] = useState('')
  const [page, setPage] = useState(1)
  const [creating, setCreating] = useState(false)
  const [editing, setEditing] = useState<Deal | null>(null)
  const [deleting, setDeleting] = useState<Deal | null>(null)
  const term = useDebounce(search)

  useEffect(() => {
    setPage(1)
  }, [term, featured])

  const deals = useQuery({
    queryKey: ['admin', 'deals', term, featured, page],
    queryFn: () =>
      dealsApi.list({
        filters: [sieve.contains('Title', term), sieve.equals('featured', featured)],
        sorts: '-CreatedAt',
        page,
        pageSize: PAGE_SIZE,
      }),
    placeholderData: (previous) => previous,
  })

  const roomsById = useMemo(() => new Map(rooms.data?.map((room) => [room.roomId, room])), [rooms.data])

  const refresh = () => {
    queryClient.invalidateQueries({ queryKey: ['admin', 'deals'] })
    queryClient.invalidateQueries({ queryKey: ['home', 'deals'] })
  }

  const remove = useMutation({
    mutationFn: (id: string) => dealsApi.remove(id),
    onSuccess: () => {
      toast.success('Deal deleted')
      setDeleting(null)
      refresh()
    },
    onError: toastError,
  })

  const columns: Column<Deal>[] = [
    { key: 'title', header: 'Deal', cell: (d) => <span className="font-semibold text-slate-900">{d.title}</span> },
    {
      key: 'room',
      header: 'Hotel / Room',
      cell: (d) => {
        const room = roomsById.get(d.roomId)
        return room ? (
          <div className="max-w-56">
            <p className="truncate">{room.hotelName}</p>
            <p className="text-xs text-slate-400">#{room.number} · {room.roomType}</p>
          </div>
        ) : <span className="text-slate-400">—</span>
      },
    },
    { key: 'discount', header: 'Discount', cell: (d) => <Badge tone="accent">-{d.discountPercentage}%</Badge> },
    {
      key: 'price',
      header: 'Price / night',
      cell: (d) => {
        const room = roomsById.get(d.roomId)
        if (!room) return <span className="text-slate-400">—</span>
        return (
          <span>
            <span className="mr-1.5 text-xs text-slate-400 line-through">{formatMoney(room.pricePerNight, true)}</span>
            <span className="font-semibold text-slate-900">{formatMoney(room.pricePerNight * (1 - d.discountPercentage / 100), true)}</span>
          </span>
        )
      },
    },
    {
      key: 'status',
      header: 'Status',
      cell: (d) => {
        const status = dealStatus(d)
        return (
          <div className="flex flex-wrap gap-1">
            <Badge tone={statusTone[status]}>{status}</Badge>
            {d.isFeatured && <Badge tone="brand">Featured</Badge>}
          </div>
        )
      },
    },
    { key: 'starts', header: 'Starts', cell: (d) => <DateCell value={d.startsAt} /> },
    { key: 'ends', header: 'Ends', cell: (d) => <DateCell value={d.endsAt} /> },
    {
      key: 'actions',
      header: '',
      className: 'text-right',
      cell: (d) => <RowActions onEdit={() => setEditing(d)} onDelete={() => setDeleting(d)} />,
    },
  ]

  return (
    <>
      <AdminPageHeader
        title="Deals"
        description="Discounts on rooms. Running featured deals are shown on the home page."
        actions={<Button onClick={() => setCreating(true)}><Plus className="size-4" /> New deal</Button>}
      />
      <AdminTableCard
        toolbar={
          <>
            <div className="w-full lg:max-w-72">
              <Input icon={<Search className="size-4" />} placeholder="Search by title…" value={search} onChange={(e) => setSearch(e.target.value)} />
            </div>
            <div className="w-full lg:max-w-52">
              <Select value={featured} onChange={(e) => setFeatured(e.target.value)}>
                <option value="">All deals</option>
                <option value="true">Featured only</option>
                <option value="false">Not featured</option>
              </Select>
            </div>
          </>
        }
      >
        <DataTable
          columns={columns}
          rows={deals.data?.items}
          rowKey={(d) => d.dealId}
          loading={deals.isLoading}
          error={deals.error}
          onRetry={() => deals.refetch()}
          onRowClick={setEditing}
          rowClassName={(d) => (dealStatus(d) === 'Expired' ? 'opacity-60' : undefined)}
          emptyTitle="No deals found"
          emptyDescription="Create a deal to show it on the home page."
        />
        {deals.data && (
          <Pagination page={page} totalPages={deals.data.totalPages} totalCount={deals.data.totalCount} pageSize={PAGE_SIZE} onChange={setPage} />
        )}
      </AdminTableCard>

      <DealFormModal open={creating} onClose={() => setCreating(false)} onSaved={refresh} />
      {editing && (
        <Drawer open onClose={() => setEditing(null)} title={editing.title} description={roomsById.get(editing.roomId)?.hotelName}>
          {rooms.isLoading ? <PageLoader /> : <DealEditForm deal={editing} room={roomsById.get(editing.roomId)} onClose={() => setEditing(null)} onSaved={refresh} />}
        </Drawer>
      )}
      <ConfirmDialog
        open={!!deleting}
        onClose={() => setDeleting(null)}
        onConfirm={() => deleting && remove.mutate(deleting.dealId)}
        loading={remove.isPending}
        title="Delete deal?"
        description={<>“{deleting?.title}” will be removed and its discount will no longer apply.</>}
      />
    </>
  )
}

function DealFields({ form, lockRoom }: { form: ReturnType<typeof useForm<Values, unknown, Parsed>>; lockRoom?: boolean }) {
  const hotels = useHotelOptions()
  const rooms = useRoomOptions()
  const { register, watch, setValue, formState: { errors } } = form
  const hotelId = watch('hotelId')
  const hotelRooms = rooms.data?.filter((room) => room.hotelId === hotelId) ?? []
  const room = rooms.data?.find((r) => r.roomId === watch('roomId'))
  const discount = Number(watch('discountPercentage')) || 0

  return (
    <div className="grid gap-4 sm:grid-cols-2">
      <Field label="Hotel" required error={errors.hotelId?.message}>
        <Select
          {...register('hotelId', { onChange: () => setValue('roomId', '') })}
          disabled={lockRoom}
          invalid={!!errors.hotelId}
        >
          <option value="">Select a hotel</option>
          {hotels.data?.map((h) => <option key={h.hotelId} value={h.hotelId}>{h.name} — {h.cityName}</option>)}
        </Select>
      </Field>
      <Field label="Room" required error={errors.roomId?.message}>
        <Select {...register('roomId')} disabled={lockRoom || !hotelId} invalid={!!errors.roomId}>
          <option value="">{hotelId ? 'Select a room' : 'Choose a hotel first'}</option>
          {hotelRooms.map((r) => (
            <option key={r.roomId} value={r.roomId}>#{r.number} · {r.roomType} · {formatMoney(r.pricePerNight, true)}</option>
          ))}
        </Select>
      </Field>
      <Field label="Title" required className="sm:col-span-2" error={errors.title?.message}>
        <Input placeholder="Summer Escape" {...register('title')} invalid={!!errors.title} />
      </Field>
      <Field
        label="Discount (%)"
        required
        error={errors.discountPercentage?.message}
        hint={room && discount > 0 && discount <= 90 ? `${formatMoney(room.pricePerNight, true)} → ${formatMoney(room.pricePerNight * (1 - discount / 100), true)} per night` : undefined}
      >
        <Input type="number" step="0.01" min={1} max={90} {...register('discountPercentage')} invalid={!!errors.discountPercentage} />
      </Field>
      <div className="flex items-end pb-2.5">
        <Switch checked={!!watch('isFeatured')} onChange={(v) => setValue('isFeatured', v)} label="Show on the home page" />
      </div>
      <Field label="Starts" required error={errors.startsAt?.message}>
        <Input type="datetime-local" {...register('startsAt')} invalid={!!errors.startsAt} />
      </Field>
      <Field label="Ends" required error={errors.endsAt?.message}>
        <Input type="datetime-local" {...register('endsAt')} invalid={!!errors.endsAt} />
      </Field>
    </div>
  )
}

const emptyDeal = (): Values => ({
  hotelId: '',
  roomId: '',
  title: '',
  discountPercentage: 20,
  startsAt: toLocalInput(new Date()),
  endsAt: toLocalInput(addDays(new Date(), 30)),
  isFeatured: true,
})

function DealFormModal({ open, onClose, onSaved }: { open: boolean; onClose: () => void; onSaved: () => void }) {
  const form = useForm<Values, unknown, Parsed>({ resolver: zodResolver(schema), defaultValues: emptyDeal() })

  const create = useMutation({
    mutationFn: ({ hotelId: _hotelId, ...values }: Parsed) =>
      dealsApi.create({ ...values, startsAt: toUtc(values.startsAt), endsAt: toUtc(values.endsAt) }),
    onSuccess: (deal) => {
      toast.success(`Deal “${deal.title}” created`)
      form.reset(emptyDeal())
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
      title="New deal"
      description="The discount applies to the room while the deal is running."
      footer={
        <>
          <Button variant="outline" onClick={onClose}>Cancel</Button>
          <Button onClick={form.handleSubmit((v) => create.mutate(v))} loading={create.isPending}>Create deal</Button>
        </>
      }
    >
      <DealFields form={form} />
    </Modal>
  )
}

function DealEditForm({ deal, room, onClose, onSaved }: { deal: Deal; room?: Room; onClose: () => void; onSaved: () => void }) {
  const form = useForm<Values, unknown, Parsed>({
    resolver: zodResolver(schema),
    defaultValues: {
      hotelId: room?.hotelId ?? '',
      roomId: deal.roomId,
      title: deal.title,
      discountPercentage: deal.discountPercentage,
      startsAt: toLocalInput(deal.startsAt),
      endsAt: toLocalInput(deal.endsAt),
      isFeatured: deal.isFeatured,
    },
  })

  const update = useMutation({
    mutationFn: ({ hotelId: _hotelId, roomId: _roomId, ...values }: Parsed) =>
      dealsApi.update(deal.dealId, { ...values, startsAt: toUtc(values.startsAt), endsAt: toUtc(values.endsAt) }),
    onSuccess: () => {
      toast.success('Deal updated')
      onSaved()
      onClose()
    },
    onError: (error) => applyServerErrors(error, form.setError, fieldNames),
  })

  return (
    <form onSubmit={form.handleSubmit((v) => update.mutate(v))} className="space-y-6">
      <DealFields form={form} lockRoom />
      <div className="flex justify-end gap-2 border-t border-slate-100 pt-5">
        <Button type="button" variant="outline" onClick={onClose}>Cancel</Button>
        <Button type="submit" loading={update.isPending}>Save changes</Button>
      </div>
    </form>
  )
}
