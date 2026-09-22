import { useEffect, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Plus, Search } from 'lucide-react'
import { toast } from 'sonner'
import { roomsApi } from '@/api/rooms.api'
import { AdminPageHeader, AdminTableCard, DateCell } from '@/components/admin/AdminPage'
import { DataTable, type Column } from '@/components/admin/DataTable'
import { RowActions } from '@/components/admin/RowActions'
import { Button } from '@/components/ui/Button'
import { Field, Input, Select, Switch, Textarea } from '@/components/ui/Field'
import { Badge, PageLoader } from '@/components/ui/Feedback'
import { ConfirmDialog, Drawer, Modal } from '@/components/ui/Overlay'
import { Pagination } from '@/components/ui/Pagination'
import { useDebounce } from '@/hooks/useDebounce'
import { useHotelOptions } from '@/hooks/useLookups'
import { applyServerErrors, toastError } from '@/lib/forms'
import { formatMoney } from '@/lib/format'
import { sieve } from '@/lib/sieve'
import { ROOM_TYPES, type Room } from '@/types/room'

const PAGE_SIZE = 10

const schema = z.object({
  hotelId: z.string().min(1, 'Choose a hotel'),
  number: z.string().trim().min(1, 'Room number is required').max(20),
  roomType: z.enum(ROOM_TYPES),
  adultCapacity: z.coerce.number().int().min(1, 'Between 1 and 10').max(10, 'Between 1 and 10'),
  childCapacity: z.coerce.number().int().min(0, 'Between 0 and 10').max(10, 'Between 0 and 10'),
  pricePerNight: z.coerce.number().positive('Must be greater than 0'),
  isAvailable: z.boolean(),
  description: z.string().max(1000).optional(),
})
type Values = z.input<typeof schema>
type Parsed = z.output<typeof schema>
const fieldNames = ['hotelId', 'number', 'roomType', 'adultCapacity', 'childCapacity', 'pricePerNight', 'isAvailable', 'description']

export function AdminRoomsPage() {
  const queryClient = useQueryClient()
  const hotels = useHotelOptions()
  const [search, setSearch] = useState('')
  const [hotelId, setHotelId] = useState('')
  const [type, setType] = useState('')
  const [available, setAvailable] = useState('')
  const [page, setPage] = useState(1)
  const [creating, setCreating] = useState(false)
  const [editing, setEditing] = useState<Room | null>(null)
  const [deleting, setDeleting] = useState<Room | null>(null)
  const term = useDebounce(search)

  useEffect(() => {
    setPage(1)
  }, [term, hotelId, type, available])

  const rooms = useQuery({
    queryKey: ['admin', 'rooms', term, hotelId, type, available, page],
    queryFn: () =>
      roomsApi.list({
        filters: [sieve.contains('Number', term), sieve.equals('HotelId', hotelId), sieve.equals('room', type), sieve.equals('available', available)],
        sorts: '-CreatedAt',
        page,
        pageSize: PAGE_SIZE,
      }),
    placeholderData: (previous) => previous,
  })

  const refresh = () => {
    queryClient.invalidateQueries({ queryKey: ['admin', 'rooms'] })
    queryClient.invalidateQueries({ queryKey: ['admin', 'hotels'] })
  }

  const remove = useMutation({
    mutationFn: (id: string) => roomsApi.remove(id),
    onSuccess: () => {
      toast.success('Room deleted')
      setDeleting(null)
      refresh()
    },
    onError: toastError,
  })

  const columns: Column<Room>[] = [
    {
      key: 'number',
      header: 'Room',
      cell: (r) => (
        <div>
          <p className="font-semibold text-slate-900">#{r.number}</p>
          {r.isDeleted && <Badge tone="red" className="mt-0.5">Deleted</Badge>}
        </div>
      ),
    },
    { key: 'hotel', header: 'Hotel', cell: (r) => <span className="block max-w-52 truncate">{r.hotelName}</span> },
    { key: 'type', header: 'Type', cell: (r) => r.roomType },
    {
      key: 'availability',
      header: 'Availability',
      cell: (r) => <Badge tone={r.isAvailable ? 'green' : 'slate'}>{r.isAvailable ? 'Available' : 'Unavailable'}</Badge>,
    },
    { key: 'adults', header: 'Adults', cell: (r) => r.adultCapacity },
    { key: 'children', header: 'Children', cell: (r) => r.childCapacity },
    { key: 'price', header: 'Price / night', cell: (r) => <span className="font-semibold text-slate-900">{formatMoney(r.pricePerNight, true)}</span> },
    { key: 'created', header: 'Created', cell: (r) => <DateCell value={r.createdAt} /> },
    { key: 'modified', header: 'Modified', cell: (r) => <DateCell value={r.modifiedAt} /> },
    {
      key: 'actions',
      header: '',
      className: 'text-right',
      cell: (r) => <RowActions onEdit={() => setEditing(r)} onDelete={() => setDeleting(r)} disabled={r.isDeleted} />,
    },
  ]

  return (
    <>
      <AdminPageHeader
        title="Rooms"
        description="Every bookable room across all hotels. Click a row to edit it."
        actions={<Button onClick={() => setCreating(true)}><Plus className="size-4" /> New room</Button>}
      />
      <AdminTableCard
        toolbar={
          <>
            <div className="w-full lg:max-w-60">
              <Input icon={<Search className="size-4" />} placeholder="Room number…" value={search} onChange={(e) => setSearch(e.target.value)} />
            </div>
            <div className="grid flex-1 grid-cols-1 gap-3 sm:grid-cols-3 lg:max-w-2xl">
              <Select value={hotelId} onChange={(e) => setHotelId(e.target.value)}>
                <option value="">All hotels</option>
                {hotels.data?.map((h) => <option key={h.hotelId} value={h.hotelId}>{h.name}</option>)}
              </Select>
              <Select value={type} onChange={(e) => setType(e.target.value)}>
                <option value="">All types</option>
                {ROOM_TYPES.map((t) => <option key={t} value={t}>{t}</option>)}
              </Select>
              <Select value={available} onChange={(e) => setAvailable(e.target.value)}>
                <option value="">Any availability</option>
                <option value="true">Available</option>
                <option value="false">Unavailable</option>
              </Select>
            </div>
          </>
        }
      >
        <DataTable
          columns={columns}
          rows={rooms.data?.items}
          rowKey={(r) => r.roomId}
          loading={rooms.isLoading}
          error={rooms.error}
          onRetry={() => rooms.refetch()}
          onRowClick={(r) => !r.isDeleted && setEditing(r)}
          rowClassName={(r) => (r.isDeleted ? 'opacity-50' : undefined)}
          emptyTitle="No rooms found"
        />
        {rooms.data && (
          <Pagination page={page} totalPages={rooms.data.totalPages} totalCount={rooms.data.totalCount} pageSize={PAGE_SIZE} onChange={setPage} />
        )}
      </AdminTableCard>

      <RoomFormModal open={creating} onClose={() => setCreating(false)} onSaved={refresh} defaultHotelId={hotelId} />
      {editing && <RoomDrawer roomId={editing.roomId} onClose={() => setEditing(null)} onSaved={refresh} />}
      <ConfirmDialog
        open={!!deleting}
        onClose={() => setDeleting(null)}
        onConfirm={() => deleting && remove.mutate(deleting.roomId)}
        loading={remove.isPending}
        title="Delete room?"
        description={<>Room #{deleting?.number} at {deleting?.hotelName} will no longer be bookable.</>}
      />
    </>
  )
}

function RoomFields({ form, lockHotel }: { form: ReturnType<typeof useForm<Values, unknown, Parsed>>; lockHotel?: boolean }) {
  const hotels = useHotelOptions()
  const { register, watch, setValue, formState: { errors } } = form
  return (
    <div className="grid gap-4 sm:grid-cols-2">
      <Field label="Hotel" required className="sm:col-span-2" error={errors.hotelId?.message}>
        <Select {...register('hotelId')} disabled={lockHotel} invalid={!!errors.hotelId}>
          <option value="">Select a hotel</option>
          {hotels.data?.map((h) => <option key={h.hotelId} value={h.hotelId}>{h.name} — {h.cityName}</option>)}
        </Select>
      </Field>
      <Field label="Room number" required error={errors.number?.message}><Input {...register('number')} invalid={!!errors.number} /></Field>
      <Field label="Room type" required error={errors.roomType?.message}>
        <Select {...register('roomType')}>{ROOM_TYPES.map((t) => <option key={t} value={t}>{t}</option>)}</Select>
      </Field>
      <Field label="Adults" required error={errors.adultCapacity?.message}><Input type="number" min={1} max={10} {...register('adultCapacity')} invalid={!!errors.adultCapacity} /></Field>
      <Field label="Children" required error={errors.childCapacity?.message}><Input type="number" min={0} max={10} {...register('childCapacity')} invalid={!!errors.childCapacity} /></Field>
      <Field label="Price per night (USD)" required error={errors.pricePerNight?.message}><Input type="number" step="0.01" min={0} {...register('pricePerNight')} invalid={!!errors.pricePerNight} /></Field>
      <div className="flex items-end pb-2.5">
        <Switch checked={!!watch('isAvailable')} onChange={(v) => setValue('isAvailable', v)} label="Available for booking" />
      </div>
      <Field label="Description" className="sm:col-span-2" error={errors.description?.message}><Textarea rows={3} {...register('description')} /></Field>
    </div>
  )
}

function RoomFormModal({ open, onClose, onSaved, defaultHotelId }: { open: boolean; onClose: () => void; onSaved: () => void; defaultHotelId: string }) {
  const form = useForm<Values, unknown, Parsed>({
    resolver: zodResolver(schema),
    defaultValues: { hotelId: defaultHotelId, roomType: 'Double', adultCapacity: 2, childCapacity: 0, isAvailable: true },
  })
  useEffect(() => {
    if (open) form.setValue('hotelId', defaultHotelId)
  }, [open, defaultHotelId, form])

  const create = useMutation({
    mutationFn: (values: Parsed) => roomsApi.create({ ...values, description: values.description || null }),
    onSuccess: (room) => {
      toast.success(`Room #${room.number} created`)
      form.reset({ hotelId: defaultHotelId, roomType: 'Double', adultCapacity: 2, childCapacity: 0, isAvailable: true })
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
      title="New room"
      footer={
        <>
          <Button variant="outline" onClick={onClose}>Cancel</Button>
          <Button onClick={form.handleSubmit((v) => create.mutate(v))} loading={create.isPending}>Create room</Button>
        </>
      }
    >
      <RoomFields form={form} />
    </Modal>
  )
}

function RoomDrawer({ roomId, onClose, onSaved }: { roomId: string; onClose: () => void; onSaved: () => void }) {
  const room = useQuery({ queryKey: ['admin', 'room', roomId], queryFn: () => roomsApi.get(roomId) })
  return (
    <Drawer open onClose={onClose} title={room.data ? `Room #${room.data.number}` : 'Room'} description={room.data?.hotelName}>
      {room.data ? <RoomEditForm room={room.data} onClose={onClose} onSaved={onSaved} /> : <PageLoader />}
    </Drawer>
  )
}

function RoomEditForm({ room, onClose, onSaved }: { room: Room; onClose: () => void; onSaved: () => void }) {
  const form = useForm<Values, unknown, Parsed>({
    resolver: zodResolver(schema),
    defaultValues: {
      hotelId: room.hotelId,
      number: room.number,
      roomType: room.roomType,
      adultCapacity: room.adultCapacity,
      childCapacity: room.childCapacity,
      pricePerNight: room.pricePerNight,
      isAvailable: room.isAvailable,
      description: room.description ?? '',
    },
  })

  const update = useMutation({
    mutationFn: ({ hotelId: _hotelId, ...values }: Parsed) =>
      roomsApi.update(room.roomId, { ...values, description: values.description || null, rowVersion: room.rowVersion }),
    onSuccess: () => {
      toast.success('Room updated')
      onSaved()
      onClose()
    },
    onError: (error) => applyServerErrors(error, form.setError, fieldNames),
  })

  return (
    <form onSubmit={form.handleSubmit((v) => update.mutate(v))} className="space-y-6">
      <RoomFields form={form} lockHotel />
      <div className="flex justify-end gap-2 border-t border-slate-100 pt-5">
        <Button type="button" variant="outline" onClick={onClose}>Cancel</Button>
        <Button type="submit" loading={update.isPending}>Save changes</Button>
      </div>
    </form>
  )
}
