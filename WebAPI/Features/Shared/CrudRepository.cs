using Microsoft.EntityFrameworkCore;

namespace WebAPI.Shared;

public class CrudRepository<TEntity, TKey> where TEntity : class
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;
    
    public CrudRepository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public TEntity? FindById(TKey id)
    {
        return _dbSet.Find(id);
    }

    public virtual int Add(TEntity entity)
    {
        _dbSet.Add(entity);
        return _context.SaveChanges();
    }

    public virtual int Update(TEntity entity)
    {
        _dbSet.Update(entity);
        return _context.SaveChanges();
    }

    public virtual int Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
        return _context.SaveChanges();
    }
}