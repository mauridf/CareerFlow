# 🚀 CareerFlow API

API REST para gestão profissional e criação de currículos otimizados para ATS (Applicant Tracking Systems).

## 📋 Funcionalidades

✅ Cadastro e autenticação de usuários  
✅ Gestão completa de perfil profissional  
✅ Upload de arquivos (fotos, diplomas, certificados)  
✅ Geração de currículos ATS em múltiplos formatos  
✅ Dashboard com análises e métricas  
✅ Exportação de dados em JSON, PDF e texto  

## 🏗️ Arquitetura

- Clean Architecture com separação em camadas  
- .NET 8.0 com ASP.NET Core Web API  
- PostgreSQL com Entity Framework Core  
- JWT Authentication para segurança  
- AutoMapper para mapeamento de DTOs  
- FluentValidation para validações  
- Serilog para logging estruturado  

## 🚀 Como Executar

### Pré-requisitos

- .NET 8.0 SDK  
- Docker e Docker Compose (opcional)  
- PostgreSQL 15+  

### Método 1: Docker Compose (Recomendado)

```bash
# Clone o repositório
git clone <repository-url>
cd CareerFlow

# Execute o script de setup
chmod +x scripts/setup-dev.sh
./scripts/setup-dev.sh

# Inicie os containers
docker-compose up
```

### Método 2: Desenvolvimento Local

```bash
# Restaurar pacotes
dotnet restore

# Configurar banco de dados
cd src/CareerFlow.Infrastructure
dotnet ef database update --startup-project ../CareerFlow.API

# Executar a API
cd ../CareerFlow.API
dotnet run
```

## 📡 Endpoints Principais

- Autenticação: `POST /api/auth/register`, `POST /api/auth/login`  
- Perfil: `GET /api/profile/dashboard/stats`, `GET /api/profile/resume`  
- Habilidades: `GET /api/skills`, `POST /api/skills`  
- Experiências: `GET /api/experiences`, `POST /api/experiences`  
- ATS: `GET /api/ats/resume`, `GET /api/ats/resume/pdf`  
- Arquivos: `POST /api/files/upload`  

## 🧪 Testes

```bash
# Executar todos os testes
./scripts/run-tests.sh

# Ou individualmente
dotnet test tests/CareerFlow.Domain.Tests
dotnet test tests/CareerFlow.Application.Tests
dotnet test tests/CareerFlow.API.Tests
```

## 🐳 Deploy no Railway

1. Crie uma conta no Railway  
2. Conecte seu repositório GitHub  
3. Configure as variáveis de ambiente:
   - `DATABASE_URL` (PostgreSQL connection string)
   - `JWT_SECRET` (chave secreta para JWT)
   - `JWT_ISSUER`, `JWT_AUDIENCE`, `JWT_EXPIRY_MINUTES`
4. Deploy automático será realizado  

### Variáveis de Ambiente para Railway

```env
DATABASE_URL=postgresql://user:password@host:port/database
JWT_SECRET=your-super-secret-key-at-least-32-characters-long
JWT_ISSUER=CareerFlowAPI
JWT_AUDIENCE=CareerFlowUsers
JWT_EXPIRY_MINUTES=60
FILE_STORAGE_PATH=uploads
MAX_FILE_SIZE_MB=10
FRONTEND_URL=https://seusite.com
```

## 📁 Estrutura do Projeto

```text
CareerFlow/
├── src/
│   ├── CareerFlow.API/            # Camada de apresentação
│   ├── CareerFlow.Application/    # Casos de uso e serviços
│   ├── CareerFlow.Domain/         # Entidades e regras de negócio
│   └── CareerFlow.Infrastructure/# Implementações (EF, serviços externos)
├── tests/                         # Testes unitários e de integração
├── scripts/                       # Scripts de automação
├── uploads/                       # Arquivos enviados
├── Dockerfile                     # Configuração Docker
├── docker-compose.yml             # Orquestração de containers
└── README.md                      # Documentação
```

## 🔧 Configuração de Desenvolvimento

- Clone o repositório  
- Execute `./scripts/setup-dev.sh`  
- Configure as variáveis de ambiente em `appsettings.Development.json`  
- Execute `docker-compose up` ou `dotnet run`  

## 📊 Health Check

- Endpoint: `GET /health`  
- Retorna: `"CareerFlow API is running!"`  

## 🛠️ Tecnologias Utilizadas

- .NET 8.0  
- ASP.NET Core  
- Entity Framework Core  
- PostgreSQL  
- JWT  
- AutoMapper  
- FluentValidation  
- Serilog  
- Swagger  
- Docker  
- Railway  

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## 🤝 Contribuição

1. Fork o projeto  
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)  
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)  
4. Push para a branch (`git push origin feature/AmazingFeature`)  
5. Abra um Pull Request  
