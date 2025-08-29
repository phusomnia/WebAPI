using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using WebAPI.Annotation;
using WebAPI.Infrastructure.Data;

namespace WebAPI.Core.shared;

public class CrudRepository<TEntity, TKey> : ICrudRepository<TEntity, TKey> where TEntity : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<TEntity> _dbSet;
    
    public CrudRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }
    
    // -- SYNCHRONOUS --
    public ICollection<Dictionary<String, Object>> executeSqlRaw(String query, params Object[] parameters)
    {
        return _context.executeSqlRaw(query, parameters);
    }
    
    public ICollection<TEntity> getAll()
    {
        return _context.Set<TEntity>().ToList();
    }

    public TEntity? findById(TKey id)
    {
        return _dbSet.Find(id);
    }

    public Int32 add(TEntity entity)
    {
        _dbSet.Add(entity);
        return _context.SaveChanges();
    }

    public Int32 update(TEntity entity)
    {
        _dbSet.Update(entity);
        return _context.SaveChanges();
    }

    public Int32 delete(TEntity entity)
    {
        _dbSet.Remove(entity);
        return _context.SaveChanges();
    }
    
    // -- ASYNCHRONOUS -- 
    
    public async Task<ICollection<Dictionary<String, Object>>> executeSqlRawAsync(String query, params Object[] parameters)
    {
        return await _context.executeSqlRawAsync(query, parameters);
    }
    
    public async Task<ICollection<TEntity>> getAllAsync()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    public async Task<TEntity?> findByIdAsync(TKey id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<Int32> addAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        return await _context.SaveChangesAsync();
    }

    public async Task<Int32> updateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        return await _context.SaveChangesAsync();
    }

    public async Task<Int32> deleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        return await _context.SaveChangesAsync();
    }
}