"use client";

import { useEffect, useMemo, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import TopBar from "@/components/TopBar";
import { useCurrentUser } from "@/hooks/currentUser";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const EMPLOYEES_API = `${API_ROOT}/api/v1/employees`;
const POSITIONS_API = `${API_ROOT}/api/v1/positions`;
const ROLES_API = `${API_ROOT}/api/v1/roles`;

type EmployeeStatus = "Undefined" | "Unactivated" | "Active" | "Fired";
type UiStatus = "Active" | "Fired";

type EmployeeDetails = { id: string; originalStatus: EmployeeStatus };

type PositionOption = { id: string; name: string };
type RoleOption = { id: string; name: string };
type ManagerOption = { id: string; firstName: string; middleName?: string | null; lastName: string };

type ErrorResponse = { errorMessage?: string; ErrorMessage?: string };
type ApiEnvelope = { data?: any; Data?: any; error?: ErrorResponse | null; Error?: ErrorResponse | null };

type FormState = {
  firstName: string;
  middleName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  workStartDate: string;
  managerId: string;
  positionId: string;
  roleId: string;
  status: UiStatus;
  statusReason: string;
  deleteReason: string;
};

const emptyForm: FormState = {
  firstName: "",
  middleName: "",
  lastName: "",
  email: "",
  phoneNumber: "",
  workStartDate: "",
  managerId: "",
  positionId: "",
  roleId: "",
  status: "Active",
  statusReason: "",
  deleteReason: "",
};

function pick<T>(obj: any, ...keys: string[]): T | undefined {
  for (const k of keys) {
    const v = obj?.[k];
    if (v !== undefined && v !== null) return v as T;
  }
  return undefined;
}

function toStr(v: any) {
  return String(v ?? "").trim();
}

function getEmployeeId(dto: any) {
  return toStr(pick(dto, "employeeId", "EmployeeId", "id", "Id"));
}

function getPayload(json: any) {
  return json?.data ?? json?.Data ?? null;
}

function toUiStatus(s: EmployeeStatus): UiStatus {
  return s === "Fired" ? "Fired" : "Active";
}

async function readErrorMessage(res: Response, fallback: string) {
  try {
    const text = await res.text();
    if (!text) return fallback;
    try {
      const json = JSON.parse(text) as { error?: ErrorResponse | null; Error?: ErrorResponse | null };
      return (
        json.error?.errorMessage ||
        json.error?.ErrorMessage ||
        json.Error?.errorMessage ||
        json.Error?.ErrorMessage ||
        text
      );
    } catch {
      return text;
    }
  } catch {
    return fallback;
  }
}

export default function EmployeeById() {
  const params = useParams();
  const router = useRouter();
  const { user, loading: userLoading } = useCurrentUser();
  const [employee, setEmployee] = useState<EmployeeDetails | null>(null);
  const [form, setForm] = useState<FormState>(emptyForm);
  const [positions, setPositions] = useState<PositionOption[]>([]);
  const [roles, setRoles] = useState<RoleOption[]>([]);
  const [managers, setManagers] = useState<ManagerOption[]>([]);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [saveError, setSaveError] = useState<string | null>(null);
  const [saveSuccess, setSaveSuccess] = useState<string | null>(null);
  const [deleteArmedUntil, setDeleteArmedUntil] = useState<number | null>(null);

  const employeeIdFromRoute = useMemo(() => {
    const p = params as any;
    const candidate =
      (typeof p?.id === "string" ? p.id : "") ||
      (typeof p?.employeeId === "string" ? p.employeeId : "") ||
      (typeof window !== "undefined"
        ? (window.location.pathname.split("/").filter(Boolean).slice(-1)[0] ?? "")
        : "");
    return decodeURIComponent(candidate || "").trim();
  }, [params]);

  const rawPerms =
    ((user as any)?.permissions as string[] | null) ??
    ((user as any)?.Permissions as string[] | null) ??
    [];
  const userPerms = Array.isArray(rawPerms) ? rawPerms.map((p) => String(p).toLowerCase()) : [];
  const rawRoles =
    ((user as any)?.roles as string[] | null) ??
    ((user as any)?.Roles as string[] | null) ??
    [];
  const rolesLower = Array.isArray(rawRoles) ? rawRoles.map((r) => String(r).toLowerCase()) : [];
  const canEditEmployee =
    userPerms.includes("employee.edit") ||
    userPerms.includes("employee.create") ||
    rolesLower.includes("admin");
  const canDeleteEmployee =
    userPerms.includes("employee.delete") ||
    rolesLower.includes("admin");
  const disabled = userLoading || !canEditEmployee || loading || saving;
  const formatFullName = (m: ManagerOption) =>
    [m.firstName, m.middleName, m.lastName].filter(Boolean).join(" ");

  const loadEmployee = async (idFromRoute: string) => {
    if (!idFromRoute) {
      setError("Invalid employee id");
      setEmployee(null);
      setForm(emptyForm);
      return;
    }
    try {
      setLoading(true);
      setError(null);
      const res = await fetch(`${EMPLOYEES_API}/${idFromRoute}`, {
        method: "GET",
        credentials: "include",
      });
      if (res.status === 404) {
        router.push("/employees");
        return;
      }
      if (!res.ok) {
        throw new Error(`Failed to load employee (status ${res.status})`);
      }
      const raw = (await res.json()) as ApiEnvelope;
      const dto = getPayload(raw);
      if (!dto) {
        const msg =
          raw.error?.errorMessage ||
          raw.error?.ErrorMessage ||
          raw.Error?.errorMessage ||
          raw.Error?.ErrorMessage ||
          "No employee data returned";
        throw new Error(msg);
      }
      const id = getEmployeeId(dto);
      if (!id) throw new Error("Employee id is missing in API response");
      const originalStatus = (toStr(pick(dto, "status", "Status")) || "Undefined") as EmployeeStatus;
      const managerId = toStr(
        pick(dto, "managerId", "ManagerId") ??
        pick(dto?.manager, "employeeId", "EmployeeId", "id", "Id") ??
        pick(dto?.Manager, "employeeId", "EmployeeId", "id", "Id")
      );
      const positionId = toStr(
        pick(dto, "positionId", "PositionId") ??
        pick(dto?.position, "id", "Id") ??
        pick(dto?.Position, "id", "Id")
      );
      const roleId = toStr(
        pick(dto, "roleId", "RoleId") ??
        pick(dto?.role, "id", "Id") ??
        pick(dto?.Role, "id", "Id")
      );
      const workStartDateRaw = pick<string>(dto, "workStartDate", "WorkStartDate") ?? "";
      const workStartDate = workStartDateRaw ? String(workStartDateRaw).substring(0, 10) : "";
      setEmployee({ id, originalStatus });
      setForm((prev) => ({
        ...prev,
        firstName: toStr(pick(dto, "firstName", "FirstName")),
        middleName: toStr(pick(dto, "middleName", "MiddleName")),
        lastName: toStr(pick(dto, "lastName", "LastName")),
        email: toStr(pick(dto, "email", "Email")),
        phoneNumber: toStr(pick(dto, "phoneNumber", "PhoneNumber")),
        workStartDate,
        managerId,
        positionId,
        roleId,
        status: toUiStatus(originalStatus),
        statusReason: "",
        deleteReason: "",
      }));
    } catch (e: any) {
      setEmployee(null);
      setForm(emptyForm);
      setError(e?.message || "Unexpected error while loading employee");
    } finally {
      setLoading(false);
    }
  };

  const loadPositions = async () => {
    try {
      const res = await fetch(`${POSITIONS_API}?perPage=100&page=1`, {
        method: "GET",
        credentials: "include",
      });
      if (!res.ok) return;
      const raw = (await res.json()) as ApiEnvelope;
      const payload = getPayload(raw);
      const arr = payload?.data ?? payload?.Data ?? [];
      const mapped: PositionOption[] = (arr ?? [])
        .map((p: any) => ({
          id: toStr(pick(p, "id", "Id")),
          name: toStr(pick(p, "name", "Name")),
        }))
        .filter((p: PositionOption) => Boolean(p.id));
      setPositions(mapped);
    } catch { }
  };

  const loadRoles = async () => {
    try {
      const res = await fetch(ROLES_API, {
        method: "GET",
        credentials: "include",
      });
      if (!res.ok) return;
      const raw = (await res.json()) as ApiEnvelope;
      const arr = getPayload(raw) ?? [];
      const mapped: RoleOption[] = (arr ?? [])
        .map((r: any) => ({
          id: toStr(pick(r, "id", "Id")),
          name: toStr(pick(r, "name", "Name")),
        }))
        .filter((r: RoleOption) => Boolean(r.id));
      setRoles(mapped);
    } catch { }
  };

  const loadManagers = async () => {
    try {
      const res = await fetch(`${EMPLOYEES_API}?perPage=1000&page=1&status=Active`, {
        method: "GET",
        credentials: "include",
      });
      if (!res.ok) return;
      const raw = (await res.json()) as ApiEnvelope;
      const payload = getPayload(raw);
      const arr = payload?.data ?? payload?.Data ?? [];
      const mapped: ManagerOption[] = (arr ?? [])
        .map((e: any) => ({
          id: getEmployeeId(e),
          firstName: toStr(pick(e, "firstName", "FirstName")),
          middleName: (pick(e, "middleName", "MiddleName") ?? null) as string | null,
          lastName: toStr(pick(e, "lastName", "LastName")),
        }))
        .filter((m: ManagerOption) => Boolean(m.id));
      setManagers(mapped);
    } catch { }
  };

  useEffect(() => {
    if (!employeeIdFromRoute) {
      setError("Invalid employee id");
      setEmployee(null);
      setForm(emptyForm);
      return;
    }
    void loadEmployee(employeeIdFromRoute);
    void loadPositions();
    void loadRoles();
    void loadManagers();
  }, [employeeIdFromRoute]);

  const handleBack = () => router.push("/employees");

  const handleSave = async () => {
    if (!canEditEmployee || !employee?.id) return;
    try {
      setSaving(true);
      setSaveError(null);
      setSaveSuccess(null);
      const body = {
        employeeId: employee.id,
        firstName: form.firstName,
        middleName: form.middleName || null,
        lastName: form.lastName,
        email: form.email,
        phoneNumber: form.phoneNumber || null,
        workStartDate: form.workStartDate || null,
        managerId: form.managerId || null,
        positionId: form.positionId || null,
        roleId: form.roleId || null,
      };
      const res = await fetch(`${EMPLOYEES_API}/${employee.id}`, {
        method: "PUT",
        credentials: "include",
        headers: {
          "Content-Type": "application/json",
          Accept: "*/*",
        },
        body: JSON.stringify(body),
      });
      if (!res.ok) {
        throw new Error(await readErrorMessage(res, `Failed to save employee (status ${res.status})`));
      }
      const nextStatus: EmployeeStatus = form.status;
      if (employee.originalStatus !== nextStatus && employee.originalStatus !== "Unactivated") {
        if (nextStatus === "Fired") {
          const fireReason = toStr(form.statusReason) || "Fired by admin";
          const fireRes = await fetch(`${EMPLOYEES_API}/${employee.id}/fire`, {
            method: "PUT",
            credentials: "include",
            headers: {
              "Content-Type": "application/json",
              Accept: "*/*",
            },
            body: JSON.stringify({ reason: fireReason, employeeId: employee.id }),
          });
          if (!fireRes.ok) {
            throw new Error(await readErrorMessage(fireRes, `Failed to change status (status ${fireRes.status})`));
          }
        } else if (nextStatus === "Active") {
          const rehRes = await fetch(`${EMPLOYEES_API}/${employee.id}/rehire`, {
            method: "PUT",
            credentials: "include",
            headers: {
              Accept: "*/*",
            },
          });
          if (!rehRes.ok) {
            throw new Error(await readErrorMessage(rehRes, `Failed to change status (status ${rehRes.status})`));
          }
        }
      }
      setSaveSuccess("Changes saved");
      await loadEmployee(employee.id);
    } catch (e: any) {
      setSaveError(e?.message || "Failed to save employee");
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!canDeleteEmployee || !employee?.id) return;
    const now = Date.now();
    const armed = deleteArmedUntil !== null && now < deleteArmedUntil;
    if (!armed) {
      setDeleteArmedUntil(now + 7000);
      return;
    }
    try {
      setSaving(true);
      setSaveError(null);
      setSaveSuccess(null);
      const delReason = toStr(form.deleteReason) || "Deleted by admin";
      const res = await fetch(`${EMPLOYEES_API}/${employee.id}`, {
        method: "DELETE",
        credentials: "include",
        headers: {
          "Content-Type": "application/json",
          Accept: "*/*",
        },
        body: JSON.stringify({ reason: delReason, employeeId: employee.id }),
      });
      if (!res.ok) {
        throw new Error(await readErrorMessage(res, `Failed to delete employee (status ${res.status})`));
      }
      router.push("/employees");
    } catch (e: any) {
      setSaveError(e?.message || "Failed to delete employee");
    } finally {
      setDeleteArmedUntil(null);
      setSaving(false);
    }
  };

  const showDeleteHint = deleteArmedUntil !== null && Date.now() < deleteArmedUntil;

  return (
    <div className="appBackground">
      <TopBar />
      <div className="pageWrapper">
        <div className="pageCard">
          <div style={{ marginBottom: 24, display: "flex", justifyContent: "space-between", alignItems: "center" }}>
            <h1 className="profileTitle">
              {employee ? `${form.firstName} ${form.lastName}` : "Employee info"}
            </h1>
            <button className="profileButtonSecondary" onClick={handleBack}>
              Back to list
            </button>
          </div>
          {error && (
            <div className="errorMessage">
              {error}
            </div>
          )}
          {saveError && (
            <div className="errorMessage">
              {saveError}
            </div>
          )}
          {saveSuccess && (
            <div style={{ marginBottom: 16, color: "#90ee90", fontSize: 16 }}>
              {saveSuccess}
            </div>
          )}
          {loading && (
            <div style={{ opacity: 0.8, marginTop: 10, marginBottom: 10 }}>
              Loading...
            </div>
          )}
          <div style={{ display: "flex", flexDirection: "column", gap: 16 }}>
            <div className="accRow">
              <label className="accLable">
                First name
              </label>
              <input
                className="accInput"
                value={form.firstName}
                onChange={(e) =>
                  setForm((p) => ({ ...p, firstName: e.target.value }))
                }
                disabled={disabled}
              />
            </div>
            <div className="accRow">
              <label className="accLable">
                Middle name
              </label>
              <input
                className="accInput"
                value={form.middleName}
                onChange={(e) =>
                  setForm((p) => ({ ...p, middleName: e.target.value }))
                }
                disabled={disabled}
              />
            </div>
            <div className="accRow">
              <label className="accLable">
                Last name
              </label>
              <input
                className="accInput"
                value={form.lastName}
                onChange={(e) =>
                  setForm((p) => ({ ...p, lastName: e.target.value }))
                }
                disabled={disabled}
              />
            </div>
            <div className="accRow">
              <label className="accLable">
                Email
              </label>
              <input
                className="accInput"
                type="email"
                value={form.email}
                onChange={(e) =>
                  setForm((p) => ({ ...p, email: e.target.value }))
                }
                disabled={disabled}
              />
            </div>
            <div className="accRow">
              <label className="accLable">
                Phone
              </label>
              <input
                className="accInput"
                value={form.phoneNumber}
                onChange={(e) =>
                  setForm((p) => ({ ...p, phoneNumber: e.target.value }))
                }
                disabled={disabled}
              />
            </div>
            <div className="accRow">
              <label className="accLable">
                Start date
              </label>
              <input
                className="accInput"
                type="date"
                value={form.workStartDate}
                onChange={(e) =>
                  setForm((p) => ({ ...p, workStartDate: e.target.value }))
                }
                disabled={disabled}
              />
            </div>
            <div className="accRow">
              <label className="accLable">
                Manager
              </label>
              <select
                className="accInput"
                value={form.managerId}
                onChange={(e) =>
                  setForm((p) => ({ ...p, managerId: e.target.value }))
                }
                disabled={disabled}
              >
                <option value="">
                  Not set
                </option>
                {managers.map((m) => (
                  <option key={m.id} value={m.id}>
                    {formatFullName(m)}
                  </option>
                ))}
              </select>
            </div>
            <div className="accRow">
              <label className="accLable">
                Position
              </label>
              <select
                className="accInput"
                value={form.positionId}
                onChange={(e) =>
                  setForm((p) => ({ ...p, positionId: e.target.value }))
                }
                disabled={disabled}
              >
                <option value="">
                  Not set
                </option>
                {positions.map((p) => (
                  <option key={p.id} value={p.id}>
                    {p.name}
                  </option>
                ))}
              </select>
            </div>
            <div className="accRow">
              <label className="accLable">
                Role
              </label>
              <select
                className="accInput"
                value={form.roleId}
                onChange={(e) =>
                  setForm((p) => ({ ...p, roleId: e.target.value }))
                }
                disabled={disabled}
              >
                <option value="">
                  Not set
                </option>
                {roles.map((r) => (
                  <option key={r.id} value={r.id}>
                    {r.name}
                  </option>
                ))}
              </select>
            </div>
            <div className="accRow">
              <label className="accLable">
                Status
              </label>
              <select
                className="accInput"
                value={form.status}
                onChange={(e) =>
                  setForm((p) => ({
                    ...p,
                    status: e.target.value as UiStatus,
                    statusReason: e.target.value === "Fired" ? p.statusReason : "",
                  }))
                }
                disabled={disabled}
              >
                <option value="Active">
                  Active
                </option>
                <option value="Fired">
                  Fired
                </option>
              </select>
            </div>
            {form.status === "Fired" && (
              <div className="accRow">
                <label className="accLable">
                  Reason
                </label>
                <input
                  className="accInput"
                  value={form.statusReason}
                  onChange={(e) =>
                    setForm((p) => ({ ...p, statusReason: e.target.value }))
                  }
                  disabled={disabled}
                  placeholder="Type reason"
                />
              </div>
            )}
            {canDeleteEmployee && (
              <div className="accRow">
                <label className="accLable">
                  Delete reason
                </label>
                <input
                  className="accInput"
                  value={form.deleteReason}
                  onChange={(e) =>
                    setForm((p) => ({ ...p, deleteReason: e.target.value }))
                  }
                  disabled={saving || loading || !employee?.id}
                  placeholder="Type reason for deletion"
                />
              </div>
            )}
            <div className="profileButtons" style={{ marginTop: 24, display: "flex", gap: 16 }}>
              {canEditEmployee && (
                <button
                  className="profileButtonPrimary"
                  onClick={handleSave}
                  disabled={disabled || !employee?.id || (form.status === "Fired" && !toStr(form.statusReason))}
                >
                  {saving ? "Saving..." : "Save changes"}
                </button>
              )}
              {canDeleteEmployee && (
                <div style={{ display: "flex", flexDirection: "column", alignItems: "flex-start" }}>
                  <button
                    className="profileButtonSecondary"
                    onClick={handleDelete}
                    disabled={saving || loading || !employee?.id}
                    style={{ borderColor: "#ff7a7a", color: "#ff7a7a" }}
                  >
                    {saving ? "Processing..." : showDeleteHint ? "Click again to delete" : "Delete employee"}
                  </button>
                  {showDeleteHint && (
                    <div style={{ marginTop: 10, fontSize: 14, opacity: 0.85 }}>
                      Are you sure? Click the button again.
                    </div>
                  )}
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
