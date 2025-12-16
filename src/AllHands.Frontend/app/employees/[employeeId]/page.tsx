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

type EmployeeDetailsDto = {
  id: string;
  firstName: string;
  middleName?: string | null;
  lastName: string;
  email: string;
  phoneNumber?: string | null;
  workStartDate?: string | null;
  status: EmployeeStatus;
  managerId?: string | null;
  positionId?: string | null;
  roleId?: string | null;
};

type PositionOption = { id: string; name: string };
type RoleOption = { id: string; name: string };
type ManagerOption = { id: string; firstName: string; middleName?: string | null; lastName: string };

type ErrorResponse = { errorMessage?: string; ErrorMessage?: string };

type EmployeeByIdApiResponse = {
  data?: any;
  Data?: any;
  error?: ErrorResponse | null;
  Error?: ErrorResponse | null;
};

type PositionsApiInnerDto = { id?: string; Id?: string; name?: string; Name?: string };
type PositionsApiData = { data?: PositionsApiInnerDto[]; Data?: PositionsApiInnerDto[]; totalCount?: number; TotalCount?: number };
type PositionsApiResponse = { data?: PositionsApiData | null; Data?: PositionsApiData | null };

type RolesApiInnerDto = { id?: string; Id?: string; name?: string; Name?: string };
type RolesApiResponse = { data?: RolesApiInnerDto[] | null; Data?: RolesApiInnerDto[] | null };

type ManagersApiEmployee = { id?: string; Id?: string; firstName?: string; FirstName?: string; middleName?: string | null; MiddleName?: string | null; lastName?: string; LastName?: string };
type ManagersApiData = { data?: ManagersApiEmployee[]; Data?: ManagersApiEmployee[]; totalCount?: number; TotalCount?: number };
type ManagersApiResponse = { data?: ManagersApiData | null; Data?: ManagersApiData | null };

type SaveErrorPayload = { error?: ErrorResponse | null; Error?: ErrorResponse | null };

export default function EmployeeById() {
  const params = useParams();
  const router = useRouter();
  const { user, loading: userLoading } = useCurrentUser();

  const [employee, setEmployee] = useState<EmployeeDetailsDto | null>(null);
  const [positions, setPositions] = useState<PositionOption[]>([]);
  const [roles, setRoles] = useState<RoleOption[]>([]);
  const [managers, setManagers] = useState<ManagerOption[]>([]);

  const [firstName, setFirstName] = useState("");
  const [middleName, setMiddleName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");
  const [workStartDate, setWorkStartDate] = useState("");
  const [managerId, setManagerId] = useState("");
  const [positionId, setPositionId] = useState("");
  const [roleId, setRoleId] = useState("");
  const [status, setStatus] = useState<EmployeeStatus>("Undefined");

  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);

  const [error, setError] = useState<string | null>(null);
  const [saveError, setSaveError] = useState<string | null>(null);
  const [saveSuccess, setSaveSuccess] = useState<string | null>(null);

  const [deleteArmedUntil, setDeleteArmedUntil] = useState<number | null>(null);

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
  const statusDisabled = disabled || status === "Unactivated";

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

  const formatFullName = (m: ManagerOption) => [m.firstName, m.middleName, m.lastName].filter(Boolean).join(" ");

  const readErrorMessage = async (res: Response, fallback: string) => {
    let message = fallback;
    try {
      const text = await res.text();
      if (!text) return message;
      try {
        const json = JSON.parse(text) as SaveErrorPayload;
        message =
          json.error?.errorMessage ||
          json.error?.ErrorMessage ||
          json.Error?.errorMessage ||
          json.Error?.ErrorMessage ||
          text;
      } catch {
        message = text;
      }
    } catch { }
    return message;
  };

  const loadEmployee = async (idFromRoute: string) => {
    if (!idFromRoute) {
      setError("Invalid employee id");
      setEmployee(null);
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
      const raw = (await res.json()) as EmployeeByIdApiResponse;
      const dto = raw.data ?? raw.Data ?? null;
      if (!dto) {
        const msg =
          raw.error?.errorMessage ||
          raw.error?.ErrorMessage ||
          raw.Error?.errorMessage ||
          raw.Error?.ErrorMessage ||
          "No employee data returned";
        throw new Error(msg);
      }
      const id = String(dto.id ?? dto.Id ?? "").trim();
      if (!id) throw new Error("Employee id is missing in API response");
      const normalized: EmployeeDetailsDto = {
        id,
        firstName: String(dto.firstName ?? dto.FirstName ?? ""),
        middleName: (dto.middleName ?? dto.MiddleName ?? null) as string | null,
        lastName: String(dto.lastName ?? dto.LastName ?? ""),
        email: String(dto.email ?? dto.Email ?? ""),
        phoneNumber: (dto.phoneNumber ?? dto.PhoneNumber ?? null) as string | null,
        workStartDate: (dto.workStartDate ?? dto.WorkStartDate ?? null) as string | null,
        status: String(dto.status ?? dto.Status ?? "Undefined") as EmployeeStatus,
        managerId: String(
          dto.managerId ??
          dto.ManagerId ??
          dto.manager?.id ??
          dto.manager?.Id ??
          dto.Manager?.id ??
          dto.Manager?.Id ??
          ""
        ).trim() || null,
        positionId: String(
          dto.positionId ??
          dto.PositionId ??
          dto.position?.id ??
          dto.position?.Id ??
          dto.Position?.id ??
          dto.Position?.Id ??
          ""
        ).trim() || null,
        roleId: String(
          dto.roleId ??
          dto.RoleId ??
          dto.role?.id ??
          dto.role?.Id ??
          dto.Role?.id ??
          dto.Role?.Id ??
          ""
        ).trim() || null,
      };
      setEmployee(normalized);
      setFirstName(normalized.firstName);
      setMiddleName(normalized.middleName ?? "");
      setLastName(normalized.lastName);
      setEmail(normalized.email);
      setPhoneNumber(normalized.phoneNumber ?? "");
      setWorkStartDate(normalized.workStartDate ? String(normalized.workStartDate).substring(0, 10) : "");
      setManagerId(normalized.managerId ?? "");
      setPositionId(normalized.positionId ?? "");
      setRoleId(normalized.roleId ?? "");
      setStatus(normalized.status ?? "Undefined");
    } catch (e: any) {
      setEmployee(null);
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
      const raw = (await res.json()) as PositionsApiResponse;
      const payload = raw.data ?? raw.Data ?? null;
      const arr = payload?.data ?? payload?.Data ?? [];
      const mapped: PositionOption[] = (arr ?? [])
        .map((p) => ({
          id: String(p.id ?? p.Id ?? "").trim(),
          name: String(p.name ?? p.Name ?? "").trim(),
        }))
        .filter((p) => Boolean(p.id));
      setPositions(mapped);
    } catch { }
  };

  const loadRoles = async () => {
    try {
      const res = await fetch(ROLES_API, { method: "GET", credentials: "include" });
      if (!res.ok) return;
      const raw = (await res.json()) as RolesApiResponse;
      const arr = raw.data ?? raw.Data ?? [];
      const mapped: RoleOption[] = (arr ?? [])
        .map((r) => ({
          id: String(r.id ?? r.Id ?? "").trim(),
          name: String(r.name ?? r.Name ?? "").trim(),
        }))
        .filter((r) => Boolean(r.id));
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
      const raw = (await res.json()) as ManagersApiResponse;
      const payload = raw.data ?? raw.Data ?? null;
      const arr = payload?.data ?? payload?.Data ?? [];
      const mapped: ManagerOption[] = (arr ?? [])
        .map((e) => ({
          id: String(e.id ?? e.Id ?? "").trim(),
          firstName: String(e.firstName ?? e.FirstName ?? ""),
          middleName: (e.middleName ?? e.MiddleName ?? null) as string | null,
          lastName: String(e.lastName ?? e.LastName ?? ""),
        }))
        .filter((m) => Boolean(m.id));
      setManagers(mapped);
    } catch { }
  };

  useEffect(() => {
    if (!employeeIdFromRoute) {
      setError("Invalid employee id");
      setEmployee(null);
      return;
    }
    void loadEmployee(employeeIdFromRoute);
    void loadPositions();
    void loadRoles();
    void loadManagers();
  }, [employeeIdFromRoute]);

  const handleBack = () => {
    router.push("/employees");
  };

  const handleSave = async () => {
    if (!canEditEmployee || !employee?.id) return;
    try {
      setSaving(true);
      setSaveError(null);
      setSaveSuccess(null);
      const body = {
        firstName,
        middleName: middleName || null,
        lastName,
        email,
        phoneNumber: phoneNumber || null,
        workStartDate: workStartDate || null,
        managerId: managerId || null,
        positionId: positionId || null,
        roleId: roleId || null,
      };
      const url = `${EMPLOYEES_API}/${employee.id}`;
      const res = await fetch(url, {
        method: "PUT",
        credentials: "include",
        headers: {
          "Content-Type": "application/json",
          Accept: "*/*",
        },
        body: JSON.stringify(body),
      });
      if (!res.ok) {
        const message = await readErrorMessage(res, `Failed to save employee (status ${res.status})`);
        throw new Error(message);
      }
      if (employee.status !== status && employee.status !== "Unactivated") {
        if (status === "Fired") {
          const fireRes = await fetch(`${EMPLOYEES_API}/${employee.id}/fire`, {
            method: "PUT",
            credentials: "include",
            headers: { "Content-Type": "application/json", Accept: "*/*" },
            body: JSON.stringify({ reason: "" }),
          });
          if (!fireRes.ok) throw new Error(`Failed to change status (status ${fireRes.status})`);
        } else if (status === "Active") {
          const rehRes = await fetch(`${EMPLOYEES_API}/${employee.id}/rehire`, {
            method: "PUT",
            credentials: "include",
            headers: { Accept: "*/*" },
          });
          if (!rehRes.ok) throw new Error(`Failed to change status (status ${rehRes.status})`);
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
      const url = `${EMPLOYEES_API}/${employee.id}`;
      const res = await fetch(url, {
        method: "DELETE",
        credentials: "include",
        headers: { Accept: "*/*" },
      });
      // Якщо бекенд ВИМАГАЄ body, заміни блок вище на цей:
      // const res = await fetch(url, {
      //   method: "DELETE",
      //   credentials: "include",
      //   headers: { "Content-Type": "application/json", Accept: "*/*" },
      //   body: JSON.stringify({ reason: "" }),
      // });
      if (!res.ok) {
        const message = await readErrorMessage(res, `Failed to delete employee (status ${res.status})`);
        throw new Error(message);
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
            <h1 className="profileTitle">{employee ? `${employee.firstName} ${employee.lastName}` : "Employee info"}</h1>
            <button className="profileButtonSecondary" onClick={handleBack}>
              Back to list
            </button>
          </div>
          {error && <div className="errorMessage">{error}</div>}
          {saveError && <div className="errorMessage">{saveError}</div>}
          {saveSuccess && <div style={{ marginBottom: 16, color: "#90ee90", fontSize: 16 }}>{saveSuccess}</div>}
          {loading && <div style={{ opacity: 0.8, marginTop: 10, marginBottom: 10 }}>Loading...</div>}
          <div style={{ display: "flex", flexDirection: "column", gap: 16 }}>
            <div className="accRow">
              <label className="accLable">First name</label>
              <input className="accInput" value={firstName} onChange={(e) => setFirstName(e.target.value)} disabled={disabled} />
            </div>
            <div className="accRow">
              <label className="accLable">Middle name</label>
              <input className="accInput" value={middleName} onChange={(e) => setMiddleName(e.target.value)} disabled={disabled} />
            </div>
            <div className="accRow">
              <label className="accLable">Last name</label>
              <input className="accInput" value={lastName} onChange={(e) => setLastName(e.target.value)} disabled={disabled} />
            </div>
            <div className="accRow">
              <label className="accLable">Email</label>
              <input className="accInput" type="email" value={email} onChange={(e) => setEmail(e.target.value)} disabled={disabled} />
            </div>
            <div className="accRow">
              <label className="accLable">Phone</label>
              <input className="accInput" value={phoneNumber} onChange={(e) => setPhoneNumber(e.target.value)} disabled={disabled} />
            </div>
            <div className="accRow">
              <label className="accLable">Start date</label>
              <input className="accInput" type="date" value={workStartDate} onChange={(e) => setWorkStartDate(e.target.value)} disabled={disabled} />
            </div>
            <div className="accRow">
              <label className="accLable">Manager</label>
              <select className="accInput" value={managerId} onChange={(e) => setManagerId(e.target.value)} disabled={disabled}>
                <option value="">Not set</option>
                {managers.map((m) => (
                  <option key={m.id} value={m.id}>
                    {formatFullName(m)}
                  </option>
                ))}
              </select>
            </div>
            <div className="accRow">
              <label className="accLable">Position</label>
              <select className="accInput" value={positionId} onChange={(e) => setPositionId(e.target.value)} disabled={disabled}>
                <option value="">Not set</option>
                {positions.map((p) => (
                  <option key={p.id} value={p.id}>
                    {p.name}
                  </option>
                ))}
              </select>
            </div>
            <div className="accRow">
              <label className="accLable">Role</label>
              <select className="accInput" value={roleId} onChange={(e) => setRoleId(e.target.value)} disabled={disabled}>
                <option value="">Not set</option>
                {roles.map((r) => (
                  <option key={r.id} value={r.id}>
                    {r.name}
                  </option>
                ))}
              </select>
            </div>
            <div className="accRow">
              <label className="accLable">Status</label>
              <select className="accInput" value={status} onChange={(e) => setStatus(e.target.value as EmployeeStatus)} disabled={statusDisabled}>
                <option value="Undefined">Undefined</option>
                <option value="Unactivated">Unactivated</option>
                <option value="Active">Active</option>
                <option value="Fired">Fired</option>
              </select>
            </div>
            <div className="profileButtons" style={{ marginTop: 24, display: "flex", gap: 16 }}>
              {canEditEmployee && (
                <button className="profileButtonPrimary" onClick={handleSave} disabled={disabled || !employee?.id}>
                  {saving ? "Saving..." : "Save changes"}
                </button>
              )}
              {canDeleteEmployee && (
                <div style={{ display: "flex", flexDirection: "column", alignItems: "flex-start" }}>
                  <button className="profileButtonSecondary" onClick={handleDelete} disabled={saving || loading || !employee?.id} style={{ borderColor: "#ff7a7a", color: "#ff7a7a" }}>
                    {saving ? "Processing..." : showDeleteHint ? "Click again to delete" : "Delete employee"}
                  </button>
                  {showDeleteHint && (
                    <div style={{ marginTop: 10, fontSize: 14, opacity: 0.85 }}>
                      Are you sure? Click the button again within a few seconds to confirm deletion.
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
