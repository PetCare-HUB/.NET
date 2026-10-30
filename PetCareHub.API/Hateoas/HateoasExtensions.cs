using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PetCareHub.Application.DTOs.Hateoas;
using PetCareHub.Application.DTOs.Paging;

namespace PetCareHub.API.Hateoas;

/// <summary>
/// Monta os links HATEOAS das respostas de consulta.
/// </summary>
/// <remarks>
/// As URLs saem do próprio request (esquema, host e PathBase), então
/// continuam corretas atrás do proxy do Render sem precisar de URL base
/// configurada em lugar nenhum.
/// </remarks>
public static class HateoasExtensions
{
    /// <summary>Raiz do controller atual, ex.: <c>https://host/api/Clinicas</c>.</summary>
    private static string Raiz(this ControllerBase controller)
    {
        var request = controller.Request;
        var nome = controller.ControllerContext.ActionDescriptor.ControllerName;
        return $"{request.Scheme}://{request.Host}{request.PathBase}/api/{nome}";
    }

    /// <summary>
    /// Devolve a página com os links de navegação: self, first, prev, next e last.
    /// Os filtros da query string atual são preservados em cada link — paginar
    /// não pode fazer o cliente perder o filtro que ele aplicou.
    /// </summary>
    public static PagedResponse<T> ComLinksDePagina<T>(
        this ControllerBase controller,
        PagedResponse<T> pagina)
    {
        var raiz = controller.Raiz();

        string Href(int numeroPagina)
        {
            var parametros = controller.Request.Query
                .ToDictionary(item => item.Key, item => (string?)item.Value.ToString());
            parametros["page"] = numeroPagina.ToString();
            parametros["size"] = pagina.Size.ToString();
            return QueryHelpers.AddQueryString(raiz, parametros);
        }

        var links = new List<LinkDto> { new("self", Href(pagina.Page), "GET") };

        if (pagina.TotalPages > 0)
        {
            links.Add(new LinkDto("first", Href(0), "GET"));

            if (!pagina.First)
                links.Add(new LinkDto("prev", Href(pagina.Page - 1), "GET"));

            if (!pagina.Last)
                links.Add(new LinkDto("next", Href(pagina.Page + 1), "GET"));

            links.Add(new LinkDto("last", Href(pagina.TotalPages - 1), "GET"));
        }

        return pagina.ComLinks(links);
    }

    /// <summary>
    /// Envelopa um recurso único com os links do que dá para fazer com ele.
    /// </summary>
    /// <typeparam name="T">Tipo do DTO de resposta.</typeparam>
    /// <param name="controller">Controller que está respondendo, usado para montar a URL.</param>
    /// <param name="dados">O recurso a ser envelopado.</param>
    /// <param name="id">Identificador do recurso.</param>
    /// <param name="somenteLeitura">
    /// Controllers que não expõem escrita (Pets, Tutores e Scores, cujo cadastro
    /// é da API Java) não devem anunciar update/delete que responderiam 405.
    /// </param>
    public static Resource<T> ComoRecurso<T>(
        this ControllerBase controller,
        T dados,
        long id,
        bool somenteLeitura = false)
    {
        var raiz = controller.Raiz();

        var links = new List<LinkDto>
        {
            new("self", $"{raiz}/{id}", "GET"),
            new("collection", raiz, "GET")
        };

        if (!somenteLeitura)
        {
            links.Add(new LinkDto("update", $"{raiz}/{id}", "PUT"));
            links.Add(new LinkDto("delete", $"{raiz}/{id}", "DELETE"));
        }

        return new Resource<T>(dados, links);
    }
}
