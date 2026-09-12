# PetCare Hub — API .NET Dashboard Clínico

API RESTful desenvolvida em **.NET 9** para a disciplina **Advanced Business Development with .NET**, dentro do Challenge FIAP 2026 — CLYVO VET.

A API representa o **dashboard B2B da clínica veterinária**, permitindo consultar informações clínicas, pets, alertas de saúde, consultas, scores e indicadores operacionais.

---

## Contexto do Projeto

O Challenge CLYVO VET propõe transformar a jornada de saúde animal de um modelo reativo e fragmentado para uma experiência contínua, preventiva, inteligente e integrada.

Dentro dessa solução, o **PetCare Hub** atua como uma plataforma de acompanhamento contínuo da saúde do pet.

A API .NET tem como foco o painel da clínica, permitindo que profissionais acompanhem:

- Pets vinculados à clínica;
- Pets em risco;
- Alertas de saúde abertos;
- Consultas realizadas;
- Scores de saúde;
- Eventos preventivos;
- Indicadores do dashboard clínico.

---

## Objetivo da API .NET

A API .NET é responsável por servir os dados do **dashboard da clínica veterinária**, consumindo o banco Oracle modelado na disciplina de Database.

A aplicação permite:

- Gerenciar clínicas via CRUD completo;
- Consultar pets vinculados à clínica (somente leitura);
- Gerenciar consultas via CRUD completo (com filtros por clínica, pet, tipo e retorno);
- Gerenciar alertas de saúde via CRUD completo (com filtros por pet, nível e situação);
- Resolver alertas;
- Consultar tutores dos pets;
- Consultar scores de saúde com filtros por categoria e faixa de score;
- Exibir métricas consolidadas no dashboard.

---

## Decisões de Arquitetura

Esta API é o **dashboard B2B** da clínica veterinária parceira. O ciclo de vida dos dados é dividido entre as APIs da plataforma PetCare Hub:

- **Pets, Tutores e Scores de Saúde** são expostos **apenas para leitura** nesta API. O cadastro, atualização e remoção desses recursos pertence à API Java (consumida pelo tutor através do app mobile), que é a fonte de verdade dos dados do animal e do seu tutor. Os Scores de Saúde são calculados pela API Java a partir das leituras IoT e gravados no banco Oracle compartilhado.
- **Clínicas, Consultas e Alertas de Saúde** possuem **CRUD completo** nesta API, pois representam operações realizadas pela equipe da clínica no seu dia a dia (cadastrar uma clínica parceira, registrar uma consulta, abrir ou resolver um alerta).

Essa separação garante que cada API tenha responsabilidade clara sobre o seu domínio e evita duplicação de regras de negócio entre Java e .NET.

---

## Autenticação

O .NET **nunca emite token** — quem faz login (e-mail/senha) é a API Java, via `POST /auth/login`.
O .NET só **valida** esse mesmo token: confere a assinatura RSA (RS256), o emissor
(`petcare-hub-api`), a validade e a claim `role`.

Todos os endpoints de dados (`Clinicas`, `Consultas`, `AlertasSaude`, `Pets`, `ScoresSaude`,
`Tutores`, `Dashboard`) exigem um `Authorization: Bearer <token>` com `role = CLINICA` — é
coerente com o resto do README: esta API é o dashboard B2B da clínica, não existe fluxo de
tutor aqui. Só `/health`, `/health/live` e `/health/ready` ficam públicos (é o que uma
ferramenta de monitoramento externa chama, sem credencial nenhuma).

| Situação | Resposta |
|---|---|
| Sem header `Authorization` | `401 Unauthorized` |
| Token malformado, assinatura inválida ou expirado | `401 Unauthorized` |
| Token válido, mas `role` diferente de `CLINICA` (ex.: `TUTOR`) | `403 Forbidden` |
| Token válido com `role = CLINICA` | segue normalmente |

### Como conseguir um token para testar

1. Suba a API Java localmente (ela cria a clínica de exemplo automaticamente em banco vazio —
   ver o `DataLoader` no README do Java) ou use as credenciais de uma clínica já cadastrada no
   Oracle compartilhado.
2. Chame `POST /auth/login` na API Java (Swagger em `/swagger-ui.html`) com o e-mail/senha da
   clínica.
3. Use o `token` da resposta como Bearer nas chamadas ao Swagger desta API .NET (botão
   **"Authorize"**).

### Configuração da chave pública

A assinatura é validada com a mesma chave RSA pública que o Java usa para assinar (nunca a
chave privada — o .NET só verifica, não emite). Por padrão, a API usa a chave de
desenvolvimento/teste commitada em `PetCareHub.API/Keys/public_key.pem` (a mesma usada pela
suíte de testes do lado Java). Em produção, sobrescreva com a chave pública real via variável
de ambiente:

```txt
Jwt__PublicKeyPem="-----BEGIN PUBLIC KEY-----\n...\n-----END PUBLIC KEY-----"
```

> **Importante:** essa variável precisa conter exatamente a mesma chave pública configurada
> como `RSA_PUBLIC_KEY` na API Java — se as chaves não baterem, todo token é rejeitado com
> `401`, mesmo sendo um token real e válido emitido pelo Java.

---

## Tecnologias Utilizadas

- .NET 9
- ASP.NET Core Web API (Controllers)
- Entity Framework Core 9
- Oracle.EntityFrameworkCore 9.23
- Oracle Database FIAP
- JWT Bearer (RS256) — valida os mesmos tokens emitidos pela API Java, não emite token próprio
- Swagger / OpenAPI (Swashbuckle)
- Serilog (console + arquivo, com correlation id) — logging estruturado
- OpenTelemetry (tracing + métricas via console exporter)
- AspNetCore.HealthChecks.Oracle — monitoramento de saúde do banco
- xUnit + Moq — testes automatizados (unitários e de integração)
- C# 13 (records, primary constructors)
- Git e GitHub

---

## Arquitetura do Projeto

O projeto foi organizado em camadas, seguindo Clean Architecture:

```txt
PetCareHub
├── PetCareHub.API                 ← Endpoints HTTP (Controllers + Swagger)
│   ├── Controllers
│   ├── Exceptions                 ← GlobalExceptionHandler
│   ├── Extensions                 ← Registro de serviços
│   ├── Health                     ← Resposta JSON do /health
│   ├── Program.cs
│   └── appsettings.json
│
├── PetCareHub.Application         ← Casos de uso (Services) + Contratos
│   ├── DTOs                       ← Requests + Responses (records)
│   ├── Repositories               ← Interfaces de repositório
│   └── Services
│       ├── Interfaces
│       └── Implementations
│
├── PetCareHub.Domain              ← Entidades de domínio, com as regras que dependem só
│   └── Entities                     do próprio estado da entidade (ex.: AlertaSaude.Resolver())
│
├── PetCareHub.Infrastructure      ← Persistência (EF Core + Oracle)
│   └── Persistence
│       ├── Configurations         ← Fluent API por entidade (mapeia o schema já criado pelo Flyway)
│       ├── Repositories
│       ├── PetCareHubContext.cs
│       └── Repository.cs          ← Repositório genérico
│
├── PetCareHub.Tests.Unit          ← Testes unitários (Moq, padrão AAA)
│   ├── Domain                     ← Entidades, sem mock nenhum (Resolver, GarantirQuePodeSerExcluida)
│   └── Services                   ← Orquestração dos Services (repositório mockado)
│
└── PetCareHub.Tests.Integration   ← Testes de integração via WebApplicationFactory
```

---

## Banco de Dados

A API utiliza o **mesmo schema Oracle que a API Java** (fonte de verdade compartilhada da disciplina de Database). O schema é gerenciado exclusivamente pelo **Flyway do Java** — o .NET nunca roda migration própria contra ele, só mapeia as tabelas já existentes via Fluent API.

Tabelas utilizadas pela API:

```txt
Classe C#          Tabela Oracle
Clinica            CLINICA
Tutor              TUTOR
Pet                PET
Consulta           CONSULTA
AlertaSaude        ALERTA_SAUDE
ScoreSaude         SCORE_SAUDE
EventoPreventivo   EVENTO_PREVENTIVO
ProtocoloPreventivo PROTOCOLO_PREVENTIVO
LeituraColeira     LEITURA_COLEIRA
LeituraComedouro   LEITURA_COMEDOURO
LeituraAmbiente    LEITURA_AMBIENTE
```

O mapeamento entre C# e Oracle é feito com **Fluent API** em `PetCareHub.Infrastructure/Persistence/Configurations`, respeitando os nomes reais das tabelas e colunas criadas pelo Flyway. Credenciais de login (`SENHA_HASH` em `TUTOR`/`CLINICA`) não são mapeadas pelo .NET — autenticação é responsabilidade exclusiva da API Java.

---

## Configuração do Banco Oracle

Por segurança, as credenciais reais **não devem ser versionadas** no GitHub.

A connection string padrão é lida da chave `ConnectionStrings:DefaultConnection`.

Crie o arquivo:

```txt
PetCareHub.API/appsettings.Development.json
```

Com o seguinte conteúdo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle.fiap.com.br)(PORT=1521))(CONNECT_DATA=(SID=orcl)))"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

`appsettings.Development.json` está no `.gitignore` — nunca foi (e não deve ser) commitado.

> **Importante para quem for entregar/empacotar este projeto:** gere o ZIP de entrega a
> partir do GitHub (botão **Code → Download ZIP** no repositório, ou um `git clone` limpo em
> outra pasta), nunca compactando a pasta local do seu computador direto. A pasta local tem
> `appsettings.Development.json` com a senha real do Oracle, além de `bin/`, `obj/`, `logs/`
> e configurações de IDE (`.idea/`, `.vs/`) — nenhum desses vai pro Git, mas todos vão junto
> se você simplesmente compactar a pasta como está no seu PC.

---

## Como Executar o Projeto

### 1. Clonar o repositório

```bash
git clone https://github.com/PetCare-HUB/.NET.git
cd .NET
```

### 2. Restaurar os pacotes

```bash
dotnet restore
```

### 3. Compilar o projeto

```bash
dotnet build
```

> O schema Oracle já existe e é gerenciado pelo Flyway da API Java — não é preciso (nem
> recomendado) rodar nenhuma migration do .NET. Veja a seção [Migrations](#migrations) abaixo.

### 4. Executar a API

```bash
dotnet run --project PetCareHub.API
```

A aplicação será iniciada em:

```txt
http://localhost:5062
```

### 5. Acessar o Swagger

O Swagger é servido na **raiz** da aplicação (não em `/swagger`), pois `RoutePrefix` está configurado como vazio.

Abra no navegador:

```txt
http://localhost:5062/
```

---

## Migrations

Na Sprint 2, o schema Oracle deste projeto foi criado e evoluído com **Entity Framework Core
Migrations** próprias do .NET (`dotnet ef migrations add` / `dotnet ef database update`).

A partir da Sprint 3, com a integração real com a API Java, ficou definido que o schema Oracle é
**compartilhado** entre as duas APIs — e que o **Flyway do Java** é quem gerencia esse schema
(criação de tabelas, renomeações, novas colunas). Manter duas ferramentas de migration diferentes
alterando o mesmo banco gera risco real de conflito (uma tabela renomeada de um lado sem o outro
lado saber, por exemplo).

Por isso, o .NET **não usa mais Migrations própria** a partir desta sprint: o `PetCareHub.Infrastructure/Persistence/Configurations`
apenas mapeia, via Fluent API, o schema que o Flyway do Java já criou. Não existe mais `dotnet ef
migrations add` nem `dotnet ef database update` neste projeto — se o modelo mudar, o ajuste é só
nas classes de `Configurations`, nunca em uma migration.

---

## Endpoints Disponíveis

Todos os endpoints seguem o padrão REST e estão documentados no Swagger.

> **Todos os endpoints abaixo exigem `Authorization: Bearer <token>` com `role = CLINICA`**
> (ver [Autenticação](#autenticação)). Isso não está repetido em cada tabela para não poluir —
> vale para Clínicas, Pets, Consultas, Alertas de Saúde, Scores de Saúde, Tutores e Dashboard.

### 🏥 Clínicas (CRUD completo)

| Método | Rota | Descrição |
|---|---|---|
| `GET`    | `/api/Clinicas`              | Lista todas as clínicas |
| `GET`    | `/api/Clinicas/{id}`         | Busca uma clínica pelo ID |
| `POST`   | `/api/Clinicas`              | Cria uma nova clínica |
| `PUT`    | `/api/Clinicas/{id}`         | Atualiza uma clínica |
| `DELETE` | `/api/Clinicas/{id}`         | Remove uma clínica (se não tiver pets vinculados) |

**Exemplo POST:**

```json
{
  "nome": "Clinica Teste DotNet",
  "cnpj": "99999999000199",
  "email": "teste@petcare.com",
  "telefone": "(11) 99999-9999",
  "endereco": "Rua Teste, 123",
  "ativo": true
}
```

> **Importante:** o campo `cnpj` deve ter exatamente **14 caracteres**, somente números (sem máscara).

---

### 🐶 Pets (somente leitura)

> Pets são gerenciados pela API Java (responsabilidade do tutor através do app mobile).
> A API .NET expõe apenas leitura, pois é o dashboard B2B da clínica — ela apenas
> consulta os pets dos clientes da clínica.

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/Pets`                          | Lista todos os pets |
| `GET` | `/api/Pets/{id}`                     | Busca pet pelo ID |
| `GET` | `/api/Pets/clinica/{clinicaId}`      | Lista pets vinculados a uma clínica |

---

### 🩺 Consultas (CRUD completo)

| Método | Rota | Descrição |
|---|---|---|
| `GET`    | `/api/Consultas`                       | Lista consultas (filtros opcionais) |
| `GET`    | `/api/Consultas/{id}`                  | Busca consulta pelo ID |
| `GET`    | `/api/Consultas/clinica/{clinicaId}`   | Lista consultas de uma clínica |
| `GET`    | `/api/Consultas/pet/{petId}`           | Lista consultas de um pet |
| `POST`   | `/api/Consultas`                       | Cria uma consulta |
| `PUT`    | `/api/Consultas/{id}`                  | Atualiza uma consulta |
| `DELETE` | `/api/Consultas/{id}`                  | Remove uma consulta |

**Filtros opcionais no GET:**

```
GET /api/Consultas?clinicaId=1
GET /api/Consultas?petId=1
GET /api/Consultas?tipoConsulta=CHECKUP
GET /api/Consultas?retornoRecomendado=true
```

**Exemplo POST:**

```json
{
  "petId": 1,
  "clinicaId": 1,
  "dataConsulta": "2026-05-24T10:00:00",
  "tipoConsulta": "CHECKUP",
  "descricao": "Consulta de rotina",
  "diagnostico": "Saudável",
  "valor": 150.00,
  "retornoRecomendado": true,
  "dataRetorno": "2026-08-24T10:00:00"
}
```

> **Importante:** `tipoConsulta` deve ser um dos valores: `CHECKUP`, `VACINA`, `EMERGENCIA`, `RETORNO` ou `EXAME`.

---

### 🚨 Alertas de Saúde (CRUD completo)

| Método | Rota | Descrição |
|---|---|---|
| `GET`    | `/api/AlertasSaude`                       | Lista alertas (filtros opcionais) |
| `GET`    | `/api/AlertasSaude/{id}`                  | Busca alerta pelo ID |
| `GET`    | `/api/AlertasSaude/pet/{petId}`           | Lista alertas de um pet |
| `GET`    | `/api/AlertasSaude/clinica/{clinicaId}`   | Lista alertas de uma clínica |
| `POST`   | `/api/AlertasSaude`                       | Cria um alerta |
| `PUT`    | `/api/AlertasSaude/{id}`                  | Atualiza um alerta |
| `PUT`    | `/api/AlertasSaude/{id}/resolver`         | Marca alerta como resolvido |
| `DELETE` | `/api/AlertasSaude/{id}`                  | Remove um alerta |

**Filtros opcionais no GET:**

```
GET /api/AlertasSaude?petId=1
GET /api/AlertasSaude?nivelAlerta=CRITICO
GET /api/AlertasSaude?resolvido=false
```

**Exemplo POST:**

```json
{
  "petId": 1,
  "tipoAlerta": "TEMPERATURA_AMBIENTE",
  "nivelAlerta": "ALTO",
  "mensagem": "Temperatura ambiente acima do recomendado para a raça",
  "valorDetectado": 32.5,
  "limiteReferencia": 28.0
}
```

> **Importante:** `nivelAlerta` deve ser um dos valores: `BAIXO`, `MEDIO`, `ALTO` ou `CRITICO`.

---

### 📊 Scores de Saúde (somente leitura)

> Os scores são calculados pela API Java a partir das leituras IoT (coleira,
> comedouro e sensores de ambiente). A API .NET apenas consulta os scores já
> persistidos no banco Oracle.

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/ScoresSaude`                          | Lista scores (filtros opcionais) |
| `GET` | `/api/ScoresSaude/{id}`                     | Busca score pelo ID |
| `GET` | `/api/ScoresSaude/pet/{petId}`              | Histórico de scores de um pet |
| `GET` | `/api/ScoresSaude/pet/{petId}/atual`        | Score atual (mais recente) do pet |
| `GET` | `/api/ScoresSaude/clinica/{clinicaId}`      | Scores dos pets de uma clínica |

**Filtros opcionais no GET:**

```
GET /api/ScoresSaude?petId=1
GET /api/ScoresSaude?clinicaId=1
GET /api/ScoresSaude?categoria=VERMELHO
GET /api/ScoresSaude?scoreMin=0&scoreMax=50
```

---

### 👤 Tutores (somente leitura)

> Tutores são cadastrados/ativados pela API Java (fluxo de primeiro acesso do
> app mobile). A clínica apenas consulta essa informação; a credencial
> (`senha_hash`) nunca é exposta pelo .NET.

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/Tutores`                       | Lista todos os tutores |
| `GET` | `/api/Tutores/{id}`                  | Busca tutor pelo ID |
| `GET` | `/api/Tutores/clinica/{clinicaId}`   | Lista tutores com pets na clínica |

---

### 📈 Dashboard Clínico

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/Dashboard/clinicas/{clinicaId}`                       | Resumo geral da clínica |
| `GET` | `/api/Dashboard/clinicas/{clinicaId}/pets-em-risco`         | Lista de pets em risco (categoria VERMELHO) |
| `GET` | `/api/Dashboard/clinicas/{clinicaId}/alertas-abertos`       | Alertas ainda não resolvidos |
| `GET` | `/api/Dashboard/clinicas/{clinicaId}/consultas-recentes`    | Últimas 10 consultas |
| `GET` | `/api/Dashboard/clinicas/{clinicaId}/eventos-pendentes`     | Eventos preventivos com status PENDENTE |

---

## Retornos HTTP Implementados

| Código | Significado |
|---|---|
| `200 OK` | Consulta realizada com sucesso |
| `201 Created` | Recurso criado com sucesso (POST) |
| `204 No Content` | Remoção ou ação executada sem corpo de resposta |
| `400 Bad Request` | Dados inválidos ou regra de negócio violada |
| `404 Not Found` | Recurso não encontrado |
| `500 Internal Server Error` | Erro inesperado (capturado pelo `GlobalExceptionHandler`) |

---

## Tratamento Global de Exceções

Implementado em `PetCareHub.API/Exceptions/GlobalExceptionHandler.cs` usando `IExceptionHandler`.

Mapeamento de exceções para códigos HTTP:

| Exceção C# | Código HTTP |
|---|---|
| `ArgumentException`, `ArgumentNullException` | 400 |
| `InvalidOperationException` | 400 |
| `KeyNotFoundException` | 404 |
| `UnauthorizedAccessException` | 401 |
| (qualquer outra) | 500 |

A resposta segue o padrão **RFC 7807 ProblemDetails**.

---

## Documentação Swagger

O Swagger é gerado automaticamente a partir dos comentários XML dos controllers e dos atributos `[ProducesResponseType]`.

Para acessar:

```txt
http://localhost:5062/
```

A documentação inclui:

- Resumo de cada endpoint;
- Parâmetros esperados;
- Códigos de retorno possíveis;
- Schemas dos DTOs.

Para testar pelo Swagger, basta usar o botão **"Try it out"** em cada endpoint. Os exemplos de JSON para `POST` e `PUT` estão documentados acima neste README e podem ser copiados e colados diretamente no campo "Request body".

---

## Monitoramento

A API separa **liveness** (o processo está de pé?) de **readiness** (as dependências externas
estão OK?) em três endpoints:

| Rota | O que verifica | Quando usar |
|---|---|---|
| `GET /health/live` | Só o próprio processo (`self`) — nunca toca no Oracle | Uma orquestração decidindo se precisa reiniciar o processo |
| `GET /health/ready` | Só as dependências externas — hoje, só o `oracle-db` | Decidir se a API está pronta para receber tráfego real |
| `GET /health` | Os dois checks juntos (`self` + `oracle-db`) | Compatibilidade / visão geral rápida |

> A **única** dependência externa que esta API chama é o Oracle da FIAP — não há fila,
> gateway de pagamento ou outra API de terceiro no caminho. Por isso só existe um
> dependency check (`oracle-db`); se um dia entrar uma integração nova (ex.: consumir a API
> Java), o check dela entraria em `/health/ready` também.

Formato de resposta (igual nos três endpoints):

```json
{
  "status": "Healthy",
  "duration": "00:00:00.3363583",
  "checks": [
    {
      "name": "self",
      "status": "Healthy",
      "description": "API em execução.",
      "duration": "00:00:00.0000010",
      "error": null
    },
    {
      "name": "oracle-db",
      "status": "Healthy",
      "description": null,
      "duration": "00:00:00.3339239",
      "error": null
    }
  ]
}
```

Quando o Oracle está inacessível ou as credenciais estão erradas, o check `oracle-db` (e por
consequência `/health` e `/health/ready`) volta como `Unhealthy` (HTTP 503), com o campo
`error` trazendo a mensagem original do banco (ex.: `ORA-01017: invalid username/password`).
`/health/live` continua `Healthy` nesse cenário — é exatamente o ponto de ele existir
separado: o processo está bem, só uma dependência dele que não está.

---

## Observabilidade

- **Logging estruturado**: [Serilog](https://serilog.net/) grava em dois destinos — console
  (com timestamp, nível e correlation id) e arquivo (`PetCareHub.API/logs/petcarehub-<data>.log`,
  um arquivo novo por dia). Toda requisição recebe um correlation id (via pacote
  [`CorrelationId`](https://github.com/stevejgordon/CorrelationId)), propagado nos logs pelo
  enricher `Serilog.Enrichers.CorrelationId`. Os `Services` de `Clinica`, `Consulta` e
  `AlertaSaude` logam os eventos de negócio mais relevantes (criação, atualização, bloqueios de
  regra de negócio); o `GlobalExceptionHandler` loga toda exceção não tratada.
- **Tracing e métricas**: [OpenTelemetry](https://opentelemetry.io/) instrumenta ASP.NET Core e
  `HttpClient` automaticamente (um span por requisição HTTP), exportando tudo no console via
  `AddConsoleExporter()` — não é necessário nenhum coletor externo (Jaeger/Zipkin/Prometheus)
  para visualizar os dados durante o desenvolvimento. Além disso, dois `ActivitySource`
  manuais criam spans **filhos** desse span de requisição, ligados a ele automaticamente
  pelo próprio `Activity.Current` do .NET:
  - `PetCareHub.Application` ([`AppTelemetry`](PetCareHub.Application/Diagnostics/AppTelemetry.cs)) — nas
    operações de negócio dos `Services` que fazem validação/gravação (`ClinicaService.Create/Update/Delete`,
    `ConsultaService.Create/Update`, `AlertaSaudeService.Create/Resolve`);
  - `PetCareHub.Infrastructure` ([`InfraTelemetry`](PetCareHub.Infrastructure/Diagnostics/InfraTelemetry.cs)) —
    em todo `GetAll/GetById/Add/Update/Delete/Exists` do `Repository<T>` genérico, cobrindo a
    persistência de qualquer entidade automaticamente.

  **Como ler tempo de resposta**: com o console exporter, cada span impresso mostra
  `Start Time` e `Duration` — a duração do span raiz (`Microsoft.AspNetCore.Hosting.HttpRequestIn`)
  é o tempo de resposta da requisição inteira; os spans filhos (`ClinicaService.Create`,
  `Clinica.Add`, etc.) mostram quanto desse tempo total foi gasto em regra de negócio vs.
  em banco — útil pra achar gargalo sem precisar de um APM externo.

  **Como calcular taxa de erro**: o exporter de métricas imprime periodicamente o histograma
  `http.server.request.duration`, que tem uma dimensão `http.response.status_code` por
  requisição. Taxa de erro = (soma das contagens com `status_code >= 500`) / (soma de todas
  as contagens) no período — como é console exporter, isso é uma conta manual sobre a saída
  impressa; para calcular automaticamente (ex.: alerta se erro > 5%), o próximo passo seria
  trocar `AddConsoleExporter()` por `AddOtlpExporter()` apontando pra um Prometheus/Grafana.

---

## Testes

O projeto usa [xUnit](https://xunit.net/) com padrão **AAA** (Arrange, Act, Assert) e
nomenclatura `MetodoTestado_Cenario_ResultadoEsperado`, dividido em dois projetos:

```bash
dotnet test
```

- **`PetCareHub.Tests.Unit`**, dividido em dois níveis:
  - **`Domain`**: testa a regra de negócio direto na entidade, **sem repositório nem mock
    nenhum** — `AlertaSaude.Resolver()` (lança exceção se já estiver resolvido) e
    `Clinica.GarantirQuePodeSerExcluida(bool)` (lança exceção se a clínica tiver pet
    vinculado). São as duas regras que dependem só do próprio estado da entidade; o fato
    externo que a regra precisa (ex.: "essa clínica tem pet vinculado?") é informado por quem
    chama, mas quem decide o que fazer com esse fato é a entidade.
  - **`Services`** (`ClinicaService`, `ConsultaService`, `AlertaSaudeService`, `PetService`,
    `TutorService`): testa a orquestração — buscar no repositório, delegar a regra pra
    entidade quando aplicável, validar coisas que só o repositório sabe (CNPJ duplicado,
    pet/clínica inexistente), persistir. Repositórios mockados via
    [Moq](https://github.com/devlooped/moq), cobrindo caso feliz e caso de erro de cada regra.
- **`PetCareHub.Tests.Integration`**: testes de ponta a ponta via `WebApplicationFactory<Program>`
  (compartilhada entre as classes de teste através de `ICollectionFixture`), batendo nos
  endpoints reais da API. Como o projeto não tem um provider in-memory para o EF Core (só
  `Oracle.EntityFrameworkCore`), esses testes rodam contra o **Oracle real da FIAP** — rodar
  `dotnet test` exige a mesma rede/VPN da FIAP usada pelo `dotnet run`. Inclui:
  - Um teste por controller (`Clinicas`, `Consultas`, `Pets`, `Tutores`, `AlertasSaude`,
    `ScoresSaude`, `Dashboard`) cobrindo o caminho feliz autenticado;
  - `AuthorizationTests` — os três cenários de autenticação exigidos: sem token (`401`),
    token malformado/expirado (`401`) e token válido com role sem permissão (`403`), além do
    caminho autorizado (`200`) e da confirmação de que `/health/live` continua público;
  - `HealthCheckTests` — cobre `/health`, `/health/live` e `/health/ready` separadamente,
    checando o `status` real (`Healthy`), não só "respondeu alguma coisa";
  - `Clinica_CriarConsultarAtualizarExcluir_FluxoCompletoPersisteNoOracle` (em
    `ClinicasControllerTests`) — fluxo de ponta a ponta real (create → get → update → get →
    delete → get), conferindo o corpo de cada resposta e que a mudança realmente persistiu no
    Oracle entre uma chamada HTTP e a seguinte (não só que o Create "parece" certo). A clínica
    de teste usa um CNPJ gerado por timestamp e é sempre excluída no fim (inclusive se um assert
    falhar no meio), pra não deixar lixo na base compartilhada da turma. O mesmo padrão
    (criar → usar o id → excluir no `finally`) é usado em `DashboardControllerTests`, que antes
    dependia de uma clínica de id fixo já existente na base.

  Testes que exigem autenticação usam `factory.CreateAuthenticatedClient(role: "CLINICA")` —
  gera um JWT assinado com a mesma chave de teste que a API valida por padrão
  (`PetCareHub.API/Keys/public_key.pem`), sem precisar de um login real contra o Java.

---

## Integração com o Challenge

O PetCare Hub se conecta ao desafio CLYVO VET ao apoiar a continuidade do cuidado do pet por meio de dados clínicos, preventivos e de sensores.

A API .NET atua na camada de visualização B2B, permitindo que clínicas acompanhem indicadores e tomem ações proativas com base nos dados registrados.

---

## 👥 Integrantes da Equipe

| Nome | RM | Turma | GitHub | LinkedIn |
|---|---|---|---|---|
| Alexander Dennis Isidro Mamani | 565554 | 2TDSPG | [alex-isidro](https://github.com/alex-isidro) | [LinkedIn](https://www.linkedin.com/in/alexander-dennis-a3b48824b/) |
| Kelson Zhang | 563748 | 2TDSPG | [KelsonZh0](https://github.com/KelsonZh0) | [LinkedIn](https://www.linkedin.com/in/kelson-zhang-211456323/) |