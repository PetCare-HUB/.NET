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
- Gerenciar pets via CRUD completo (com filtros por clínica, espécie e situação);
- Gerenciar consultas via CRUD completo (com filtros por clínica, pet, tipo e retorno);
- Gerenciar alertas de saúde via CRUD completo (com filtros por pet, nível e situação);
- Resolver alertas;
- Consultar responsáveis pelos pets;
- Consultar scores de saúde com filtros por categoria e faixa de score;
- Exibir métricas consolidadas no dashboard.

---

## Tecnologias Utilizadas

- .NET 9
- ASP.NET Core Web API (Controllers)
- Entity Framework Core 9
- Oracle.EntityFrameworkCore 9.23
- Oracle Database FIAP
- Swagger / OpenAPI (Swashbuckle)
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
├── PetCareHub.Domain              ← Entidades de domínio (puras)
│   └── Entities
│
└── PetCareHub.Infrastructure      ← Persistência (EF Core + Oracle)
    └── Persistence
        ├── Configurations         ← Fluent API por entidade
        ├── Migrations
        ├── Repositories
        ├── PetCareHubContext.cs
        └── Repository.cs          ← Repositório genérico
```

---

## Banco de Dados

A API utiliza o banco Oracle da FIAP, compartilhado com a modelagem da disciplina de Database.

Tabelas utilizadas pela API:

```txt
Classe C#         Tabela Oracle
Clinica           CLINICA
Responsavel       RESPONSAVEL
Pet               PET
Consulta          CONSULTA
AlertaSaude       ALERTA_SAUDE
ScoreSaude        SCORE_SAUDE
EventoPreventivo  EVENTO_PREVENTIVO
LeituraSensor     LEITURA_SENSOR
```

O mapeamento entre C# e Oracle é feito com **Fluent API** em `PetCareHub.Infrastructure/Persistence/Configurations`, respeitando os nomes reais das tabelas e colunas.

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

O arquivo `appsettings.Development.json` está no `.gitignore` e não deve ser commitado.

---

## Como Executar o Projeto

### 1. Clonar o repositório

```bash
git clone <url-do-repositorio>
cd PetCareHub
```

### 2. Restaurar os pacotes

```bash
dotnet restore
```

### 3. Aplicar as migrations no banco (apenas em ambiente novo)

> Se o schema Oracle da FIAP já possuir as tabelas criadas pela modelagem de Database, pule este passo.

```bash
dotnet ef database update --project PetCareHub.Infrastructure --startup-project PetCareHub.API
```

### 4. Compilar o projeto

```bash
dotnet build
```

### 5. Executar a API

```bash
dotnet run --project PetCareHub.API
```

A aplicação será iniciada em:

```txt
http://localhost:5062
```

### 6. Acessar o Swagger

O Swagger é servido na **raiz** da aplicação (não em `/swagger`), pois `RoutePrefix` está configurado como vazio.

Abra no navegador:

```txt
http://localhost:5062/
```

---

## Migrations

O projeto utiliza Entity Framework Core Migrations.

Para criar uma nova migration:

```bash
dotnet ef migrations add NomeDaMigration --project PetCareHub.Infrastructure --startup-project PetCareHub.API --output-dir Persistence/Migrations
```

Para aplicar migrations no banco:

```bash
dotnet ef database update --project PetCareHub.Infrastructure --startup-project PetCareHub.API
```

---

## Endpoints Disponíveis

Todos os endpoints seguem o padrão REST e estão documentados no Swagger.

### 🏥 Clínicas

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

### 🐶 Pets

| Método | Rota | Descrição |
|---|---|---|
| `GET`    | `/api/Pets`                          | Lista pets (filtros opcionais) |
| `GET`    | `/api/Pets/{id}`                     | Busca pet pelo ID |
| `GET`    | `/api/Pets/clinica/{clinicaId}`      | Lista pets de uma clínica |
| `POST`   | `/api/Pets`                          | Cadastra um novo pet |
| `PUT`    | `/api/Pets/{id}`                     | Atualiza um pet |
| `DELETE` | `/api/Pets/{id}`                     | Remove um pet |

**Filtros opcionais no GET:**

```
GET /api/Pets?clinicaId=1
GET /api/Pets?especie=CAO
GET /api/Pets?ativo=true
GET /api/Pets?clinicaId=1&especie=GATO&ativo=true
```

**Exemplo POST:**

```json
{
  "responsavelId": 1,
  "clinicaId": 1,
  "nome": "Rex",
  "especie": "CAO",
  "raca": "Labrador",
  "dataNascimento": "2022-05-10",
  "pesoKg": 18.5,
  "sexo": "M",
  "condicoesCronicas": null,
  "ativo": true
}
```

---

### 🩺 Consultas

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

---

### 🚨 Alertas de Saúde

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

---

### 📊 Scores de Saúde

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

### 👤 Responsáveis

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/Responsaveis`                       | Lista todos os responsáveis |
| `GET` | `/api/Responsaveis/{id}`                  | Busca responsável pelo ID |
| `GET` | `/api/Responsaveis/clinica/{clinicaId}`   | Lista responsáveis com pets na clínica |

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

---

## Integração com o Challenge

O PetCare Hub se conecta ao desafio CLYVO VET ao apoiar a continuidade do cuidado do pet por meio de dados clínicos, preventivos e de sensores.

A API .NET atua na camada de visualização B2B, permitindo que clínicas acompanhem indicadores e tomem ações proativas com base nos dados registrados.

---

## Status do Projeto

```txt
API .NET: funcional
Conexão Oracle: funcionando
Swagger: funcionando
EF Core: configurado
Migrations: criadas
CRUD de clínicas: funcional
CRUD de pets: funcional
CRUD de consultas: funcional
CRUD de alertas: funcional
Dashboard clínico: funcional
```

---

## 👥 Integrantes da Equipe

| Nome | RM | Turma | GitHub | LinkedIn |
|---|---|---|---|---|
| Alexander Dennis Isidro Mamani | 565554 | 2TDSPG | [alex-isidro](https://github.com/alex-isidro) | [LinkedIn](https://www.linkedin.com/in/alexander-dennis-a3b48824b/) |
| Kelson Zhang | 563748 | 2TDSPG | [KelsonZh0](https://github.com/KelsonZh0) | [LinkedIn](https://www.linkedin.com/in/kelson-zhang-211456323/) |