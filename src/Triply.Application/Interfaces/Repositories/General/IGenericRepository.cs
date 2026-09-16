using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Triply.Application.Interfaces.Repositories.General;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<ICollection<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken);
    Task AddRangeAsync(ICollection<TEntity> entities);
    Task UpdateAsync(TEntity entity);
    Task UpdateRangeAsync(ICollection<TEntity> entities);
    Task DeleteAsync(TEntity entity);
    Task DeleteRangeAsync(ICollection<TEntity> entities);
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitAsync();
    Task RollBackAsync();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task LoadAllCollectionsAsync(TEntity entity);
    Task LoadReferenceAsync<TProperty>(
        TEntity entity,
        Expression<Func<TEntity, TProperty?>> navigationProperty) where TProperty : class;

    Task LoadCollectionAsync<TProperty>(
        TEntity entity,
        Expression<Func<TEntity, IEnumerable<TProperty>>> navigationProperty)
        where TProperty : class;

    Task LoadCollectionAsync<TProperty>(
        IEnumerable<TEntity> entities,
        Expression<Func<TEntity, IEnumerable<TProperty>>> navigationProperty)
        where TProperty : class;
}