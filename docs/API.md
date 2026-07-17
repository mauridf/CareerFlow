# CareerFlow API - Documentação

## Autenticação

Todas as rotas protegidas exigem o header:

```
Authorization: Bearer {access_token}
```

### Fluxo de Autenticação

1. **Registrar** → `POST /api/v1/auth/register` → retorna `accessToken` + `refreshToken`
2. **Login** → `POST /api/v1/auth/login` → retorna `accessToken` + `refreshToken`
3. **Refresh** → `POST /api/v1/auth/refresh` → renova o access token
4. **Authenticated requests** → incluir `Authorization: Bearer {token}`

### Tokens

| Tipo | Duração | Uso |
|------|---------|-----|
| Access Token | 15 minutos (configurável) | Autenticação de requisições |
| Refresh Token | 7 dias (configurável) | Renovação do access token |

O access token contém as claims: `sub` (userId), `email`, `name`, `role`, `jti`, `iat`, `exp`.

---

## Respostas Padronizadas

### Sucesso (200/201)

```json
{
  "success": true,
  "data": { ... },
  "meta": {
    "timestamp": "2026-07-15T10:30:00Z",
    "requestId": "req-uuid"
  }
}
```

### Erro de Validação (400)

```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Dados inválidos",
    "details": [
      { "field": "email", "message": "Email inválido" },
      { "field": "password", "message": "Senha deve ter no mínimo 8 caracteres" }
    ]
  },
  "meta": {
    "requestId": "req-uuid",
    "timestamp": "2026-07-15T10:30:00Z"
  }
}
```

### Não Autorizado (401)

```json
{
  "success": false,
  "error": {
    "code": "UNAUTHORIZED",
    "message": "Credenciais inválidas"
  },
  "meta": {
    "requestId": "req-uuid",
    "timestamp": "2026-07-15T10:30:00Z"
  }
}
```

### Não Encontrado (404)

```json
{
  "success": false,
  "error": {
    "code": "NOT_FOUND",
    "message": "Perfil não encontrado"
  },
  "meta": {
    "requestId": "req-uuid",
    "timestamp": "2026-07-15T10:30:00Z"
  }
}
```

### Conflito (409)

```json
{
  "success": false,
  "error": {
    "code": "CONFLICT",
    "message": "Email já cadastrado"
  },
  "meta": {
    "requestId": "req-uuid",
    "timestamp": "2026-07-15T10:30:00Z"
  }
}
```

### Erro de Negócio (422)

```json
{
  "success": false,
  "error": {
    "code": "DOMAIN_ERROR",
    "message": "Limite máximo de 50 habilidades atingido"
  },
  "meta": {
    "requestId": "req-uuid",
    "timestamp": "2026-07-15T10:30:00Z"
  }
}
```

---

## Rate Limiting

| Escopo | Limite | Janela |
|--------|--------|--------|
| Global | 100 requisições | 1 minuto |
| Login | 5 tentativas | 1 minuto |
| Register | 3 requisições | 1 hora |

Quando excedido, retorna HTTP 429 com header `Retry-After`:

```json
{
  "success": false,
  "error": {
    "code": "RATE_LIMIT",
    "message": "Limite de requisições excedido. Tente novamente em 45 segundos."
  },
  "meta": {
    "requestId": "req-uuid",
    "timestamp": "2026-07-15T10:30:00Z"
  }
}
```

---

## Códigos de Erro

| HTTP | Code | Descrição |
|------|------|-----------|
| 400 | `VALIDATION_ERROR` | Dados inválidos (FluentValidation) |
| 401 | `UNAUTHORIZED` | Token ausente, inválido ou expirado |
| 404 | `NOT_FOUND` | Recurso não encontrado |
| 409 | `CONFLICT` | Conflito (registro duplicado) |
| 422 | `DOMAIN_ERROR` | Violação de regra de negócio |
| 429 | `RATE_LIMIT` | Limite de requisições excedido |
| 500 | `INTERNAL_ERROR` | Erro interno do servidor |

---

## Políticas de Autorização

| Policy | Acesso |
|--------|--------|
| `Authenticated` | Qualquer usuário logado |
| `Premium` | Usuários premium ou admin |
| `Admin` | Apenas administradores |

Rotas admin (`/api/v1/admin/*`) exigem role `Admin`.

---

## Headers de Resposta

| Header | Descrição |
|--------|-----------|
| `X-Request-Id` | ID único de correlação da requisição |
| `X-RateLimit-Limit` | Limite de requisições por janela |
| `X-RateLimit-Remaining` | Requisições restantes na janela |
| `Retry-After` | Segundos até reset do rate limit (429) |

---

## Observabilidade

### Health Check

```
GET /health
```

Retorna `200 OK` com status do banco PostgreSQL.

### Logging

A API utiliza Serilog com os seguintes sinks configurados:

- Console (estruturado)
- Arquivo (`logs/careerflow-.log`, com rolling diário)
- PostgreSQL (opcional, via sink próprio)

Cada requisição gera um log estruturado com:
- Método HTTP, path, status code, duração
- Correlation ID
- User ID (quando autenticado)
- Erros com stack trace completos
