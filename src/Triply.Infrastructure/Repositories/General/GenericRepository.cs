using System.Linq.Expressions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Triply.Application.Interfaces.Repositories.General;
using Triply.Infrastructure.Db;

namespace Triply.Infrastructure.Repositories.General;

public class GenericRepository<TEntity>(
    TriplyDbContext context) : IGenericRepository<TEntity>
    where TEntity : class
{
    public virtual async Task<ICollection<TEntity>> GetAllAsync()
        => await context.Set<TEntity>().ToListAsync();

    public virtual async Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await context.Set<TEntity>().FindAsync(id, cancellationToken);

    public virtual async Task<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken)
        => await context.Set<TEntity>().AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(ICollection<TEntity> entities)
        => await context.BulkInsertAsync(entities);

    public virtual async Task UpdateAsync(TEntity entity)
        => context.Set<TEntity>().Update(entity);

    public virtual async Task UpdateRangeAsync(ICollection<TEntity> entities)
        => await context.BulkInsertOrUpdateAsync(entities);

    public virtual async Task DeleteAsync(TEntity entity)
        => context.Set<TEntity>().Remove(entity);

    public virtual async Task DeleteRangeAsync(ICollection<TEntity> entities)
        => await context.BulkDeleteAsync(entities);

    public async Task<IDbContextTransaction> BeginTransactionAsync()
        => await context.Database.BeginTransactionAsync();

    public async Task CommitAsync()
        => await context.Database.CommitTransactionAsync();

    public async Task RollBackAsync()
        => await context.Database.RollbackTransactionAsync();

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => await context.SaveChangesAsync(cancellationToken); 

    public async Task LoadAllCollectionsAsync(TEntity entity)
    {
        var navigations = context.Entry(entity).Collections;
        foreach (var navigation in navigations)
            if (!navigation.IsLoaded)
                await navigation.LoadAsync();
    }

    public async Task LoadReferenceAsync<TProperty>(TEntity entity,
        Expression<Func<TEntity, TProperty?>> navigationProperty) where TProperty : class
    {
        var navigation = context.Entry(entity).Reference(navigationProperty);
        if (!navigation.IsLoaded)
            await navigation.LoadAsync();
    }

    public async Task LoadCollectionAsync<TProperty>(
        TEntity entity,
        Expression<Func<TEntity, IEnumerable<TProperty>>> navigationProperty)
        where TProperty : class
    {
        var navigation = context.Entry(entity).Collection(navigationProperty);
        if (!navigation.IsLoaded)
            await navigation.LoadAsync();
    }

    public async Task LoadCollectionAsync<TProperty>(
        IEnumerable<TEntity> entities,
        Expression<Func<TEntity, IEnumerable<TProperty>>> navigationProperty)
        where TProperty : class
    {
        foreach (var entity in entities)
            await LoadCollectionAsync(entity, navigationProperty);
    }
    public IQueryable<TEntity> GetTableNoTracking()
        => context.Set<TEntity>().AsNoTracking().AsQueryable();
}