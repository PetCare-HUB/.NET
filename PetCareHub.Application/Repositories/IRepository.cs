using PetCareHub.Application.DTOs.Paging;

namespace PetCareHub.Application.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    IEnumerable<TEntity> GetAll();

    /// <summary>
    /// Uma página de registros mais o total do filtro. A contagem e o recorte
    /// acontecem no banco — nunca traz a tabela inteira para a memória.
    /// </summary>
    PagedResult<TEntity> GetPaged(PageQuery consulta);

    TEntity? GetById(long id);
    
    void Add(TEntity entity);
    
    void Update(TEntity entity);
    
    bool Delete(long id);
    
    bool Exists(long id);
}