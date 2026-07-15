-- ============================================
-- Extensões PostgreSQL para o CareerFlow
-- ============================================

-- UUID generation (gen_random_uuid())
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Full text search (para busca de currículos)
CREATE EXTENSION IF NOT EXISTS "unaccent";

-- Trigram similarity (para sugestões de habilidades)
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- Verificar extensões instaladas
SELECT extname, extversion 
FROM pg_extension 
ORDER BY extname;