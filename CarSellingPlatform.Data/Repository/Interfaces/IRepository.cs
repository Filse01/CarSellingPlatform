using System.Linq.Expressions;

namespace CarSellingPlatform.Data.Interfaces.Repository;

public interface IRepository<TEntity, TKey>
{
    TEntity? GetById(TKey id);
    Task<TEntity?> GetByIdAsync(TKey id);
    TEntity? SingleOrDefault(Func<TEntity, bool> predicate);
    Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    TEntity? FirstOrDefault(Func<TEntity, bool> predicate);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    IEnumerable<TEntity> GetAll();
    Task<IEnumerable<TEntity>> GetAllAsync();
    IQueryable<TEntity> GetAllAttached();
    void Add(TEntity entity);
    Task AddAsync(TEntity entity);
    void AddRange(TEntity[] entities);
    Task AddRangeAsync(TEntity[] entities);
    bool Delete(TEntity entity);
    bool HardDelete(TEntity entity);
    bool Update(TEntity entity);
    Task<bool> UpdateAsync(TEntity entity);
  void SaveChanges();
    Task SaveChangesAsync();
}