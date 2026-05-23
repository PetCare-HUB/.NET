namespace PetCareHub.Application.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    IEnumerable<TEntity> GetAll();

    TEntity? GetById(long id);
    
    void Add(TEntity entity);
    
    void Update(TEntity entity);
    
    bool Delete(long id);
    
    bool Exists(long id);
}