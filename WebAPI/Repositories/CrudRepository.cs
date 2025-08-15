using Microsoft.EntityFrameworkCore;

namespace WebAPI.Repositories;

public class CrudRepository<TEntity, TKey> where TEntity : class
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public CrudRepository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public TEntity FindById(TKey id)
    {
        return _dbSet.Find(id) ?? throw new Exception();
    }

    public virtual TEntity Add(TEntity entity)
    {
        _dbSet.Add(entity);
        _context.SaveChanges();
        return entity;
    }
}