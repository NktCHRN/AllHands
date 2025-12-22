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
    password '${DB_PASSWORD}'
);







CREATE FOREIGN TABLE public."AspNetUsers_ft" (
    "Id"            uuid NOT NULL,
    "GlobalUserId"  uuid NOT NULL
)
SERVER remote_allhands_prod
OPTIONS (
    schema_name 'public',
    table_name  'AspNetUsers'
);

CREATE FOREIGN TABLE public."AspNetUserRoles_ft" (
    "UserId" uuid NOT NULL,
    "RoleId" uuid NOT NULL
)
SERVER remote_allhands_prod
OPTIONS (
    schema_name 'public',
    table_name  'AspNetUserRoles'
);



CREATE FOREIGN TABLE public.mt_doc_employee_ft (
    tenant_id        varchar NOT NULL,
    id               uuid    NOT NULL,
    data             jsonb   NOT NULL,
    mt_last_modified timestamptz,
    mt_dotnet_type   varchar,
    mt_deleted       boolean,
    mt_deleted_at    timestamptz
)
SERVER remote_allhands_prod
OPTIONS (schema_name 'public', table_name 'mt_doc_employee');

BEGIN;

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


UPDATE public.mt_doc_employee e
SET data =
    e.data
    || jsonb_build_object(
        'GlobalUserId', u."GlobalUserId",
        'RoleId', r."RoleId"
    )
FROM public."AspNetUsers_ft" u
LEFT JOIN LATERAL (
    SELECT ur."RoleId"
    FROM public."AspNetUserRoles_ft" ur
    WHERE ur."UserId" = u."Id"
    ORDER BY ur."RoleId"      -- deterministic "first"
    LIMIT 1
) r ON true
WHERE
    (e.data ->> 'UserId')::uuid = u."Id";


COMMIT;













CREATE FOREIGN TABLE public.mt_doc_company_ft (
    tenant_id        varchar NOT NULL,
    id               uuid    NOT NULL,
    data             jsonb   NOT NULL,
    mt_last_modified timestamptz,
    mt_dotnet_type   varchar,
    mt_deleted       boolean,
    mt_deleted_at    timestamptz
)
SERVER remote_allhands_prod
OPTIONS (schema_name 'public', table_name 'mt_doc_company');

BEGIN;

-- Optional: speed up big loads (only do this if you understand the risk)
-- SET LOCAL synchronous_commit = off;

INSERT INTO public.mt_doc_company (
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
FROM public.mt_doc_company_ft
ON CONFLICT (tenant_id, id)
DO UPDATE SET
    data             = EXCLUDED.data,
    mt_last_modified = EXCLUDED.mt_last_modified,
    mt_dotnet_type   = EXCLUDED.mt_dotnet_type,
    mt_deleted       = EXCLUDED.mt_deleted,
    mt_deleted_at    = EXCLUDED.mt_deleted_at;

COMMIT;












CREATE FOREIGN TABLE public.mt_doc_position_ft (
    tenant_id        varchar NOT NULL,
    id               uuid    NOT NULL,
    data             jsonb   NOT NULL,
    mt_last_modified timestamptz,
    mt_dotnet_type   varchar,
    mt_deleted       boolean,
    mt_deleted_at    timestamptz
)
SERVER remote_allhands_prod
OPTIONS (schema_name 'public', table_name 'mt_doc_position');

BEGIN;


INSERT INTO public.mt_doc_position (
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
FROM public.mt_doc_position_ft
ON CONFLICT (tenant_id, id)
DO UPDATE SET
    data             = EXCLUDED.data,
    mt_last_modified = EXCLUDED.mt_last_modified,
    mt_dotnet_type   = EXCLUDED.mt_dotnet_type,
    mt_deleted       = EXCLUDED.mt_deleted,
    mt_deleted_at    = EXCLUDED.mt_deleted_at;

COMMIT;









CREATE FOREIGN TABLE public."AspNetRoles_ft" (
    "Id"        uuid    NOT NULL,
    "CompanyId" uuid    NOT NULL,
    "Name"      varchar(256),
    "IsDefault" boolean NOT NULL,
	"DeletedAt" timestamptz NULL
)
SERVER remote_allhands_prod
OPTIONS (
    schema_name 'public',
    table_name  'AspNetRoles'
);

BEGIN;

-- Optional: speed up bulk load
-- SET LOCAL synchronous_commit = off;

INSERT INTO public.mt_doc_role (
    tenant_id,
    id,
    data,
    mt_last_modified,
    mt_dotnet_type,
    mt_deleted,
    mt_deleted_at
)
SELECT
    r."CompanyId"               AS tenant_id,
    r."Id"                      AS id,

    -- Build Marten document JSON
    jsonb_build_object(
        'Id',        r."Id",
        'Name',      r."Name",
        'CompanyId', r."CompanyId",
        'IsDefault', r."IsDefault"
    )                            AS data,

    now()                        AS mt_last_modified,
    'AllHands.EmployeeService.Domain.Models.Role'                         AS mt_dotnet_type,
r."DeletedAt" IS NOT NULL      AS mt_deleted,
    r."DeletedAt"                 AS mt_deleted_at

FROM public."AspNetRoles_ft" r

ON CONFLICT (tenant_id, id)
DO UPDATE SET
    data             = EXCLUDED.data,
    mt_last_modified = EXCLUDED.mt_last_modified,
    mt_deleted       = EXCLUDED.mt_deleted,
    mt_deleted_at    = EXCLUDED.mt_deleted_at;

COMMIT;















CREATE FOREIGN TABLE public.mt_streams_ft (
    tenant_id        varchar        NOT NULL,
    id               uuid           NOT NULL,
    type             varchar,
    version          bigint,
    "timestamp"      timestamptz    NOT NULL,
    snapshot         jsonb,
    snapshot_version integer,
    created          timestamptz    NOT NULL,
    is_archived      boolean
)
SERVER remote_allhands_prod
OPTIONS (
    schema_name 'public',
    table_name  'mt_streams'
);

BEGIN;

-- Optional: speed up bulk loads (only if acceptable)
-- SET LOCAL synchronous_commit = off;

INSERT INTO public.mt_streams (
    tenant_id,
    id,
    type,
    version,
    "timestamp",
    snapshot,
    snapshot_version,
    created,
    is_archived
)
SELECT
    tenant_id,
    id,
    type,
    version,
    "timestamp",
    snapshot,
    snapshot_version,
    created,
    is_archived
FROM public.mt_streams_ft
WHERE type IS NULL OR type = 'employee'
ON CONFLICT (tenant_id, id)
DO UPDATE SET
    type             = EXCLUDED.type,
    version          = EXCLUDED.version,
    "timestamp"      = EXCLUDED."timestamp",
    snapshot         = EXCLUDED.snapshot,
    snapshot_version = EXCLUDED.snapshot_version,
    created          = EXCLUDED.created,
    is_archived      = EXCLUDED.is_archived;

COMMIT;




CREATE FOREIGN TABLE public.mt_events_ft (
    seq_id         bigint         NOT NULL,
    id             uuid           NOT NULL,
    stream_id      uuid,
    version        bigint         NOT NULL,
    data           jsonb          NOT NULL,
    type           varchar(500)   NOT NULL,
    "timestamp"    timestamptz    NOT NULL,
    tenant_id      varchar,
    mt_dotnet_type varchar,
    is_archived    boolean
)
SERVER remote_allhands_prod
OPTIONS (
    schema_name 'public',
    table_name  'mt_events'
);

BEGIN;

-- Optional for bulk loads
-- SET LOCAL synchronous_commit = off;

INSERT INTO public.mt_events (
    seq_id,
    id,
    stream_id,
    version,
    data,
    type,
    "timestamp",
    tenant_id,
    mt_dotnet_type,
    is_archived
)
SELECT
    seq_id,
    id,
    stream_id,
    version,
    data,
    type,
    "timestamp",
    tenant_id,
    mt_dotnet_type,
    is_archived
FROM public.mt_events_ft e
WHERE EXISTS (
    SELECT 1
    FROM public.mt_streams s
    WHERE s.id = e.stream_id
)
ON CONFLICT (seq_id)
DO UPDATE SET
    id             = EXCLUDED.id,
    stream_id      = EXCLUDED.stream_id,
    version        = EXCLUDED.version,
    data           = EXCLUDED.data,
    type           = EXCLUDED.type,
    "timestamp"    = EXCLUDED."timestamp",
    tenant_id      = EXCLUDED.tenant_id,
    mt_dotnet_type = EXCLUDED.mt_dotnet_type,
    is_archived    = EXCLUDED.is_archived;

COMMIT;

DROP SERVER IF EXISTS remote_allhands_prod CASCADE;
