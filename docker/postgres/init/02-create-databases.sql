-- ============================================
-- Databases adicionais (testes, shadow)
-- ============================================

-- Database para testes de integração
CREATE DATABASE careerflow_test
    WITH 
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'pt_BR.UTF-8'
    LC_CTYPE = 'pt_BR.UTF-8'
    TEMPLATE template0;

-- Database shadow para EF Core migrations (opcional)
CREATE DATABASE careerflow_shadow
    WITH 
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'pt_BR.UTF-8'
    LC_CTYPE = 'pt_BR.UTF-8'
    TEMPLATE template0;

-- Conceder privilégios
GRANT ALL PRIVILEGES ON DATABASE careerflow_test TO postgres;
GRANT ALL PRIVILEGES ON DATABASE careerflow_shadow TO postgres;