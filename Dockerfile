# Imagem da API PetCare Hub (.NET 9) para hospedagem em container.
#
# O Render nao tem runtime nativo de .NET: e por este arquivo que ele sabe
# construir e iniciar a aplicacao. Mesmo build serve para rodar local:
#   docker build -t petcarehub-api .
#   docker run -p 8080:8080 -e ConnectionStrings__DefaultConnection="..." petcarehub-api

# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Restaura primeiro so com os .csproj. Enquanto as dependencias nao mudam, o
# Docker reaproveita esta camada e o deploy cai de minutos para segundos.
COPY PetCareHub.Domain/PetCareHub.Domain.csproj PetCareHub.Domain/
COPY PetCareHub.Application/PetCareHub.Application.csproj PetCareHub.Application/
COPY PetCareHub.Infrastructure/PetCareHub.Infrastructure.csproj PetCareHub.Infrastructure/
COPY PetCareHub.API/PetCareHub.API.csproj PetCareHub.API/
RUN dotnet restore PetCareHub.API/PetCareHub.API.csproj

COPY . .
RUN dotnet publish PetCareHub.API/PetCareHub.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# O Serilog grava em logs/ e o container roda sem privilegio: sem este diretorio
# com o dono certo, o sink de arquivo falha na inicializacao.
RUN mkdir -p /app/logs && chown -R $APP_UID /app/logs
USER $APP_UID

ENV ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_gcServer=0

EXPOSE 8080

# O Render injeta PORT em tempo de execucao e espera que a aplicacao escute
# nela; fora do Render, 8080 e o padrao. O shell e necessario justamente para
# expandir PORT, e o exec mantem o dotnet como PID 1 para receber o SIGTERM.
ENTRYPOINT ["/bin/sh", "-c", "exec dotnet PetCareHub.API.dll --urls http://0.0.0.0:${PORT:-8080}"]
