# CareerFlow - Resumo Rápido

## Comandos Essenciais

### Build e Execução

```bash
# Restaurar pacotes
dotnet restore

# Compilar tudo
dotnet build

# Executar API (desenvolvimento)
cd src/CareerFlow.Api
dotnet run

# Executar com hot reload
dotnet watch run
```

### Testes

```bash
# Testes unitários
dotnet test tests/CareerFlow.Tests.Unit

# Testes de integração (requer Docker)
dotnet test tests/CareerFlow.Tests.Integration

# Todos os testes
dotnet test

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### Docker

```bash
# Subir PostgreSQL + Redis
docker compose up -d

# Parar containers
docker compose down

# Ver logs
docker compose logs -f
```

### Migrations (EF Core)

```bash
# Adicionar migration
dotnet ef migrations add NomeDaMigration \
  --project src/CareerFlow.Infrastructure \
  --startup-project src/CareerFlow.Api

# Aplicar migrations
dotnet ef database update \
  --project src/CareerFlow.Infrastructure \
  --startup-project src/CareerFlow.Api

# Remover última migration
dotnet ef migrations remove \
  --project src/CareerFlow.Infrastructure \
  --startup-project src/CareerFlow.Api
```

### Publicação

```bash
# Publicar para produção
dotnet publish src/CareerFlow.Api -c Release -o out

# Publicar com runtime específico
dotnet publish src/CareerFlow.Api -c Release -o out \
  --runtime linux-x64 \
  --self-contained false
```

---

## Links Úteis

| Recurso | URL |
|---------|-----|
| API (dev) | http://localhost:5000 |
| Scalar Docs | http://localhost:5000/scalar/v1 |
| OpenAPI JSON | http://localhost:5000/openapi/v1.json |
| Health Check | http://localhost:5000/health |
| Render Dashboard | https://dashboard.render.com |
| NuGet Gallery | https://www.nuget.org |

---

## Credenciais de Teste (Seed Data)

| Tipo | Email | Senha |
|------|-------|-------|
| **Admin** | admin@careerflow.com | Admin@123 |
| **Premium** | joao.premium@email.com | Teste@123 |

---

## Estrutura do Projeto

```
CareerFlow/
├── src/
│   ├── CareerFlow.Core/           # DDD - Entidades, ValueObjects, Enums, Interfaces
│   ├── CareerFlow.Application/    # CQRS - Commands, Queries, Handlers, Validators
│   ├── CareerFlow.Infrastructure/ # EF Core, Repositories, PDF, Outbox
│   ├── CareerFlow.Api/            # Controllers, Middleware, Program.cs
│   └── CareerFlow.Scheduler/      # Quartz.NET jobs (placeholder)
├── tests/
│   ├── CareerFlow.Tests.Unit/     # 8 classes (xUnit + Moq)
│   └── CareerFlow.Tests.Integration/ # 3 classes (Testcontainers)
├── docs/                          # Documentação
├── docker-compose.yml             # PostgreSQL + Redis
└── .gitignore
```

---

## Stack

| Tecnologia | Versão | Uso |
|------------|--------|-----|
| .NET | 10 | Runtime |
| C# | 13 | Linguagem |
| ASP.NET Core | 10 | API |
| EF Core | 10 | ORM |
| PostgreSQL | 16 | Banco |
| Redis | 7 | Cache |
| MediatR | 12.4 | CQRS |
| FluentValidation | 11.11 | Validação |
| AutoMapper | 14.0 | Mapping |
| Serilog | 8.0 | Logging |
| QuestPDF | 2025.1 | PDF |
| xUnit | 2.9.3 | Testes |
| Testcontainers | 4.3.0 | Testes integração |
| Scalar | 2.1 | API docs |

---

## Arquitetura: Fluxo de uma Requisição

```
HTTP Request
    ↓
Middleware Pipeline (Exception → Logging → CORS → Rate Limit → Auth)
    ↓
Controller
    ↓
MediatR → Handler (Command/Query)
    ↓
Validação (FluentValidation)
    ↓
Serviço / Repository (via Interface)
    ↓
EF Core / PostgreSQL
    ↓
Response ← Middleware
```

---

## Padrões Implementados

- **Domain-Driven Design** (DDD)
- **CQRS** (Command Query Responsibility Segregation)
- **Repository Pattern**
- **Unit of Work**
- **Outbox Pattern** (eventual consistency)
- **Specification Pattern**
- **Pipeline Behavior** (MediatR)
- **Audit Logging** (EF Core interceptor)
- **Soft Delete**

---

## Sobre

CareerFlow é um sistema SaaS para criação e gerenciamento de currículos profissionais com análise de compatibilidade ATS (Applicant Tracking System).

**Versão:** 1.0 | **Licença:** Privada
