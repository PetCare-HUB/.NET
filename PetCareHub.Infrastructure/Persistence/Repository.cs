using Microsoft.EntityFrameworkCore;
using PetCareHub.Application.Repositories;

namespace PetCareHub.Infrastructure.Persistence;

public class Repository<TEntity>(PetCareHubContext context) 
    : IRepository<TEntity> where TEntity : class
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
    protected readonly PetCareHubContext Context = context;

    public IEnumerable<TEntity> GetAll() => 
        _dbSet.AsNoTracking().ToList();

    public TEntity? GetById(long id) => 
        _dbSet.Find(id);

    public void Add(TEntity entity)
    {
        _dbSet.Add(entity);
        Context.SaveChanges();
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
        Context.SaveChanges();
    }

    public bool Delete(long id)
    {
        var entity = GetById(id);
        if (entity is null)
            return false;

        _dbSet.Remove(entity);
        Context.SaveChanges();
        return true;
    }

    public bool Exists(long id) => 
        _dbSet.Find(id) is not null;
}