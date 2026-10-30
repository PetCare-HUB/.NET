using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PetCareHub.Application.DTOs.Paging;
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
        return _context.Consultar<T>().AsNoTracking().ToList();
    }

    public virtual PagedResult<T> GetPaged(PageQuery consulta)
    {
        using var activity = StartActivity("GetPaged");
        activity?.SetTag("db.page", consulta.Page);
        activity?.SetTag("db.size", consulta.Size);
        activity?.SetTag("db.sort", consulta.Sort);

        var query = _context.Consultar<T>().AsNoTracking();

        // Contagem antes do recorte: o cliente precisa do total do filtro,
        // nao do tamanho da pagina.
        var total = query.LongCount();

        var itens = Ordenar(query, consulta)
            .Skip(consulta.Skip)
            .Take(consulta.Size)
            .ToList();

        return new PagedResult<T>(itens, total);
    }

    /// <summary>
    /// Ordena por nome de propriedade vindo da query string. O nome e resolvido
    /// por reflection contra a entidade: se nao existir, cai para Id. Como a
    /// ordenacao vira expression tree, nunca ha concatenacao de SQL.
    /// </summary>
    private static IQueryable<T> Ordenar(IQueryable<T> query, PageQuery consulta)
    {
        var propriedade =
            typeof(T).GetProperty(consulta.Sort ?? string.Empty,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
            ?? typeof(T).GetProperty("Id",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (propriedade is null)
            return query;

        var parametro = Expression.Parameter(typeof(T), "e");
        var acesso = Expression.Property(parametro, propriedade);
        var seletor = Expression.Lambda(acesso, parametro);

        var metodo = consulta.Descending ? "OrderByDescending" : "OrderBy";
        var chamada = Expression.Call(
            typeof(Queryable),
            metodo,
            [typeof(T), propriedade.PropertyType],
            query.Expression,
            Expression.Quote(seletor));

        return query.Provider.CreateQuery<T>(chamada);
    }

    public virtual T? GetById(long id)
    {
        using var activity = StartActivity("GetById");
        activity?.SetTag("db.entity.id", id);
        return _context.Consultar<T>()
            .FirstOrDefault(entity => EF.Property<long>(entity, "Id") == id);
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

        var entity = _context.Consultar<T>()
            .FirstOrDefault(candidate => EF.Property<long>(candidate, "Id") == id);
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
        return _context.Consultar<T>()
            .Count(e => EF.Property<long>(e, "Id") == id) > 0;
    }
}
