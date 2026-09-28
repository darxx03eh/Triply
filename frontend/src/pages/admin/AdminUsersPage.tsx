import { useEffect, useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { Search } from 'lucide-react'
import { usersApi } from '@/api/users.api'
import { AdminPageHeader, AdminTableCard, DateCell } from '@/components/admin/AdminPage'
import { DataTable, type Column } from '@/components/admin/DataTable'
import { Badge } from '@/components/ui/Feedback'
import { Input } from '@/components/ui/Field'
import { Pagination } from '@/components/ui/Pagination'
import { useDebounce } from '@/hooks/useDebounce'
import type { User } from '@/types/user'

const PAGE_SIZE = 10

export function AdminUsersPage() {
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(1)
  const term = useDebounce(search)

  useEffect(() => {
    setPage(1)
  }, [term])

  const users = useQuery({
    queryKey: ['admin', 'users', term, page],
    queryFn: () => usersApi.list({
      filters: [term && '(FirstName|LastName|Email|UserName)@=*' + term.trim()],
      sorts: '-CreatedAt',
      page,
      pageSize: PAGE_SIZE,
    }),
    placeholderData: (previous) => previous,
  })

  const columns: Column<User>[] = [
    {
      key: 'user',
      header: 'User',
      cell: (user) => (
        <div>
          <p className="font-semibold text-slate-900">{[user.firstName, user.lastName].filter(Boolean).join(' ') || user.userName || '—'}</p>
          <p className="mt-0.5 text-xs text-slate-400">@{user.userName ?? '—'}</p>
        </div>
      ),
    },
    {
      key: 'email',
      header: 'Email',
      cell: (user) => (
        <div>
          <p>{user.email ?? '—'}</p>
          <Badge tone={user.emailConfirmed ? 'green' : 'amber'} className="mt-1">
            {user.emailConfirmed ? 'Confirmed' : 'Unconfirmed'}
          </Badge>
        </div>
      ),
    },
    {
      key: 'roles',
      header: 'Roles',
      cell: (user) => user.roles.length
        ? <div className="flex flex-wrap gap-1">{user.roles.map((role) => <Badge key={role} tone={role === 'Admin' ? 'brand' : 'slate'}>{role}</Badge>)}</div>
        : <span className="text-slate-300">—</span>,
    },
    {
      key: 'status',
      header: 'Status',
      cell: (user) => user.isDeleted
        ? <Badge tone="red">Deleted</Badge>
        : <Badge tone={user.isActive ? 'green' : 'amber'}>{user.isActive ? 'Active' : 'Inactive'}</Badge>,
    },
    { key: 'lastLogin', header: 'Last login', cell: (user) => <DateCell value={user.lastLoginAt} /> },
    { key: 'created', header: 'Joined', cell: (user) => <DateCell value={user.createdAt} /> },
  ]

  return (
    <>
      <AdminPageHeader
        title="Users"
        description="View registered Triply accounts, their roles, and account status."
      />
      <AdminTableCard
        toolbar={
          <div className="w-full lg:max-w-sm">
            <Input icon={<Search className="size-4" />} placeholder="Search name, email, or username…" value={search} onChange={(event) => setSearch(event.target.value)} />
          </div>
        }
      >
        <DataTable
          columns={columns}
          rows={users.data?.items}
          rowKey={(user) => user.userId}
          loading={users.isLoading}
          error={users.error}
          onRetry={() => users.refetch()}
          emptyTitle="No users found"
          emptyDescription="Try a different name, email address, or username."
        />
        {users.data && (
          <Pagination page={page} totalPages={users.data.totalPages} totalCount={users.data.totalCount} pageSize={PAGE_SIZE} onChange={setPage} />
        )}
      </AdminTableCard>
    </>
  )
}
