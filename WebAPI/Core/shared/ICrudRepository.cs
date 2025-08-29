using WebAPI.Annotation;

namespace WebAPI.Core.shared;

public interface ICrudRepository<TEntity, TKey> where TEntity : class
{
    // -- SYNCHRONOUS -- 
    public ICollection<TEntity> getAll();
    public TEntity? findById(TKey id);
    public Int32 add(TEntity entity);
    public Int32 update(TEntity entity);
    public Int32 delete(TEntity entity);
    
    // -- ASYNC -- 
    public Task<ICollection<TEntity>> getAllAsync();
    public Task<TEntity?> findByIdAsync(TKey id);
    public Task<Int32> addAsync(TEntity entity);
    public Task<Int32> updateAsync(TEntity entity);
    public Task<Int32> deleteAsync(TEntity entity);
}