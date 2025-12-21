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

BEGIN;
UPDATE public.mt_doc_employee
SET data =
    jsonb_set(
        data,
        '{Status}',
        to_jsonb(
            CASE (data->>'Status')::int
                WHEN 1 THEN 'Unactivated'
                WHEN 2 THEN 'Active'
                WHEN 3 THEN 'Fired'
                ELSE 'Undefined'
            END
        ),
        true
    )
WHERE data ? 'Status';
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











CREATE FOREIGN TABLE public.mt_doc_holiday_ft (
    tenant_id        varchar NOT NULL,
    id               uuid    NOT NULL,
    data             jsonb   NOT NULL,
    mt_last_modified timestamptz,
    mt_dotnet_type   varchar,
    mt_deleted       boolean,
    mt_deleted_at    timestamptz,
	date				date
)
SERVER remote_allhands_prod
OPTIONS (schema_name 'public', table_name 'mt_doc_holiday');

BEGIN;

-- Optional: speed up big loads (only do this if you understand the risk)
-- SET LOCAL synchronous_commit = off;

INSERT INTO public.mt_doc_holiday (
    tenant_id,
    id,
    data,
    mt_last_modified,
    mt_dotnet_type,
    mt_deleted,
    mt_deleted_at,
	date
)
SELECT
    tenant_id,
    id,
    data,
    mt_last_modified,
    mt_dotnet_type,
    mt_deleted,
    mt_deleted_at,
	date
FROM public.mt_doc_holiday_ft
ON CONFLICT (tenant_id, id)
DO UPDATE SET
    data             = EXCLUDED.data,
    mt_last_modified = EXCLUDED.mt_last_modified,
    mt_dotnet_type   = EXCLUDED.mt_dotnet_type,
    mt_deleted       = EXCLUDED.mt_deleted,
    mt_deleted_at    = EXCLUDED.mt_deleted_at,
	date				= EXCLUDED.date;

COMMIT;







CREATE FOREIGN TABLE public.mt_doc_timeoffbalance_ft (
    tenant_id        varchar NOT NULL,
    id               uuid    NOT NULL,
    data             jsonb   NOT NULL,
    mt_last_modified timestamptz,
    mt_dotnet_type   varchar,
	last_auto_update timestamptz
)
SERVER remote_allhands_prod
OPTIONS (schema_name 'public', table_name 'mt_doc_timeoffbalance');

BEGIN;

-- Optional: speed up big loads (only do this if you understand the risk)
-- SET LOCAL synchronous_commit = off;

INSERT INTO public.mt_doc_timeoffbalance (
    tenant_id,
    id,
    data,
    mt_last_modified,
    mt_dotnet_type,
	last_auto_update
)
SELECT
    tenant_id,
    id,
    data,
    mt_last_modified,
    mt_dotnet_type,
	last_auto_update
FROM public.mt_doc_timeoffbalance_ft
ON CONFLICT (tenant_id, id)
DO UPDATE SET
    data             = EXCLUDED.data,
    mt_last_modified = EXCLUDED.mt_last_modified,
    mt_dotnet_type   = EXCLUDED.mt_dotnet_type,
	last_auto_update = EXCLUDED.last_auto_update;

COMMIT;











CREATE FOREIGN TABLE public.mt_doc_timeoffrequest_ft (
    tenant_id        varchar NOT NULL,
    id               uuid    NOT NULL,
    data             jsonb   NOT NULL,
    mt_last_modified timestamptz,
    mt_dotnet_type   varchar,
	start_date timestamptz,
	end_date timestamptz
)
SERVER remote_allhands_prod
OPTIONS (schema_name 'public', table_name 'mt_doc_timeoffrequest');

BEGIN;

-- Optional: speed up big loads (only do this if you understand the risk)
-- SET LOCAL synchronous_commit = off;

INSERT INTO public.mt_doc_timeoffrequest (
    tenant_id,
    id,
    data,
    mt_last_modified,
    mt_dotnet_type,
	start_date,
	end_date
)
SELECT
    tenant_id,
    id,
    data,
    mt_last_modified,
    mt_dotnet_type,
	start_date,
	end_date
FROM public.mt_doc_timeoffrequest_ft
ON CONFLICT (tenant_id, id)
DO UPDATE SET
    data             = EXCLUDED.data,
    mt_last_modified = EXCLUDED.mt_last_modified,
    mt_dotnet_type   = EXCLUDED.mt_dotnet_type,
	start_date = EXCLUDED.start_date,
	end_date = EXCLUDED.end_date;

COMMIT;











CREATE FOREIGN TABLE public.mt_doc_timeofftype_ft (
    tenant_id        varchar NOT NULL,
    id               uuid    NOT NULL,
    data             jsonb   NOT NULL,
    mt_last_modified timestamptz,
    mt_dotnet_type   varchar,
    mt_deleted       boolean,
    mt_deleted_at    timestamptz
)
SERVER remote_allhands_prod
OPTIONS (schema_name 'public', table_name 'mt_doc_timeofftype');

BEGIN;

-- Optional: speed up big loads (only do this if you understand the risk)
-- SET LOCAL synchronous_commit = off;

INSERT INTO public.mt_doc_timeofftype (
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
FROM public.mt_doc_timeofftype_ft
ON CONFLICT (tenant_id, id)
DO UPDATE SET
    data             = EXCLUDED.data,
    mt_last_modified = EXCLUDED.mt_last_modified,
    mt_dotnet_type   = EXCLUDED.mt_dotnet_type,
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
WHERE type IS NULL OR type = 'time_off_balance' OR type = 'time_off_request'
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
