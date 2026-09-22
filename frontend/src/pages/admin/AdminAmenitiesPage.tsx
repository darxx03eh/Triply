import { useState, type FormEvent } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { Check, Pencil, Plus, Sparkles, Trash2, X } from 'lucide-react'
import { toast } from 'sonner'
import { amenitiesApi } from '@/api/amenities.api'
import { AdminPageHeader } from '@/components/admin/AdminPage'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { Input } from '@/components/ui/Field'
import { EmptyState, Skeleton } from '@/components/ui/Feedback'
import { ConfirmDialog } from '@/components/ui/Overlay'
import { useAmenityOptions } from '@/hooks/useLookups'
import { toastError } from '@/lib/forms'
import type { Amenity } from '@/types/amenity'

export function AdminAmenitiesPage() {
  const queryClient = useQueryClient()
  const amenities = useAmenityOptions()
  const [name, setName] = useState('')
  const [editingId, setEditingId] = useState<string | null>(null)
  const [editName, setEditName] = useState('')
  const [deleting, setDeleting] = useState<Amenity | null>(null)

  const refresh = () => queryClient.invalidateQueries({ queryKey: ['lookup', 'amenities'] })

  const create = useMutation({
    mutationFn: () => amenitiesApi.create(name),
    onSuccess: (a) => {
      toast.success(`“${a.name}” added`)
      setName('')
      refresh()
    },
    onError: toastError,
  })

  const rename = useMutation({
    mutationFn: (id: string) => amenitiesApi.update(id, editName),
    onSuccess: () => {
      toast.success('Amenity renamed')
      setEditingId(null)
      refresh()
    },
    onError: toastError,
  })

  const remove = useMutation({
    mutationFn: (id: string) => amenitiesApi.remove(id),
    onSuccess: () => {
      toast.success('Amenity deleted')
      setDeleting(null)
      refresh()
    },
    onError: toastError,
  })

  const submit = (e: FormEvent) => {
    e.preventDefault()
    if (name.trim()) create.mutate()
  }

  return (
    <>
      <AdminPageHeader title="Amenities" description="Facilities hotels can offer. Guests use them to filter search results." />
      <Card className="mb-6 p-4">
        <form onSubmit={submit} className="flex flex-col gap-3 sm:flex-row">
          <div className="flex-1">
            <Input icon={<Sparkles className="size-4" />} placeholder="e.g. Rooftop terrace" value={name} onChange={(e) => setName(e.target.value)} />
          </div>
          <Button type="submit" loading={create.isPending} disabled={!name.trim()}><Plus className="size-4" /> Add amenity</Button>
        </form>
      </Card>

      {amenities.isLoading ? (
        <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-3">{Array.from({ length: 9 }, (_, i) => <Skeleton key={i} className="h-16" />)}</div>
      ) : amenities.data?.length === 0 ? (
        <EmptyState title="No amenities yet" description="Add the first one above." />
      ) : (
        <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-3">
          {amenities.data?.map((amenity) => (
            <Card key={amenity.amenityId} className="flex items-center gap-3 p-3 pl-4">
              <span className="flex size-9 shrink-0 items-center justify-center rounded-xl bg-brand-50 text-brand-600"><Sparkles className="size-4" /></span>
              {editingId === amenity.amenityId ? (
                <form
                  className="flex flex-1 items-center gap-1.5"
                  onSubmit={(e) => {
                    e.preventDefault()
                    rename.mutate(amenity.amenityId)
                  }}
                >
                  <Input autoFocus value={editName} onChange={(e) => setEditName(e.target.value)} className="h-9" />
                  <Button type="submit" size="icon" variant="ghost" loading={rename.isPending}><Check className="size-4 text-emerald-600" /></Button>
                  <Button type="button" size="icon" variant="ghost" onClick={() => setEditingId(null)}><X className="size-4" /></Button>
                </form>
              ) : (
                <>
                  <span className="flex-1 truncate font-semibold text-slate-800">{amenity.name}</span>
                  <Button
                    size="icon"
                    variant="ghost"
                    onClick={() => {
                      setEditingId(amenity.amenityId)
                      setEditName(amenity.name)
                    }}
                    aria-label="Rename"
                  >
                    <Pencil className="size-4" />
                  </Button>
                  <Button size="icon" variant="ghost" className="hover:bg-red-50 hover:text-red-600" onClick={() => setDeleting(amenity)} aria-label="Delete">
                    <Trash2 className="size-4" />
                  </Button>
                </>
              )}
            </Card>
          ))}
        </div>
      )}

      <ConfirmDialog
        open={!!deleting}
        onClose={() => setDeleting(null)}
        onConfirm={() => deleting && remove.mutate(deleting.amenityId)}
        loading={remove.isPending}
        title="Delete amenity?"
        description={<>“{deleting?.name}” will be removed from every hotel that offers it.</>}
      />
    </>
  )
}
