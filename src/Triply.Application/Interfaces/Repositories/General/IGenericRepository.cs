using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Triply.Application.Interfaces.Repositories.General;

/// <summary>Defines common persistence operations for an entity.</summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IGenericRepository<TEntity> where TEntity : class
{
    /// <summary>Gets all entities.</summary>
    Task<ICollection<TEntity>> GetAllAsync();
    /// <summary>Gets an entity by uuid identifier.</summary>
    Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    /// <summary>Adds an entity to the current unit of work.</summary>
    Task<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken);
    /// <summary>Adds a collection of entities.</summary>
    Task AddRangeAsync(ICollection<TEntity> entities);
    /// <summary>Marks an entity for update.</summary>
    Task UpdateAsync(TEntity entity);
    /// <summary>Updates a collection of entities.</summary>
    Task UpdateRangeAsync(ICollection<TEntity> entities);
    /// <summary>Marks an entity for deletion.</summary>
    Task DeleteAsync(TEntity entity);
    /// <summary>Deletes a collection of entities.</summary>
    Task DeleteRangeAsync(ICollection<TEntity> entities);
    /// <summary>Begins a database transaction.</summary>
    Task<IDbContextTransaction> BeginTransactionAsync();
    /// <summary>Commits the current database transaction.</summary>
    Task CommitAsync();
    /// <summary>Rolls back the current database transaction.</summary>
    Task RollBackAsync();
    /// <summary>Persists pending changes.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    /// <summary>Loads all collection navigations for an entity.</summary>
    Task LoadAllCollectionsAsync(TEntity entity);
    /// <summary>Loads a reference navigation property.</summary>
    Task LoadReferenceAsync<TProperty>(
        TEntity entity,
        Expression<Func<TEntity, TProperty?>> navigationProperty) where TProperty : class;

    /// <summary>Loads a collection navigation property for one entity.</summary>
    Task LoadCollectionAsync<TProperty>(
        TEntity entity,
        Expression<Func<TEntity, IEnumerable<TProperty>>> navigationProperty)
        where TProperty : class;

    /// <summary>Loads a collection navigation property for multiple entities.</summary>
    Task LoadCollectionAsync<TProperty>(
        IEnumerable<TEntity> entities,
        Expression<Func<TEntity, IEnumerable<TProperty>>> navigationProperty)
        where TProperty : class;

    /// <summary>Gets a queryable entity set without change tracking.</summary>
    public IQueryable<TEntity> GetTableNoTracking();
}