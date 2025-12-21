DROP SERVER IF EXISTS remote_allhands_prod CASCADE;

CREATE EXTENSION IF NOT EXISTS postgres_fdw;
CREATE SERVER remote_allhands_prod
FOREIGN DATA WRAPPER postgres_fdw
OPTIONS (
    host '${DB_HOST}',
    dbname '${DB_NAME}',
    port '5432'
);
CREATE USER MAPPING FOR postgres
SERVER remote_allhands_prod
OPTIONS (
    user 'postgres',
    password ''
);









CREATE FOREIGN TABLE public.mt_doc_employee_ft (
    tenant_id        varchar NOT NULL,
    id               uuid    NOT NULL,
    data             jsonb   NOT NULL,
    mt_last_modified timestamptz,
    mt_dotnet_type   varchar,
    mt_version       integer NOT NULL,
    mt_deleted       boolean,
    mt_deleted_at    timestamptz
)
SERVER remote_allhands_prod
OPTIONS (schema_name 'public', table_name 'mt_doc_employee');

BEGIN;

-- Optional: speed up big loads (only do this if you understand the risk)
-- SET LOCAL synchronous_commit = off;

INSERT INTO public.mt_doc_employee (
    tenant_id,
    id,
    data,
    mt_last_modified,
    mt_dotnet_type,
    mt_deleted,
    mt_deleted_at
)
SELECT
    tenant_id,
    id,
    data,
    mt_last_modified,
    mt_dotnet_type,
    mt_deleted,
    mt_deleted_at
FROM public.mt_doc_employee_ft
ON CONFLICT (tenant_id, id)
DO UPDATE SET
    data             = EXCLUDED.data,
    mt_last_modified = EXCLUDED.mt_last_modified,
    mt_dotnet_type   = EXCLUDED.mt_dotnet_type,
    mt_deleted       = EXCLUDED.mt_deleted,
    mt_deleted_at    = EXCLUDED.mt_deleted_at;

COMMIT;






CREATE FOREIGN TABLE IF NOT EXISTS public.mt_doc_newspost_ft (
    tenant_id        varchar      NOT NULL,
    id               uuid         NOT NULL,
    data             jsonb        NOT NULL,
    mt_last_modified timestamptz,
    mt_version       uuid,
    mt_dotnet_type   varchar,
    mt_deleted       boolean,
    mt_deleted_at    timestamptz
)
SERVER remote_allhands_prod
OPTIONS (schema_name 'public', table_name 'mt_doc_newspost');

BEGIN;

INSERT INTO public.mt_doc_newspost (
    tenant_id,
    id,
    data,
    mt_last_modified,
    mt_dotnet_type,
    mt_deleted,
    mt_deleted_at
)
SELECT
    tenant_id,
    id,
    data,
    mt_last_modified,
    mt_dotnet_type,
    mt_deleted,
    mt_deleted_at
FROM public.mt_doc_newspost_ft
ON CONFLICT (tenant_id, id)
DO UPDATE SET
    data             = EXCLUDED.data,
    mt_last_modified = EXCLUDED.mt_last_modified,
    mt_dotnet_type   = EXCLUDED.mt_dotnet_type,
    mt_deleted       = EXCLUDED.mt_deleted,
    mt_deleted_at    = EXCLUDED.mt_deleted_at;

COMMIT;




DROP SERVER IF EXISTS remote_allhands_prod CASCADE;
