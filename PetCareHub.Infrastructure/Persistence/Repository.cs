using Microsoft.EntityFrameworkCore;
using PetCareHub.Application.Repositories;

namespace PetCareHub.Infrastructure.Persistence;

public abstract class Repository<T>(PetCareHubContext context) : IRepository<T> where T : class
{
    protected readonly PetCareHubContext _context = context;

    public virtual IEnumerable<T> GetAll() =>
        _context.Set<T>().AsNoTracking().ToList();

    public virtual T? GetById(long id) =>
        _context.Set<T>().Find(id);

    public virtual void Add(T entity)
    {
        _context.Set<T>().Add(entity);
        _context.SaveChanges();
    }

    public virtual void Update(T entity)
    {
        _context.Set<T>().Update(entity);
        _context.SaveChanges();
    }

    public virtual bool Delete(long id)
    {
        var entity = _context.Set<T>().Find(id);
        if (entity is null)
            return false;

        _context.Set<T>().Remove(entity);
        _context.SaveChanges();
        return true;
    }

    // FIX bug Oracle EF Core 9.23 — não usar .Any(), usar Count() > 0
    public virtual bool Exists(long id) =>
        _context.Set<T>()
            .Count(e => EF.Property<long>(e, "Id") == id) > 0;
}