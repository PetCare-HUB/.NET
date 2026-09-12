using Microsoft.EntityFrameworkCore;
using PetCareHub.Application.Repositories;
using PetCareHub.Infrastructure.Diagnostics;

namespace PetCareHub.Infrastructure.Persistence;

public abstract class Repository<T>(PetCareHubContext context) : IRepository<T> where T : class
{
    protected readonly PetCareHubContext _context = context;

    private static readonly string EntityName = typeof(T).Name;

    private static System.Diagnostics.Activity? StartActivity(string operation)
    {
        var activity = InfraTelemetry.Source.StartActivity($"{EntityName}.{operation}");
        activity?.SetTag("db.system", "oracle");
        activity?.SetTag("db.entity", EntityName);
        activity?.SetTag("db.operation", operation);
        return activity;
    }

    public virtual IEnumerable<T> GetAll()
    {
        using var activity = StartActivity("GetAll");
        return _context.Set<T>().AsNoTracking().ToList();
    }

    public virtual T? GetById(long id)
    {
        using var activity = StartActivity("GetById");
        activity?.SetTag("db.entity.id", id);
        return _context.Set<T>().Find(id);
    }

    public virtual void Add(T entity)
    {
        using var activity = StartActivity("Add");
        _context.Set<T>().Add(entity);
        _context.SaveChanges();
    }

    public virtual void Update(T entity)
    {
        using var activity = StartActivity("Update");
        _context.Set<T>().Update(entity);
        _context.SaveChanges();
    }

    public virtual bool Delete(long id)
    {
        using var activity = StartActivity("Delete");
        activity?.SetTag("db.entity.id", id);

        var entity = _context.Set<T>().Find(id);
        if (entity is null)
            return false;

        _context.Set<T>().Remove(entity);
        _context.SaveChanges();
        return true;
    }

    // FIX bug Oracle EF Core 9.23 — não usar .Any(), usar Count() > 0
    public virtual bool Exists(long id)
    {
        using var activity = StartActivity("Exists");
        activity?.SetTag("db.entity.id", id);
        return _context.Set<T>()
            .Count(e => EF.Property<long>(e, "Id") == id) > 0;
    }
}