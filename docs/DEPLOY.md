# Deploy do CareerFlow no Render

## Pré-requisitos

- Conta no [Render](https://render.com)
- Repositório no GitHub com o código
- PostgreSQL 16 (Render oferecido como serviço)

---

## Passo a Passo

### 1. Criar o Banco de Dados

No dashboard do Render:

1. **New +** → **PostgreSQL**
2. Configure:
   - **Name:** `careerflow-db`
   - **Database:** `careerflow`
   - **User:** `careerflow_user`
   - **Region:** `ohio` (ou a mais próxima)
   - **Plan:** Free (ou Starter para produção)
3. Anote a **Internal Database URL** e a **External Database URL**

### 2. Criar o Web Service

1. **New +** → **Web Service**
2. Conecte ao repositório GitHub
3. Configure:

| Campo | Valor |
|-------|-------|
| **Name** | `careerflow-api` |
| **Region** | `ohio` |
| **Branch** | `main` |
| **Runtime** | `.NET` |
| **Build Command** | `dotnet publish src/CareerFlow.Api -c Release -o out` |
| **Start Command** | `dotnet out/CareerFlow.Api.dll` |
| **Plan** | Free (ou Starter) |

### 3. Variáveis de Ambiente

Adicione as seguintes environment variables no Render:

```env
Database__ConnectionString=Host=SEU_HOST;Port=5432;Database=careerflow;Username=careerflow_user;Password=SUA_SENHA;SSL Mode=Require;Trust Server Certificate=true;

Jwt__SecretKey=SUA_CHAVE_COM_NO_MINIMO_64_CARACTERES_AQUI

Redis__Enabled=false

Application__Name=CareerFlow

Application__Environment=Production

Application__CorsOrigins=https://seudominio.com

RateLimiting__Login__PermitLimit=5

RateLimiting__Register__PermitLimit=3

ASPNETCORE_ENVIRONMENT=Production
```

> **Atenção:** Use nomes com `__` (double underscore) que o .NET mapeia automaticamente para seções aninhadas do `appsettings.json`.

### 4. Health Check

O Render pode monitorar a saúde do serviço em:

```
https://careerflow-api.onrender.com/health
```

Configure no Render: **Health Check Path** = `/health`

### 5. Domain Personalizado (opcional)

1. **Settings** → **Custom Domain**
2. Adicione seu domínio (ex: `api.careerflow.com`)
3. Configure o DNS com o CNAME fornecido pelo Render

---

## Deploy Local (para teste/desenvolvimento)

```bash
# 1. Restaurar pacotes
dotnet restore

# 2. Build de release
dotnet build -c Release

# 3. Publicar
dotnet publish src/CareerFlow.Api -c Release -o out

# 4. Executar
ASPNETCORE_ENVIRONMENT=Production \
Database__ConnectionString="Host=localhost;Port=5432;Database=careerflow;Username=postgres;Password=postgres;" \
dotnet out/CareerFlow.Api.dll
```

---

## Docker

### Construir imagem

```bash
docker build -t careerflow-api -f src/CareerFlow.Api/Dockerfile .
```

### Executar container

```bash
docker run -d \
  --name careerflow-api \
  -p 5000:8080 \
  -e Database__ConnectionString="Host=host.docker.internal;Port=5432;Database=careerflow;Username=postgres;Password=postgres;" \
  -e Jwt__SecretKey="sua-chave-aqui" \
  -e Redis__Enabled=false \
  careerflow-api
```

### Docker Compose

```bash
# Subir PostgreSQL + Redis + API
docker compose up -d

# Apenas dependências (API roda fora)
docker compose up -d postgres redis
```

---

## Variáveis de Ambiente (Referência Completa)

| Variável | Obrigatório | Padrão | Descrição |
|----------|-------------|--------|-----------|
| `Database__ConnectionString` | Sim | - | Connection string PostgreSQL |
| `Jwt__SecretKey` | Sim | - | Chave secreta JWT (mín. 64 chars) |
| `Jwt__Issuer` | Não | `CareerFlow` | Emissor do token |
| `Jwt__Audience` | Não | `CareerFlow` | Audiência do token |
| `Jwt__AccessTokenExpirationMinutes` | Não | `15` | Expiração do access token |
| `Jwt__RefreshTokenExpirationDays` | Não | `7` | Expiração do refresh token |
| `Redis__Enabled` | Não | `false` | Habilitar cache Redis |
| `Redis__ConnectionString` | Se habilitado | - | Connection string Redis |
| `Redis__InstanceName` | Não | `CareerFlow` | Prefixo das chaves Redis |
| `Storage__Provider` | Não | `Local` | Provedor de storage |
| `Storage__LocalPath` | Não | `uploads` | Caminho local para uploads |
| `Application__CorsOrigins` | Não | `*` | Origens permitidas (CORS) |
| `RateLimiting__Login__PermitLimit` | Não | `5` | Tentativas de login/minuto |
| `RateLimiting__Register__PermitLimit` | Não | `3` | Registros/hora |

---

## Troubleshooting

### Erro: `Cannot write DateTime with Kind=Unspecified`

As colunas `timestamptz` exigem `DateTimeKind.Utc`. Todos os `DateTime` devem ser criados com `DateTimeKind.Utc` ou usar `DateTime.UtcNow`.

### Erro: `An error occurred while accessing the database`

Verificar:
1. Se o banco PostgreSQL está acessível (firewall/SSL)
2. Se a connection string está correta
3. Se o banco `careerflow` foi criado

### Erro: `JWT authentication failed`

Verificar:
1. Se `Jwt__SecretKey` tem no mínimo 64 caracteres
2. Se a chave é consistente entre emissão e validação
3. Se o token não expirou

### Health Check falhando

Se o health check estiver falhando, verificar os logs do serviço no Render. O endpoint `/health` verifica apenas a conexão com o PostgreSQL.

---

## Segurança

- Senhas hasheadas com BCrypt (`$2a$12`)
- JWT com HMAC-SHA512
- Rate limiting por endpoint (login: 5/min, register: 3/hora)
- Lockout após 5 tentativas de login inválidas (15 minutos)
- Soft delete para usuários
- Headers de segurança configurados via ASP.NET Core
- CORS configurável por ambiente
