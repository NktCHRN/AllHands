"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import TopBar from "@/components/TopBar";
import { useCurrentUser } from "@/hooks/currentUser";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const EMPLOYEES_API = `${API_ROOT}/api/v1/employees`;
const POSITIONS_API = `${API_ROOT}/api/v1/positions`;
const ROLES_API = `${API_ROOT}/api/v1/roles`;
const EMPLOYEE_EDIT_PERMISSION = "employee.edit";

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

type PositionOption = {
  id: string;
  name: string;
};

type RoleOption = {
  id: string;
  name: string;
};

type ManagerOption = {
  id: string;
  firstName: string;
  middleName?: string | null;
  lastName: string;
};

type ErrorResponse = {
  errorMessage?: string;
  ErrorMessage?: string;
};

type EmployeeByIdApiResponse = {
  data?: EmployeeDetailsDto | null;
  Data?: EmployeeDetailsDto | null;
  error?: ErrorResponse | null;
  Error?: ErrorResponse | null;
};

type PositionsApiInnerDto = {
  id: string;
  name: string;
};

type PositionsApiData = {
  data: PositionsApiInnerDto[];
  totalCount: number;
};

type PositionsApiResponse = {
  data?: PositionsApiData | null;
  Data?: PositionsApiData | null;
};

type RolesApiInnerDto = {
  id: string;
  name: string;
};

type RolesApiResponse = {
  data?: RolesApiInnerDto[] | null;
  Data?: RolesApiInnerDto[] | null;
};

type ManagersApiEmployee = {
  id: string;
  firstName: string;
  middleName?: string | null;
  lastName: string;
};

type ManagersApiData = {
  data: ManagersApiEmployee[];
  totalCount: number;
};

type ManagersApiResponse = {
  data?: ManagersApiData | null;
  Data?: ManagersApiData | null;
};

export default function EmployeeById() {
  const params = useParams();
  const router = useRouter();
  const { user } = useCurrentUser();

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

  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [saveError, setSaveError] = useState<string | null>(null);
  const [saveSuccess, setSaveSuccess] = useState<string | null>(null);

  const rawPerms =
    ((user as any)?.permissions as string[] | undefined) ??
    ((user as any)?.Permissions as string[] | undefined) ??
    [];
  const perms = Array.isArray(rawPerms) ? rawPerms.map((p) => p.toLowerCase()) : [];
  const canEditEmployee = perms.includes(EMPLOYEE_EDIT_PERMISSION.toLowerCase());

  const resolveEmployeeId = () => {
    const p = params as any;
    if (typeof p?.id === "string") return p.id;
    if (typeof p?.employeeId === "string") return p.employeeId;
    if (typeof window !== "undefined") {
      const parts = window.location.pathname.split("/");
      const last = parts[parts.length - 1];
      if (last) return last;
    }
    return "";
  };

  const formatFullName = (m: ManagerOption) =>
    [m.firstName, m.middleName, m.lastName].filter(Boolean).join(" ");

  const loadEmployee = async (employeeId: string) => {
    if (!employeeId) {
      setError("Invalid employee id");
      setEmployee(null);
      return;
    }
    try {
      setLoading(true);
      setError(null);
      const res = await fetch(`${EMPLOYEES_API}/${employeeId}`, {
        method: "GET",
        credentials: "include",
      });
      if (!res.ok) {
        throw new Error("Failed to load employee");
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
      setEmployee(dto);
      setFirstName(dto.firstName ?? "");
      setMiddleName(dto.middleName ?? "");
      setLastName(dto.lastName ?? "");
      setEmail(dto.email ?? "");
      setPhoneNumber(dto.phoneNumber ?? "");
      setWorkStartDate(dto.workStartDate ? dto.workStartDate.substring(0, 10) : "");
      setManagerId(dto.managerId ?? "");
      setPositionId(dto.positionId ?? "");
      setRoleId(dto.roleId ?? "");
    } catch (e: any) {
      setEmployee(null);
      setError(e?.message || "Unexpected error while loading employee");
    } finally {
      setLoading(false);
    }
  };

  const loadPositions = async () => {
    try {
      const url = `${POSITIONS_API}?perPage=100&page=1`;
      const res = await fetch(url, { method: "GET", credentials: "include" });
      if (!res.ok) return;
      const raw = (await res.json()) as PositionsApiResponse;
      const payload = raw.data ?? raw.Data ?? null;
      if (!payload) return;
      const mapped: PositionOption[] = (payload.data ?? []).map((p) => ({
        id: p.id,
        name: p.name,
      }));
      setPositions(mapped);
    } catch {}
  };

  const loadRoles = async () => {
    try {
      const res = await fetch(ROLES_API, {
        method: "GET",
        credentials: "include",
      });
      if (!res.ok) return;
      const raw = (await res.json()) as RolesApiResponse;
      const arr = raw.data ?? raw.Data ?? null;
      if (!arr) return;
      const mapped: RoleOption[] = (arr ?? []).map((r) => ({
        id: r.id,
        name: r.name,
      }));
      setRoles(mapped);
    } catch {}
  };

  const loadManagers = async () => {
    try {
      const url = `${EMPLOYEES_API}?perPage=1000&page=1&status=Active`;
      const res = await fetch(url, { method: "GET", credentials: "include" });
      if (!res.ok) return;
      const raw = (await res.json()) as ManagersApiResponse;
      const payload = raw.data ?? raw.Data ?? null;
      if (!payload) return;
      const mapped: ManagerOption[] = (payload.data ?? []).map((e) => ({
        id: e.id,
        firstName: e.firstName,
        middleName: e.middleName ?? null,
        lastName: e.lastName,
      }));
      setManagers(mapped);
    } catch {}
  };

  useEffect(() => {
    const employeeId = resolveEmployeeId();
    if (!employeeId) {
      setError("Invalid employee id");
      setEmployee(null);
      return;
    }
    void loadEmployee(employeeId);
    void loadPositions();
    void loadRoles();
    void loadManagers();
  }, [params]);

  const handleBack = () => {
    router.push("/employees");
  };

  const handleSave = async () => {
    if (!canEditEmployee || !employee) return;
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
      const res = await fetch(`${EMPLOYEES_API}/${employee.id}`, {
        method: "PUT",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body),
      });
      if (!res.ok) {
        const text = await res.text();
        throw new Error(text || "Failed to save employee");
      }
      setSaveSuccess("Changes saved");
      await loadEmployee(employee.id);
    } catch (e: any) {
      setSaveError(e?.message || "Failed to save employee");
    } finally {
      setSaving(false);
    }
  };

  const disabled = !canEditEmployee || loading || saving;

  return (
    <div className="appBackground">
      <TopBar />
      <div className="pageWrapper">
        <div className="pageCard">
          <div style={{ marginBottom: 24, display: "flex", justifyContent: "space-between", alignItems: "center" }}>
            <h1 className="profileTitle">{employee ? `${employee.firstName} ${employee.lastName}` : "Employee info"}</h1>
            <button className="profileButtonSecondary" onClick={handleBack}>Back to list</button>
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
                  <option key={m.id} value={m.id}>{formatFullName(m)}</option>
                ))}
              </select>
            </div>
            <div className="accRow">
              <label className="accLable">Position</label>
              <select className="accInput" value={positionId} onChange={(e) => setPositionId(e.target.value)} disabled={disabled}>
                <option value="">Not set</option>
                {positions.map((p) => (
                  <option key={p.id} value={p.id}>{p.name}</option>
                ))}
              </select>
            </div>
            <div className="accRow">
              <label className="accLable">Role</label>
              <select className="accInput" value={roleId} onChange={(e) => setRoleId(e.target.value)} disabled={disabled}>
                <option value="">Not set</option>
                {roles.map((r) => (
                  <option key={r.id} value={r.id}>{r.name}</option>
                ))}
              </select>
            </div>
            <div className="accRow">
              <label className="accLable">Status</label>
              <input className="accInput" value={employee?.status ?? ""} disabled />
            </div>
            <div className="profileButtons" style={{ marginTop: 24 }}>
              {canEditEmployee && (
                <button className="profileButtonPrimary" onClick={handleSave} disabled={disabled || !employee}>
                  {saving ? "Saving..." : "Save changes"}
                </button>
              )}
              <button className="profileButtonSecondary" onClick={handleBack}>Cancel</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
