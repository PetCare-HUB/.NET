using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace PetCareHub.Tests.Integration;

/// <summary>
/// Gera tokens JWT (RS256) para os testes de integração, assinados com a mesma chave de
/// teste que a API valida por padrão (<c>PetCareHub.API/Keys/public_key.pem</c>) — que também
/// é a chave de teste do lado Java (<c>src/test/resources/Keys</c>). Nunca usar a chave real
/// de produção aqui: isso é só para simular, nos testes, o token que o Java emitiria.
/// </summary>
public static class TestTokenFactory
{
    private const string PrivateKeyPem = """
        -----BEGIN PRIVATE KEY-----
        MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDtdORm29KvLwa1
        9Sd1J7CKma9SGUisE/fCx+tQmBqaKwGVav/zt4gfn4mbrVb0zURl3yQiYnHxndEb
        YjwIBYxcKFiq3DVWTUtUvUyWOm7Xe5YGbiLb6CRT0uJAK8tc6zBBqW3CGI4l2PUe
        QQ401HVs3jA/5kzlaZcTzeAsZY8lWRORILOUjxcRwJRawBaG0Iel156Z076fA1kG
        3Ha4b+qYLQn0darsCIOe/LIW+KCoBs0Q7OgC9Z1unHGCTYwtln1THZzaxi+IM+J8
        ttZhvO2iq6DljmkfpSPjEZju/kDjhdAD2V9glepcKaIufvNT5SotG/HrVQRRxJjm
        +Iq6Lb6dAgMBAAECggEAB/ML2m2thfPbE6hbY86qWFmAxwzVo/0fJEQxGoedDGA4
        M1pbqzsPsY9TugR5jsR4b7iiqwwEqIQSBgqp29LjugoXvdCVcIgx52pW8AiQwWH/
        ycNSI26IGM5MiL1ul3PAL9KOPBs6VwYGXrbq1NXwAG8HynvZQyzfw+f2g0X7HtLm
        FLM7l2ztAXaDSo7H1e8BnaTec2ushwBz5ziCRlhzCafoop3+fXG3XggQ98iUpfjn
        YgJuwkERFQhuJeviFmQmRImYZMkq9TWoQt5DPIcI5fiUl4auDMX873Vc5WN8iOMj
        7EAFTk+S01GhwKIvfITq5puJBZDb7rhvmQLtaMcaOQKBgQD28mKyoHbZov/nh/2P
        tT1W8R4o5TDRi+/8FGIQWbMIYPkRVVEYbppVKcUraHBDm1CFFjveS9iqJ9xq182K
        DWtlwbNRGhBp5DagWSSDEU+hPrRjPBAQ49zg8njQEMCwr54ol6O96fZBRQ2wgA9Z
        dm623hxlxW1x4dhZUyxmH2PwIwKBgQD2KXC57yI5a4quGToyQfVBbqilgupXaNbQ
        95BlGoLZGifNY1QMms9mQuBdZNeoINVkz06zwd5AvdKOrRA53hlHj5B1evxHHQiF
        hQ52dbvGjkPzrGD9hNIYw/1SsspKYiYZ15A6RyXyB2uhI9M7vL6govVSnLNbaZQ7
        rLTB7RsiPwKBgA5CIIC/0xzt1GjB6VoK1OfYNC2YTiFLOygK5T6cb0Hkl3zwuCX4
        /OA98Sx4c0L0DMoiZoKHTpNEie2BWFTIQZM2g+wC3T+/9Yd8nicTMM3WmQPBzAut
        nGtAlMkurnGFMlSVrNaiU52hJxEx07EOrYXRfUJDC/avW5aMvtuGpHaRAoGBALS3
        TaOjFHwVIHWZ3aih1ZzY0YTY7JOJSxOUe/RNlzwtQIU/Y6fxS0um0zKdYtlyaVcd
        /ohnTnQ3J0pJcX0aOXLYydQFmwnWHbhwK2L6IgWt0eMlbPsRtHAsCmDZEzuyfIhC
        QgcWzpu1qOVe+KeFdGlX2URx+BanFwUMUJOCL4ZfAoGBAJ45S4cAT8daG1kbELpD
        /+uKAcbL+wh3OqUINyZP03y/nBDN9lDQGDF0TraLLWzhiR3fucJord17459Ut3yv
        YLgdJFOFrGVuSn6N9xDI04GoiUxPuw8rkgZGnVHAjQRtQ60Qq4nzRQAlctIL/wsk
        LaDcRO35pJW5tCcCmXZV/cKm
        -----END PRIVATE KEY-----
        """;

    public static string CreateToken(
        string role,
        long? clinicaId = null,
        long? tutorId = null,
        string subject = "teste@petcare.com",
        TimeSpan? lifetime = null,
        DateTime? notBefore = null)
    {
        // Sem "using": o RSA precisa continuar vivo até o WriteToken assinar de fato — em
        // alguns ambientes o handle nativo (RSABCrypt) já aparecia disposed nesse ponto com
        // "using var". É um objeto de teste, de vida curta; o GC recolhe sem problema.
        var rsa = RSA.Create();
        rsa.ImportFromPem(PrivateKeyPem);
        var credentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, subject),
            new("role", role)
        };

        if (clinicaId.HasValue)
            claims.Add(new Claim("clinicaId", clinicaId.Value.ToString()));

        if (tutorId.HasValue)
            claims.Add(new Claim("tutorId", tutorId.Value.ToString()));

        var inicio = notBefore ?? DateTime.UtcNow;
        var token = new JwtSecurityToken(
            issuer: "petcare-hub-api",
            claims: claims,
            notBefore: inicio,
            expires: inicio.Add(lifetime ?? TimeSpan.FromMinutes(30)),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>Token de clínica já vencido — para testar a rejeição por lifetime expirado.</summary>
    public static string CreateExpiredToken(string role = "CLINICA", long clinicaId = 1) =>
        CreateToken(
            role,
            clinicaId,
            notBefore: DateTime.UtcNow.AddMinutes(-30),
            lifetime: TimeSpan.FromMinutes(5)); // expira 25 min atrás, começou a valer 30 min atrás
}
