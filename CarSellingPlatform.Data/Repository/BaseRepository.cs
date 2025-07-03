using System.Linq.Expressions;
using System.Reflection;
using CarSellingPlatform.Data.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CarSellingPlatform.Data.Repository;

public class BaseRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity: class
{
    private readonly CarSellingPlatformDbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;
    public BaseRepository(CarSellingPlatformDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<TEntity>();
    }
    public TEntity? GetById(TKey id)
    {
        return _dbSet.Find(id);
    }

    public async Task<TEntity?> GetByIdAsync(TKey id)
    {
        return await _dbSet.FindAsync(id);
    }

    public TEntity? SingleOrDefault(Func<TEntity, bool> predicate)
    {
        return _dbSet.SingleOrDefault(predicate);
    }

    public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.SingleOrDefaultAsync(predicate);
    }

    public TEntity? FirstOrDefault(Func<TEntity, bool> predicate)
    {
        return _dbSet.FirstOrDefault(predicate);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public IEnumerable<TEntity> GetAll()
    {
        return _dbSet.ToArray();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public IQueryable<TEntity> GetAllAttached()
    {
        return _dbSet.AsQueryable();
    }

    public void Add(TEntity entity)
    {
        this._dbSet.Add(entity);
        this._dbContext.SaveChanges();
    }

    public async Task AddAsync(TEntity entity)
    {
        await this._dbSet.AddAsync(entity);
        await this._dbContext.SaveChangesAsync();
    }

    public void AddRange(TEntity[] entities)
    {
        this._dbSet.AddRange(entities);
        this._dbContext.SaveChanges();
    }

    public async Task AddRangeAsync(TEntity[] entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await this._dbContext.SaveChangesAsync();
    }

    public bool Delete(TEntity entity)
    {
        PerformSoftDeleteOfEntity(entity);
        return Update(entity);
    }

    public bool HardDelete(TEntity entity)
    {
        _dbSet.Remove(entity);
        int updatedCount = this._dbContext.SaveChanges();
        return updatedCount > 0;
    }
    

    public bool Update(TEntity entity)
    {
        try
        {
            _dbSet.Attach(entity);
            _dbSet.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(TEntity entity)
    {
        try
        {
            _dbSet.Attach(entity);
            _dbSet.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void SaveChanges()
    {
        this._dbContext.SaveChanges();
    }

    public async Task SaveChangesAsync()
    {
        await this._dbContext.SaveChangesAsync();
    }

    private void PerformSoftDeleteOfEntity(TEntity entity)
    {
        PropertyInfo? isDeletedProperty = GetIsDeletedProperty(entity);
        if (isDeletedProperty == null)
        {
            throw new InvalidOperationException($"Entity {entity} is not deleted");
        }
        isDeletedProperty.SetValue(entity, true);
    }

    private PropertyInfo? GetIsDeletedProperty(TEntity entity)
    {
        return typeof(TEntity)
            .GetProperties()
            .FirstOrDefault(pi => pi.PropertyType == typeof(bool) && pi.Name=="IsDeleted");
    }
}