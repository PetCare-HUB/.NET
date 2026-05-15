# PetCare Hub — API .NET Dashboard Clínico

API RESTful desenvolvida em **.NET Core .NET 9** para a disciplina **Advanced Business Development with .NET**, dentro do Challenge FIAP 2026 — CLYVO VET.

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

- Consultar clínicas cadastradas;
- Gerenciar clínicas via CRUD;
- Consultar pets da clínica;
- Consultar consultas clínicas;
- Consultar alertas de saúde;
- Resolver alertas;
- Consultar scores de saúde;
- Exibir métricas consolidadas no dashboard.

---

## Tecnologias Utilizadas

- .NET 9
- ASP.NET Core Web API
- Controllers
- Entity Framework Core 9
- Oracle Entity Framework Core
- Oracle Database FIAP
- Swagger / OpenAPI
- C#
- Git e GitHub

---

## Arquitetura do Projeto

O projeto foi organizado em camadas, seguindo uma estrutura próxima de Clean Architecture:

```txt
PetCareHub
├── PetCareHub.API
│   ├── Controllers
│   ├── Program.cs
│   └── appsettings.json
│
├── PetCareHub.Application
│
├── PetCareHub.Domain
│   └── Entities
│
└── PetCareHub.Infrastructure
    ├── Persistence
    │   ├── Configurations
    │   ├── Migrations
    │   └── PetCareHubContext.cs
    └── DependencyInjection.cs
```

---

## Responsabilidade das Camadas

### PetCareHub.API

Camada responsável pela exposição dos endpoints HTTP.

Contém:

- Controllers;
- Swagger;
- Configuração da aplicação;
- Rotas REST.

### PetCareHub.Domain

Camada responsável pelas entidades principais do domínio.

Entidades:

- Clinica
- Pet
- Consulta
- EventoPreventivo
- LeituraSensor
- AlertaSaude
- ScoreSaude

### PetCareHub.Infrastructure

Camada responsável pela integração com o banco Oracle.

Contém:

- DbContext;
- Configurações Fluent API;
- Migrations;
- Configuração do Oracle via EF Core.

---

## Banco de Dados

A API utiliza o banco Oracle da FIAP, compartilhado com a modelagem desenvolvida na disciplina de Database.

Tabelas utilizadas pela API:

- CLINICA
- PET
- CONSULTA
- EVENTO_PREVENTIVO
- LEITURA_SENSOR
- ALERTA_SAUDE
- SCORE_SAUDE

O mapeamento entre C# e Oracle foi feito com **Fluent API**, respeitando os nomes reais das tabelas e colunas do banco.

Exemplo:

```txt
Classe C#       Tabela Oracle
Clinica         CLINICA
Pet             PET
Consulta        CONSULTA
AlertaSaude     ALERTA_SAUDE
ScoreSaude      SCORE_SAUDE
```

---

## Configuração do Banco Oracle

Por segurança, as credenciais reais não devem ser versionadas no GitHub.

Crie o arquivo:

```txt
PetCareHub.API/appsettings.Development.json
```

Com o seguinte formato:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle.fiap.com.br)(PORT=1521))(CONNECT_DATA=(SID=orcl)))"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

O arquivo `appsettings.Development.json` deve estar no `.gitignore`.

Exemplo de `.gitignore`:

```gitignore
bin/
obj/
.vs/
.idea/
*.user
*.suo
*.rsuser
*.log
appsettings.Development.json
```

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

### 3. Compilar o projeto

```bash
dotnet build
```

### 4. Executar a API

```bash
dotnet run --project PetCareHub.API
```

A aplicação será iniciada em uma URL semelhante a:

```txt
http://localhost:5062
```

### 5. Acessar o Swagger

Abra no navegador:

```txt
http://localhost:5062/swagger
```

---

## Migrations

O projeto utiliza Entity Framework Core Migrations.

Para criar a migration inicial:

```bash
dotnet ef migrations add InitialCreate --project PetCareHub.Infrastructure --startup-project PetCareHub.API --output-dir Persistence/Migrations
```

Observação:

Como o banco Oracle da disciplina de Database já possui as tabelas criadas, o comando abaixo **não deve ser executado no schema real caso as tabelas já existam**:

```bash
dotnet ef database update --project PetCareHub.Infrastructure --startup-project PetCareHub.API
```

Esse comando deve ser usado apenas em um schema vazio ou ambiente novo.

---

## Endpoints Disponíveis

# Clínicas

## Listar clínicas

```http
GET /api/Clinicas
```

Retorno:

```http
200 OK
```

---

## Buscar clínica por ID

```http
GET /api/Clinicas/{id}
```

Retornos:

```http
200 OK
404 Not Found
```

---

## Criar clínica

```http
POST /api/Clinicas
```

Body:

```json
{
  "nome": "Clinica Teste DotNet",
  "cnpj": "99.999.999/0001-99",
  "email": "teste@petcare.com",
  "telefone": "(11) 99999-9999",
  "endereco": "Rua Teste, 123"
}
```

Retornos:

```http
201 Created
400 Bad Request
```

---

## Atualizar clínica

```http
PUT /api/Clinicas/{id}
```

Body:

```json
{
  "nome": "Clinica Teste DotNet Atualizada",
  "cnpj": "99.999.999/0001-99",
  "email": "atualizada@petcare.com",
  "telefone": "(11) 98888-8888",
  "endereco": "Rua Atualizada, 456",
  "ativo": true
}
```

Retornos:

```http
200 OK
400 Bad Request
404 Not Found
```

---

## Remover clínica

```http
DELETE /api/Clinicas/{id}
```

Retornos:

```http
204 No Content
400 Bad Request
404 Not Found
```

Observação:

A API não permite remover uma clínica que possui pets vinculados.

---

# Pets

## Listar pets

```http
GET /api/Pets
```

Filtros opcionais:

```http
GET /api/Pets?clinicaId=1
GET /api/Pets?especie=CAO
GET /api/Pets?ativo=true
```

---

## Buscar pet por ID

```http
GET /api/Pets/{id}
```

---

## Listar pets por clínica

```http
GET /api/Pets/clinica/{clinicaId}
```

---

# Consultas

## Listar consultas

```http
GET /api/Consultas
```

Filtros opcionais:

```http
GET /api/Consultas?clinicaId=1
GET /api/Consultas?petId=1
GET /api/Consultas?tipoConsulta=CHECKUP
GET /api/Consultas?retornoRecomendado=true
```

---

## Buscar consulta por ID

```http
GET /api/Consultas/{id}
```

---

## Listar consultas por clínica

```http
GET /api/Consultas/clinica/{clinicaId}
```

---

## Listar consultas por pet

```http
GET /api/Consultas/pet/{petId}
```

---

# Alertas de Saúde

## Listar alertas

```http
GET /api/AlertasSaude
```

Filtros opcionais:

```http
GET /api/AlertasSaude?petId=1
GET /api/AlertasSaude?nivelAlerta=CRITICO
GET /api/AlertasSaude?resolvido=false
```

---

## Buscar alerta por ID

```http
GET /api/AlertasSaude/{id}
```

---

## Listar alertas por pet

```http
GET /api/AlertasSaude/pet/{petId}
```

---

## Listar alertas por clínica

```http
GET /api/AlertasSaude/clinica/{clinicaId}
```

Exemplo:

```http
GET /api/AlertasSaude/clinica/1?resolvido=false
```

---

## Resolver alerta

```http
PUT /api/AlertasSaude/{id}/resolver
```

Retornos:

```http
204 No Content
400 Bad Request
404 Not Found
```

---

# Scores de Saúde

## Listar scores

```http
GET /api/ScoresSaude
```

Filtros opcionais:

```http
GET /api/ScoresSaude?petId=1
GET /api/ScoresSaude?clinicaId=1
GET /api/ScoresSaude?categoria=VERMELHO
GET /api/ScoresSaude?scoreMin=0&scoreMax=50
```

---

## Buscar score por ID

```http
GET /api/ScoresSaude/{id}
```

---

## Listar scores por pet

```http
GET /api/ScoresSaude/pet/{petId}
```

---

## Buscar score atual do pet

```http
GET /api/ScoresSaude/pet/{petId}/atual
```

---

## Listar scores por clínica

```http
GET /api/ScoresSaude/clinica/{clinicaId}
```

---

# Dashboard Clínico

## Resumo da clínica

```http
GET /api/Dashboard/clinicas/{clinicaId}
```

Retorna:

- Dados da clínica;
- Total de pets;
- Pets ativos;
- Alertas abertos;
- Consultas realizadas;
- Eventos pendentes;
- Score médio;
- Pets em risco.

---

## Pets em risco

```http
GET /api/Dashboard/clinicas/{clinicaId}/pets-em-risco
```

---

## Alertas abertos

```http
GET /api/Dashboard/clinicas/{clinicaId}/alertas-abertos
```

---

## Consultas recentes

```http
GET /api/Dashboard/clinicas/{clinicaId}/consultas-recentes
```

---

## Eventos preventivos pendentes

```http
GET /api/Dashboard/clinicas/{clinicaId}/eventos-pendentes
```

---

## Retornos HTTP Implementados

A API utiliza os seguintes retornos HTTP:

```txt
200 OK              Consulta realizada com sucesso
201 Created         Recurso criado com sucesso
204 No Content      Remoção ou atualização sem corpo de resposta
400 Bad Request     Dados inválidos ou regra de negócio violada
404 Not Found       Recurso não encontrado
500 Internal Error  Erro inesperado
```

---

## Exemplos de Teste no Swagger

### Criar uma clínica

```http
POST /api/Clinicas
```

```json
{
  "nome": "Clinica Teste DotNet",
  "cnpj": "99.999.999/0001-99",
  "email": "teste@petcare.com",
  "telefone": "(11) 99999-9999",
  "endereco": "Rua Teste, 123"
}
```

### Atualizar uma clínica

```http
PUT /api/Clinicas/{id}
```

```json
{
  "nome": "Clinica Teste DotNet Atualizada",
  "cnpj": "99.999.999/0001-99",
  "email": "atualizada@petcare.com",
  "telefone": "(11) 98888-8888",
  "endereco": "Rua Atualizada, 456",
  "ativo": true
}
```

### Resolver um alerta

```http
PUT /api/AlertasSaude/{id}/resolver
```

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
Dashboard clínico: funcional
```

---

## 👥 Integrantes da Equipe

| Nome | RM | Turma | GitHub | LinkedIn |
|---|---|---|---|---|
| Alexander Dennis Isidro Mamani | 565554 | 2TDSPG | [alex-isidro](https://github.com/alex-isidro) | [LinkedIn](https://www.linkedin.com/in/alexander-dennis-a3b48824b/) |
| Kelson Zhang | 563748 | 2TDSPG | [KelsonZh0](https://github.com/KelsonZh0) | [LinkedIn](https://www.linkedin.com/in/kelson-zhang-211456323/) |

---
