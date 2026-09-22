import { useRef, useState, type DragEvent } from 'react'
import { ImagePlus } from 'lucide-react'
import { cn } from '@/lib/cn'
import { Spinner } from '../ui/Feedback'

const ACCEPT = ['image/jpeg', 'image/png', 'image/webp']

export function ImageUploadBox({ onFile, loading, label = 'Drop an image or click to browse' }: {
  onFile: (file: File) => void
  loading?: boolean
  label?: string
}) {
  const input = useRef<HTMLInputElement>(null)
  const [drag, setDrag] = useState(false)

  const pick = (file?: File) => file && onFile(file)
  const onDrop = (e: DragEvent) => {
    e.preventDefault()
    setDrag(false)
    pick(e.dataTransfer.files[0])
  }

  return (
    <button
      type="button"
      disabled={loading}
      onClick={() => input.current?.click()}
      onDragOver={(e) => {
        e.preventDefault()
        setDrag(true)
      }}
      onDragLeave={() => setDrag(false)}
      onDrop={onDrop}
      className={cn(
        'flex w-full cursor-pointer flex-col items-center justify-center gap-2 rounded-2xl border-2 border-dashed px-6 py-8 text-center transition',
        drag ? 'border-brand-500 bg-brand-50' : 'border-slate-200 hover:border-brand-300 hover:bg-slate-50',
      )}
    >
      {loading ? <Spinner className="size-7" /> : <ImagePlus className="size-7 text-brand-500" />}
      <span className="text-sm font-semibold text-slate-700">{loading ? 'Uploading…' : label}</span>
      <span className="text-xs text-slate-400">JPG, PNG or WEBP · up to 10 MB</span>
      <input
        ref={input}
        type="file"
        accept={ACCEPT.join(',')}
        className="hidden"
        onChange={(e) => {
          pick(e.target.files?.[0])
          e.target.value = ''
        }}
      />
    </button>
  )
}
