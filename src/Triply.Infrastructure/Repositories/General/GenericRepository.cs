using System.Data;
using System.Linq.Expressions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Triply.Application.Interfaces.Repositories.General;
using Triply.Infrastructure.Db;

namespace Triply.Infrastructure.Repositories.General;

/// <summary>Generic Entity Framework implementation of <see cref="IGenericRepository{TEntity}"/>.</summary>
public class GenericRepository<TEntity>(
    TriplyDbContext context) : IGenericRepository<TEntity>
    where TEntity : class
{
    /// <inheritdoc />
    public virtual async Task<ICollection<TEntity>> GetAllAsync()
        => await context.Set<TEntity>().ToListAsync();

    /// <inheritdoc />
    public virtual async Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await context.Set<TEntity>().FindAsync(id, cancellationToken);

    /// <inheritdoc />
    public virtual async Task<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken)
        => await context.Set<TEntity>().AddAsync(entity, cancellationToken);

    /// <inheritdoc />
    public async Task AddRangeAsync(ICollection<TEntity> entities)
        => await context.BulkInsertAsync(entities);

    /// <inheritdoc />
    public virtual async Task UpdateAsync(TEntity entity)
        => context.Set<TEntity>().Update(entity);

    /// <inheritdoc />
    public virtual async Task UpdateRangeAsync(ICollection<TEntity> entities)
        => await context.BulkInsertOrUpdateAsync(entities);

    /// <inheritdoc />
    public virtual async Task DeleteAsync(TEntity entity)
        => context.Set<TEntity>().Remove(entity);

    /// <inheritdoc />
    public virtual async Task DeleteRangeAsync(ICollection<TEntity> entities)
        => await context.BulkDeleteAsync(entities);

    /// <inheritdoc />
    public async Task<IDbContextTransaction> BeginTransactionAsync()
        => await context.Database.BeginTransactionAsync();
    
    /// <inheritdoc />
    public async Task<IDbContextTransaction> BeginSerializableTransactionAsync(
        CancellationToken cancellationToken = default)
        => await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

    /// <inheritdoc />
    public async Task CommitAsync()
        => await context.Database.CommitTransactionAsync();

    /// <inheritdoc />
    public async Task RollBackAsync()
        => await context.Database.RollbackTransactionAsync();

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => await context.SaveChangesAsync(cancellationToken); 

    /// <inheritdoc />
    public async Task LoadAllCollectionsAsync(TEntity entity)
    {
        var navigations = context.Entry(entity).Collections;
        foreach (var navigation in navigations)
            if (!navigation.IsLoaded)
                await navigation.LoadAsync();
    }

    /// <inheritdoc />
    public async Task LoadReferenceAsync<TProperty>(TEntity entity,
        Expression<Func<TEntity, TProperty?>> navigationProperty) where TProperty : class
    {
        var navigation = context.Entry(entity).Reference(navigationProperty);
        if (!navigation.IsLoaded)
            await navigation.LoadAsync();
    }

    /// <inheritdoc />
    public async Task LoadCollectionAsync<TProperty>(
        TEntity entity,
        Expression<Func<TEntity, IEnumerable<TProperty>>> navigationProperty)
        where TProperty : class
    {
        var navigation = context.Entry(entity).Collection(navigationProperty);
        if (!navigation.IsLoaded)
            await navigation.LoadAsync();
    }

    /// <inheritdoc />
    public async Task LoadCollectionAsync<TProperty>(
        IEnumerable<TEntity> entities,
        Expression<Func<TEntity, IEnumerable<TProperty>>> navigationProperty)
        where TProperty : class
    {
        foreach (var entity in entities)
            await LoadCollectionAsync(entity, navigationProperty);
    }
    /// <inheritdoc />
    public IQueryable<TEntity> GetTableNoTracking()
        => context.Set<TEntity>().AsNoTracking().AsQueryable();
}