import { Pencil, Trash2 } from 'lucide-react'

export function RowActions({ onEdit, onDelete, disabled }: { onEdit: () => void; onDelete: () => void; disabled?: boolean }) {
  return (
    <div className="flex justify-end gap-1" onClick={(e) => e.stopPropagation()}>
      <button onClick={onEdit} className="rounded-lg p-2 text-slate-400 transition hover:bg-brand-50 hover:text-brand-600" aria-label="Edit">
        <Pencil className="size-4" />
      </button>
      <button
        onClick={onDelete}
        disabled={disabled}
        className="rounded-lg p-2 text-slate-400 transition hover:bg-red-50 hover:text-red-600 disabled:pointer-events-none disabled:opacity-30"
        aria-label="Delete"
      >
        <Trash2 className="size-4" />
      </button>
    </div>
  )
}
