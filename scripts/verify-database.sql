-- ============================================
-- Script de verificação do banco de dados
-- ============================================

-- Listar todas as tabelas
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
ORDER BY table_name;

-- Contar registros por tabela
SELECT 'users' AS tabela, COUNT(*) AS registros FROM users
UNION ALL
SELECT 'persons', COUNT(*) FROM persons
UNION ALL
SELECT 'skills', COUNT(*) FROM skills
UNION ALL
SELECT 'experiences', COUNT(*) FROM experiences
UNION ALL
SELECT 'educations', COUNT(*) FROM educations
UNION ALL
SELECT 'certificates', COUNT(*) FROM certificates
UNION ALL
SELECT 'languages', COUNT(*) FROM languages
UNION ALL
SELECT 'social_networks', COUNT(*) FROM social_networks
UNION ALL
SELECT 'resume_views', COUNT(*) FROM resume_views
UNION ALL
SELECT 'resume_analytics', COUNT(*) FROM resume_analytics
UNION ALL
SELECT 'resume_suggestions', COUNT(*) FROM resume_suggestions
UNION ALL
SELECT 'activity_logs', COUNT(*) FROM activity_logs
UNION ALL
SELECT 'outbox_messages', COUNT(*) FROM outbox_messages
ORDER BY tabela;

-- Verificar extensões instaladas
SELECT extname, extversion FROM pg_extension;

-- Verificar índices
SELECT tablename, indexname, indexdef 
FROM pg_indexes 
WHERE schemaname = 'public' 
ORDER BY tablename, indexname;