# CareerFlow API

Sistema SaaS para criação, gerenciamento e compartilhamento de currículos profissionais com foco em compatibilidade ATS (Applicant Tracking System).

![.NET](https://img.shields.io/badge/.NET-10.0-purple?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-13.0-blue?logo=csharp)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue?logo=postgresql)
![Redis](https://img.shields.io/badge/Redis-7-red?logo=redis)
![Docker](https://img.shields.io/badge/Docker-ready-blue?logo=docker)
![Tests](https://img.shields.io/badge/tests-14%20passed-brightgreen)
![Endpoints](https://img.shields.io/badge/endpoints-59-orange)

---

## Stack Tecnológica

| Componente | Tecnologia | Versão |
|------------|------------|--------|
| Runtime | .NET | 10 |
| Linguagem | C# | 13 |
| Framework Web | ASP.NET Core | 10 |
| ORM | Entity Framework Core | 10 |
| Banco de Dados | PostgreSQL | 16 |
| Cache | Redis | 7 |
| Mensageria | Outbox Pattern (PostgreSQL) | - |
| PDF | QuestPDF | 2025.1 |
| CQRS | MediatR | 12.4 |
| Validação | FluentValidation | 11.11 |
| Mapeamento | AutoMapper | 14.0 |
| Logging | Serilog | 8.0 |
| Testes | xUnit + Testcontainers | - |
| Documentação | Scalar | 2.1 |
| Autenticação | JWT (HMAC-SHA512) | - |

---

## Arquitetura

```
CareerFlow.sln
├── src/
│   ├── CareerFlow.Core/               # Domínio (DDD)
│   │   ├── Entities/                  # 12 entidades
│   │   ├── ValueObjects/              # Email, Phone, URL, DateRange
│   │   ├── Enums/                     # 9 enums
│   │   ├── Events/                    # 9 domain events
│   │   ├── Interfaces/                # 13 interfaces
│   │   ├── Exceptions/                # 5 exceções
│   │   └── Specifications/            # 7 specifications
│   │
│   ├── CareerFlow.Application/        # Aplicação (CQRS)
│   │   ├── Features/
│   │   │   ├── Auth/                  # Register, Login, JWT
│   │   │   ├── Profile/              # CRUD Perfil
│   │   │   ├── Skills/               # CRUD Habilidades
│   │   │   ├── Experiences/          # CRUD Experiências
│   │   │   ├── Education/            # CRUD Formação
│   │   │   ├── Certificates/         # CRUD Certificados
│   │   │   ├── Languages/            # CRUD Idiomas
│   │   │   ├── SocialNetworks/       # CRUD Redes Sociais
│   │   │   ├── Resume/               # PDF, Share, Analytics
│   │   │   ├── Dashboard/            # Stats, Insights
│   │   │   └── Admin/                # Gestão de usuários
│   │   ├── Common/                    # Behaviors, Mappings
│   │   └── Services/                  # Password, Token, Logger
│   │
│   ├── CareerFlow.Infrastructure/     # Infraestrutura
│   │   ├── Data/                      # DbContext, Configurations
│   │   ├── Repositories/              # 9 repositórios
│   │   ├── External/PDF/              # QuestPDF Generator
│   │   └── Outbox/                    # Outbox Pattern (Timer)
│   │
│   ├── CareerFlow.Api/                # Web API (ASP.NET Core)
│   │   ├── Controllers/               # 14 controllers
│   │   ├── Middlewares/               # Exception, Logging
│   │   └── Program.cs
│   │
│   └── CareerFlow.Scheduler/          # Background Jobs (Quartz.NET)
│
└── tests/
    ├── CareerFlow.Tests.Unit/         # 8 classes de teste
    └── CareerFlow.Tests.Integration/  # 3 classes (Testcontainers)
```

### Padrões e Práticas

- **Domain-Driven Design (DDD)** com Aggregates, Value Objects e Domain Events
- **CQRS** com Commands, Queries e Handlers via MediatR
- **Repository Pattern** com interfaces no Core e implementação no Infrastructure
- **Unit of Work** encapsulado pelo DbContext
- **Outbox Pattern** para consistência eventual entre banco e mensageria
- **Specification Pattern** para regras de negócio reutilizáveis
- **FluentValidation** com pipeline behavior do MediatR
- **Audit Logging** via EF Core interceptor (SaveChangesInterceptor)

---

## Endpoints da API (59 total)

### Autenticação (6)

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/v1/auth/register` | Registrar usuário |
| POST | `/api/v1/auth/login` | Login |
| POST | `/api/v1/auth/refresh` | Refresh token |
| GET | `/api/v1/auth/me` | Perfil autenticado |
| POST | `/api/v1/auth/forgot-password` | Recuperar senha |
| POST | `/api/v1/auth/change-password` | Alterar senha |

### Perfil (6)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/profile` | Obter perfil |
| POST | `/api/v1/profile` | Criar perfil |
| PUT | `/api/v1/profile` | Atualizar perfil |
| GET | `/api/v1/profile/completion` | Percentual de completude |
| POST | `/api/v1/profile/photo` | Upload de foto |
| DELETE | `/api/v1/profile/photo` | Remover foto |

### Skills (6)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/profile/skills` | Listar habilidades |
| GET | `/api/v1/profile/skills/categories` | Listar categorias |
| POST | `/api/v1/profile/skills` | Adicionar habilidade |
| PUT | `/api/v1/profile/skills/{id}` | Atualizar habilidade |
| DELETE | `/api/v1/profile/skills/{id}` | Remover habilidade |
| POST | `/api/v1/profile/skills/reorder` | Reordenar habilidades |

### Experiences (5)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/profile/experiences` | Listar experiências |
| GET | `/api/v1/profile/experiences/{id}` | Detalhes da experiência |
| POST | `/api/v1/profile/experiences` | Adicionar experiência |
| PUT | `/api/v1/profile/experiences/{id}` | Atualizar experiência |
| DELETE | `/api/v1/profile/experiences/{id}` | Remover experiência |

### Education (4)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/profile/education` | Listar formações |
| POST | `/api/v1/profile/education` | Adicionar formação |
| PUT | `/api/v1/profile/education/{id}` | Atualizar formação |
| DELETE | `/api/v1/profile/education/{id}` | Remover formação |

### Certificates (4)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/profile/certificates` | Listar certificados |
| POST | `/api/v1/profile/certificates` | Adicionar certificado |
| PUT | `/api/v1/profile/certificates/{id}` | Atualizar certificado |
| DELETE | `/api/v1/profile/certificates/{id}` | Remover certificado |

### Languages (4)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/profile/languages` | Listar idiomas |
| POST | `/api/v1/profile/languages` | Adicionar idioma |
| PUT | `/api/v1/profile/languages/{id}` | Atualizar idioma |
| DELETE | `/api/v1/profile/languages/{id}` | Remover idioma |

### Social Networks (4)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/profile/social-networks` | Listar redes sociais |
| POST | `/api/v1/profile/social-networks` | Adicionar rede social |
| PUT | `/api/v1/profile/social-networks/{id}` | Atualizar rede social |
| DELETE | `/api/v1/profile/social-networks/{id}` | Remover rede social |

### Resume (7)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/resume` | Currículo completo |
| GET | `/api/v1/resume/analytics` | Analytics do currículo |
| POST | `/api/v1/resume/share` | Compartilhar currículo |
| PUT | `/api/v1/resume/publish` | Publicar currículo |
| PUT | `/api/v1/resume/unpublish` | Despublicar currículo |
| POST | `/api/v1/resume/generate` | Gerar PDF padrão |
| POST | `/api/v1/resume/generate-ats` | Gerar PDF ATS |

### Público (1)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/resume/shared/{slug}` | Currículo público (sem auth) |

### Dashboard (5)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/dashboard/stats` | Estatísticas gerais |
| GET | `/api/v1/dashboard/insights` | Insights e recomendações |
| GET | `/api/v1/dashboard/activity` | Atividades recentes |
| GET | `/api/v1/dashboard/views-chart` | Gráfico de visualizações |
| GET | `/api/v1/dashboard/skills-gap` | Análise de skills gap |

### Admin (7)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/admin/stats/overview` | Estatísticas do sistema |
| GET | `/api/v1/admin/users` | Listar usuários |
| GET | `/api/v1/admin/users/{id}` | Detalhe do usuário |
| PUT | `/api/v1/admin/users/{id}` | Atualizar usuário |
| PATCH | `/api/v1/admin/users/{id}/status` | Ativar/desativar usuário |
| PATCH | `/api/v1/admin/users/{id}/premium` | Gerenciar premium |
| DELETE | `/api/v1/admin/users/{id}` | Soft delete |

---

## Como Executar

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [PostgreSQL 16](https://www.postgresql.org/download/)
- [Redis 7](https://redis.io/download/) (opcional para MVP)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (para testes de integração)

### 1. Clonar e configurar

```bash
git clone https://github.com/seu-usuario/CareerFlow.git
cd CareerFlow
```

### 2. Subir dependências (Docker)

```bash
docker compose up -d
```

### 3. Configurar variáveis de ambiente

Edite `src/CareerFlow.Api/appsettings.Development.json`:

```json
{
  "Database": {
    "ConnectionString": "Host=localhost;Port=5432;Database=careerflow;Username=postgres;Password=postgres;"
  },
  "Jwt": {
    "SecretKey": "sua-chave-secreta-com-minimo-512-bits-aqui"
  }
}
```

### 4. Executar a API

```bash
dotnet restore
dotnet build
cd src/CareerFlow.Api
dotnet run
```

### 5. Acessar

| Recurso | URL |
|---------|-----|
| API | http://localhost:5000 |
| Health Check | http://localhost:5000/health |
| Scalar Docs | http://localhost:5000/scalar/v1 |
| OpenAPI JSON | http://localhost:5000/openapi/v1.json |

### Credenciais de Teste

| Tipo | Email | Senha |
|------|-------|-------|
| Admin | admin@careerflow.com | Admin@123 |
| Premium | joao.premium@email.com | Teste@123 |

---

## Testes

```bash
# Testes unitários (8 classes)
dotnet test tests/CareerFlow.Tests.Unit

# Testes de integração (3 classes, requer Docker)
dotnet test tests/CareerFlow.Tests.Integration

# Todos os testes
dotnet test
```

---

## Métricas do Projeto

| Métrica | Valor |
|---------|-------|
| Entidades | 12 |
| Value Objects | 5 |
| Enums | 9 |
| Domain Events | 9 |
| Specifications | 7 |
| Exceptions | 5 |
| Interfaces | 13 |
| Repositórios | 9 |
| Controllers | 14 |
| Endpoints | 59 |
| Handlers (CQRS) | 25+ |
| Testes Unitários | 8 classes |
| Testes Integração | 3 classes |
| Arquivos | ~200+ |
| Linhas de Código | ~15.000+ |

---

## Documentação Adicional

- [docs/API.md](docs/API.md) - Detalhes de autenticação, respostas e rate limiting
- [docs/DEPLOY.md](docs/DEPLOY.md) - Guia de deploy no Render
- [docs/RESUMO.md](docs/RESUMO.md) - Comandos e links úteis

---

## Licença

Este projeto é privado. Todos os direitos reservados.

**Versão:** 1.0 | **Última atualização:** Julho 2026 | **Status:** Implementação completa
