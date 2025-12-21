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




CREATE FOREIGN TABLE public."GlobalUsers_ft" (
    "Id"              uuid          NOT NULL,
    "Email"           varchar(256)  NOT NULL,
    "NormalizedEmail" varchar(256)  NOT NULL,
    "DefaultCompanyId"  uuid          NOT NULL
)
SERVER remote_allhands_prod
OPTIONS (
    schema_name 'public',
    table_name  'GlobalUsers'
);


BEGIN;

-- Optional for large loads
-- SET LOCAL synchronous_commit = off;

INSERT INTO public."GlobalUsers" (
    "Id",
    "Email",
    "NormalizedEmail",
    "DefaultCompanyId"
)
SELECT
    "Id",
    "Email",
    "NormalizedEmail",
    "DefaultCompanyId"
FROM public."GlobalUsers_ft"
ON CONFLICT ("Id")
DO UPDATE SET
    "Email"           = EXCLUDED."Email",
    "NormalizedEmail" = EXCLUDED."NormalizedEmail",
    "DefaultCompanyId"  = EXCLUDED."DefaultCompanyId";

COMMIT;














CREATE FOREIGN TABLE public."AspNetRoles_ft" (
    "Id"                uuid           NOT NULL,
    "CompanyId"         uuid           NOT NULL,
    "DeletedAt"         timestamptz,
    "Name"              varchar(256),
    "NormalizedName"    varchar(256),
    "ConcurrencyStamp"  text,
    "IsDefault"         boolean        NOT NULL,
    "CreatedAt"         timestamptz    NOT NULL,
    "CreatedByUserId"   uuid,
    "DeletedByUserId"   uuid,
    "UpdatedAt"         timestamptz,
    "UpdatedByUserId"   uuid
)
SERVER remote_allhands_prod
OPTIONS (
    schema_name 'public',
    table_name  'AspNetRoles'
);

BEGIN;

-- Optional: faster bulk load
-- SET LOCAL synchronous_commit = off;

INSERT INTO public."AspNetRoles" (
    "Id",
    "CompanyId",
    "DeletedAt",
    "Name",
    "NormalizedName",
    "ConcurrencyStamp",
    "IsDefault",
    "CreatedAt",
    "CreatedByUserId",
    "DeletedByUserId",
    "UpdatedAt",
    "UpdatedByUserId"
)
SELECT
    "Id",
    "CompanyId",
    "DeletedAt",
    "Name",
    "NormalizedName",
    "ConcurrencyStamp",
    "IsDefault",
    "CreatedAt",
    "CreatedByUserId",
    "DeletedByUserId",
    "UpdatedAt",
    "UpdatedByUserId"
FROM public."AspNetRoles_ft"
ON CONFLICT ("Id")
DO UPDATE SET
    "CompanyId"        = EXCLUDED."CompanyId",
    "DeletedAt"        = EXCLUDED."DeletedAt",
    "Name"             = EXCLUDED."Name",
    "NormalizedName"   = EXCLUDED."NormalizedName",
    "ConcurrencyStamp" = EXCLUDED."ConcurrencyStamp",
    "IsDefault"        = EXCLUDED."IsDefault",
    "CreatedAt"        = EXCLUDED."CreatedAt",
    "CreatedByUserId"  = EXCLUDED."CreatedByUserId",
    "DeletedByUserId"  = EXCLUDED."DeletedByUserId",
    "UpdatedAt"        = EXCLUDED."UpdatedAt",
    "UpdatedByUserId"  = EXCLUDED."UpdatedByUserId";

COMMIT;












CREATE FOREIGN TABLE public."AllHandsRoleClaims_ft" (
    "Id"         uuid          NOT NULL,
    "RoleId"     uuid          NOT NULL,
    "ClaimType"  varchar(255)  NOT NULL,
    "ClaimValue" varchar(255)  NOT NULL
)
SERVER remote_allhands_prod
OPTIONS (
    schema_name 'public',
    table_name  'AllHandsRoleClaims'
);

BEGIN;

-- Optional: faster bulk copy
-- SET LOCAL synchronous_commit = off;

INSERT INTO public."AllHandsRoleClaims" (
    "Id",
    "RoleId",
    "ClaimType",
    "ClaimValue"
)
SELECT
    "Id",
    "RoleId",
    "ClaimType",
    "ClaimValue"
FROM public."AllHandsRoleClaims_ft"
ON CONFLICT ("Id")
DO UPDATE SET
    "RoleId"     = EXCLUDED."RoleId",
    "ClaimType"  = EXCLUDED."ClaimType",
    "ClaimValue" = EXCLUDED."ClaimValue";

COMMIT;









CREATE FOREIGN TABLE public."mt_doc_employee_ft" (
    "tenant_id"        varchar        NOT NULL,
    "id"               uuid           NOT NULL,
    "data"             jsonb          NOT NULL,
    "mt_last_modified" timestamptz,
    "mt_dotnet_type"   varchar,
    "mt_version"       integer        NOT NULL,
    "mt_deleted"       boolean,
    "mt_deleted_at"    timestamptz
)
SERVER remote_allhands_prod
OPTIONS (
    schema_name 'public',
    table_name  'mt_doc_employee'
);




CREATE FOREIGN TABLE public."AspNetUsers_ft" (
    "Id"                         uuid          NOT NULL,
    "FirstName"                  varchar(255)  NOT NULL,
    "MiddleName"                 varchar(255),
    "LastName"                   varchar(255)  NOT NULL,
    "CompanyId"                  uuid          NOT NULL,
    "DeletedAt"                  timestamptz,
    "UserName"                   varchar(256),
    "NormalizedUserName"          varchar(256),
    "Email"                      varchar(256),
    "NormalizedEmail"            varchar(256),
    "EmailConfirmed"             boolean       NOT NULL,
    "PasswordHash"               text,
    "SecurityStamp"              text,
    "ConcurrencyStamp"            text,
    "PhoneNumber"                text,
    "PhoneNumberConfirmed"       boolean       NOT NULL,
    "TwoFactorEnabled"           boolean       NOT NULL,
    "LockoutEnd"                 timestamptz,
    "LockoutEnabled"             boolean       NOT NULL,
    "AccessFailedCount"           integer       NOT NULL,
    "LastPasswordResetRequestedAt" timestamptz,
    "GlobalUserId"               uuid          NOT NULL,
    "IsInvitationAccepted"       boolean       NOT NULL,
    "DeactivatedAt"              timestamptz
)
SERVER remote_allhands_prod
OPTIONS (
    schema_name 'public',
    table_name  'AspNetUsers'
);




BEGIN;

INSERT INTO public."AspNetUsers" (
    "Id",
    "FirstName",
    "MiddleName",
    "LastName",
    "CompanyId",
    "DeletedAt",
    "UserName",
    "NormalizedUserName",
    "Email",
    "NormalizedEmail",
    "EmailConfirmed",
    "PasswordHash",
    "SecurityStamp",
    "ConcurrencyStamp",
    "PhoneNumber",
    "PhoneNumberConfirmed",
    "TwoFactorEnabled",
    "LockoutEnd",
    "LockoutEnabled",
    "AccessFailedCount",
    "LastPasswordResetRequestedAt",
    "GlobalUserId",
    "IsInvitationAccepted",
    "DeactivatedAt",
    "EmployeeId"
)
SELECT
    u."Id",
    u."FirstName",
    u."MiddleName",
    u."LastName",
    u."CompanyId",
    u."DeletedAt",
    u."UserName",
    u."NormalizedUserName",
    u."Email",
    u."NormalizedEmail",
    u."EmailConfirmed",
    u."PasswordHash",
    u."SecurityStamp",
    u."ConcurrencyStamp",
    u."PhoneNumber",
    u."PhoneNumberConfirmed",
    u."TwoFactorEnabled",
    u."LockoutEnd",
    u."LockoutEnabled",
    u."AccessFailedCount",
    u."LastPasswordResetRequestedAt",
    u."GlobalUserId",
    u."IsInvitationAccepted",
    u."DeactivatedAt",

    -- 🎯 EmployeeId resolved from JSONB
    (e."data" ->> 'Id')::uuid AS "EmployeeId"

FROM public."AspNetUsers_ft" u
LEFT JOIN public."mt_doc_employee_ft" e
    ON (e."data" ->> 'UserId')::uuid = u."Id"

ON CONFLICT ("Id")
DO UPDATE SET
    "FirstName"                   = EXCLUDED."FirstName",
    "MiddleName"                  = EXCLUDED."MiddleName",
    "LastName"                    = EXCLUDED."LastName",
    "CompanyId"                   = EXCLUDED."CompanyId",
    "DeletedAt"                   = EXCLUDED."DeletedAt",
    "UserName"                    = EXCLUDED."UserName",
    "NormalizedUserName"          = EXCLUDED."NormalizedUserName",
    "Email"                       = EXCLUDED."Email",
    "NormalizedEmail"             = EXCLUDED."NormalizedEmail",
    "EmailConfirmed"              = EXCLUDED."EmailConfirmed",
    "PasswordHash"                = EXCLUDED."PasswordHash",
    "SecurityStamp"               = EXCLUDED."SecurityStamp",
    "ConcurrencyStamp"            = EXCLUDED."ConcurrencyStamp",
    "PhoneNumber"                 = EXCLUDED."PhoneNumber",
    "PhoneNumberConfirmed"        = EXCLUDED."PhoneNumberConfirmed",
    "TwoFactorEnabled"            = EXCLUDED."TwoFactorEnabled",
    "LockoutEnd"                  = EXCLUDED."LockoutEnd",
    "LockoutEnabled"              = EXCLUDED."LockoutEnabled",
    "AccessFailedCount"           = EXCLUDED."AccessFailedCount",
    "LastPasswordResetRequestedAt"= EXCLUDED."LastPasswordResetRequestedAt",
    "GlobalUserId"                = EXCLUDED."GlobalUserId",
    "IsInvitationAccepted"        = EXCLUDED."IsInvitationAccepted",
    "DeactivatedAt"               = EXCLUDED."DeactivatedAt",
    "EmployeeId"                  = EXCLUDED."EmployeeId";

COMMIT;



















CREATE FOREIGN TABLE public."Invitations_ft" (
    "Id"        uuid          NOT NULL,
    "TokenHash" varchar(64)   NOT NULL,
    "IssuedAt"  timestamptz   NOT NULL,
    "ExpiresAt" timestamptz   NOT NULL,
    "IsUsed"    boolean       NOT NULL,
    "IssuerId"  uuid          NOT NULL,
    "UserId"    uuid          NOT NULL
)
SERVER remote_allhands_prod
OPTIONS (
    schema_name 'public',
    table_name  'Invitations'
);


BEGIN;

-- Optional: faster bulk copy
-- SET LOCAL synchronous_commit = off;

INSERT INTO public."Invitations" (
    "Id",
    "TokenHash",
    "IssuedAt",
    "ExpiresAt",
    "IsUsed",
    "IssuerId",
    "UserId"
)
SELECT
    "Id",
    "TokenHash",
    "IssuedAt",
    "ExpiresAt",
    "IsUsed",
    "IssuerId",
    "UserId"
FROM public."Invitations_ft"
ON CONFLICT ("Id")
DO UPDATE SET
    "TokenHash" = EXCLUDED."TokenHash",
    "IssuedAt"  = EXCLUDED."IssuedAt",
    "ExpiresAt" = EXCLUDED."ExpiresAt",
    "IsUsed"    = EXCLUDED."IsUsed",
    "IssuerId"  = EXCLUDED."IssuerId",
    "UserId"    = EXCLUDED."UserId";

COMMIT;

















CREATE FOREIGN TABLE public."PasswordResetTokens_ft" (
    "Id"           uuid         NOT NULL,
    "TokenHash"    varchar(64)  NOT NULL,
    "IssuedAt"     timestamptz  NOT NULL,
    "ExpiresAt"    timestamptz  NOT NULL,
    "IsUsed"       boolean      NOT NULL,
    "GlobalUserId" uuid         NOT NULL
)
SERVER remote_allhands_prod
OPTIONS (
    schema_name 'public',
    table_name  'PasswordResetTokens'
);


BEGIN;

-- Optional: speed up bulk copy
-- SET LOCAL synchronous_commit = off;

INSERT INTO public."PasswordResetTokens" (
    "Id",
    "TokenHash",
    "IssuedAt",
    "ExpiresAt",
    "IsUsed",
    "GlobalUserId"
)
SELECT
    "Id",
    "TokenHash",
    "IssuedAt",
    "ExpiresAt",
    "IsUsed",
    "GlobalUserId"
FROM public."PasswordResetTokens_ft"
ON CONFLICT ("Id")
DO UPDATE SET
    "TokenHash"    = EXCLUDED."TokenHash",
    "IssuedAt"     = EXCLUDED."IssuedAt",
    "ExpiresAt"    = EXCLUDED."ExpiresAt",
    "IsUsed"       = EXCLUDED."IsUsed",
    "GlobalUserId" = EXCLUDED."GlobalUserId";

COMMIT;


















CREATE FOREIGN TABLE public."Sessions_ft" (
    "Key"         uuid        NOT NULL,
    "TicketValue" bytea       NOT NULL,
    "IssuedAt"    timestamptz,
    "ExpiresAt"   timestamptz,
    "UserId"      uuid        NOT NULL,
    "IsRevoked"   boolean     NOT NULL
)
SERVER remote_allhands_prod
OPTIONS (
    schema_name 'public',
    table_name  'Sessions'
);


BEGIN;

-- Optional: faster bulk copy
-- SET LOCAL synchronous_commit = off;

INSERT INTO public."Sessions" (
    "Key",
    "TicketValue",
    "IssuedAt",
    "ExpiresAt",
    "UserId",
    "IsRevoked"
)
SELECT
    "Key",
    "TicketValue",
    "IssuedAt",
    "ExpiresAt",
    "UserId",
    "IsRevoked"
FROM public."Sessions_ft"
ON CONFLICT ("Key")
DO UPDATE SET
    "TicketValue" = EXCLUDED."TicketValue",
    "IssuedAt"    = EXCLUDED."IssuedAt",
    "ExpiresAt"   = EXCLUDED."ExpiresAt",
    "UserId"      = EXCLUDED."UserId",
    "IsRevoked"   = EXCLUDED."IsRevoked";

COMMIT;



















DROP SERVER IF EXISTS remote_allhands_prod CASCADE;
