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

type EmployeeDetails = {
  id: string;
  firstName: string;
  middleName: string | null;
  lastName: string;
  email: string;
  phoneNumber: string | null;
  workStartDate: string | null;
  status: EmployeeStatus;
  managerId: string | null;
  positionId: string | null;
  roleId: string | null;
};

type Option = {
  id: string;
  name: string;
};

type ManagerOption = {
  id: string;
  firstName: string;
  middleName: string | null;
  lastName: string;
};

export default function EmployeeById() {
  const params = useParams();
  const router = useRouter();
  const { user, loading: userLoading } = useCurrentUser();

  /* -------- data -------- */
  const [employee, setEmployee] = useState<EmployeeDetails | null>(null);

  const [positions, setPositions] = useState<Option[]>([]);
  const [roles, setRoles] = useState<Option[]>([]);
  const [managers, setManagers] = useState<ManagerOption[]>([]);

  /* -------- form -------- */
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

  /* -------- ui state -------- */
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [saveError, setSaveError] = useState<string | null>(null);
  const [saveSuccess, setSaveSuccess] = useState<string | null>(null);
  const [deleteConfirmUntil, setDeleteConfirmUntil] = useState<number | null>(null);

  /* -------- permissions -------- */
  const permissions =
    ((user as any)?.permissions as string[] | null)?.map(p => p.toLowerCase()) ?? [];

  const rolesLower =
    ((user as any)?.roles as string[] | null)?.map(r => r.toLowerCase()) ?? [];

  const canEdit =
    permissions.includes("employee.edit") ||
    permissions.includes("employee.create") ||
    rolesLower.includes("admin");

  const canDelete =
    permissions.includes("employee.delete") ||
    rolesLower.includes("admin");

  const disabled = userLoading || loading || saving || !canEdit;
  const statusDisabled = disabled || status === "Unactivated";

  /* -------- helpers -------- */
  const employeeIdFromRoute = useMemo(() => {
    const p = params as any;
    const raw =
      p?.id ??
      p?.employeeId ??
      (typeof window !== "undefined"
        ? window.location.pathname.split("/").filter(Boolean).pop()
        : "");
    return String(raw ?? "").trim();
  }, [params]);

  const getId = (x: any) =>
    String(x?.employeeId ?? x?.EmployeeId ?? x?.id ?? x?.Id ?? "").trim();

  const formatManagerName = (m: ManagerOption) =>
    [m.firstName, m.middleName, m.lastName].filter(Boolean).join(" ");

  /* -------- load employee -------- */
  const loadEmployee = async (id: string) => {
    try {
      setLoading(true);
      setError(null);

      const res = await fetch(`${EMPLOYEES_API}/${id}`, {
        credentials: "include",
      });

      if (res.status === 404) {
        router.push("/employees");
        return;
      }

      if (!res.ok) throw new Error("Failed to load employee");

      const json = await res.json();
      const dto = json.data ?? json.Data;
      if (!dto) throw new Error("Employee not found");

      const employeeId = getId(dto);
      if (!employeeId) throw new Error("Employee id is missing");

      const normalized: EmployeeDetails = {
        id: employeeId,
        firstName: dto.firstName ?? "",
        middleName: dto.middleName ?? null,
        lastName: dto.lastName ?? "",
        email: dto.email ?? "",
        phoneNumber: dto.phoneNumber ?? null,
        workStartDate: dto.workStartDate ?? null,
        status: dto.status ?? "Undefined",
        managerId: getId(dto.manager) || null,
        positionId: getId(dto.position) || null,
        roleId: getId(dto.role) || null,
      };

      setEmployee(normalized);

      setFirstName(normalized.firstName);
      setMiddleName(normalized.middleName ?? "");
      setLastName(normalized.lastName);
      setEmail(normalized.email);
      setPhoneNumber(normalized.phoneNumber ?? "");
      setWorkStartDate(normalized.workStartDate?.substring(0, 10) ?? "");
      setManagerId(normalized.managerId ?? "");
      setPositionId(normalized.positionId ?? "");
      setRoleId(normalized.roleId ?? "");
      setStatus(normalized.status);
    } catch (e: any) {
      setError(e.message ?? "Error loading employee");
    } finally {
      setLoading(false);
    }
  };

  /* -------- load lookups -------- */
  const loadPositions = async () => {
    const res = await fetch(`${POSITIONS_API}?perPage=100&page=1`, { credentials: "include" });
    if (!res.ok) return;

    const json = await res.json();
    const list = json.data?.data ?? [];

    setPositions(
      list.map((p: any) => ({
        id: getId(p),
        name: p.name ?? "",
      }))
    );
  };

  const loadRoles = async () => {
    const res = await fetch(ROLES_API, { credentials: "include" });
    if (!res.ok) return;

    const json = await res.json();
    const list = json.data ?? [];

    setRoles(
      list.map((r: any) => ({
        id: getId(r),
        name: r.name ?? "",
      }))
    );
  };

  const loadManagers = async () => {
    const res = await fetch(`${EMPLOYEES_API}?status=Active&perPage=1000&page=1`, {
      credentials: "include",
    });
    if (!res.ok) return;

    const json = await res.json();
    const list = json.data?.data ?? [];

    setManagers(
      list.map((e: any) => ({
        id: getId(e),
        firstName: e.firstName ?? "",
        middleName: e.middleName ?? null,
        lastName: e.lastName ?? "",
      }))
    );
  };

  useEffect(() => {
    if (!employeeIdFromRoute) return;

    loadEmployee(employeeIdFromRoute);
    loadPositions();
    loadRoles();
    loadManagers();
  }, [employeeIdFromRoute]);

  /* -------- actions -------- */
  const handleSave = async () => {
    if (!employee) return;

    try {
      setSaving(true);
      setSaveError(null);
      setSaveSuccess(null);

      const body = {
        employeeId: employee.id,
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

      if (!res.ok) throw new Error("Failed to save");

      setSaveSuccess("Changes saved");
      await loadEmployee(employee.id);
    } catch (e: any) {
      setSaveError(e.message ?? "Save failed");
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!employee) return;

    const now = Date.now();
    if (!deleteConfirmUntil || now > deleteConfirmUntil) {
      setDeleteConfirmUntil(now + 5000);
      return;
    }

    await fetch(`${EMPLOYEES_API}/${employee.id}`, {
      method: "DELETE",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ reason: "", employeeId: employee.id }),
    });

    router.push("/employees");
  };

  /* -------- render -------- */
  return (
    <div className="appBackground">
      <TopBar />

      <div className="pageWrapper">
        <div className="pageCard">
          <h1 className="profileTitle">Employee info</h1>

          {error && <div className="errorMessage">{error}</div>}
          {saveError && <div className="errorMessage">{saveError}</div>}
          {saveSuccess && <div style={{ color: "#90ee90" }}>{saveSuccess}</div>}

          <div className="accRow">
            <label>First name</label>
            <input value={firstName} onChange={e => setFirstName(e.target.value)} disabled={disabled} />
          </div>

          <div className="accRow">
            <label>Middle name</label>
            <input value={middleName} onChange={e => setMiddleName(e.target.value)} disabled={disabled} />
          </div>

          <div className="accRow">
            <label>Last name</label>
            <input value={lastName} onChange={e => setLastName(e.target.value)} disabled={disabled} />
          </div>

          <div className="profileButtons">
            {canEdit && (
              <button onClick={handleSave} disabled={disabled}>
                Save changes
              </button>
            )}

            {canDelete && (
              <button onClick={handleDelete}>
                {deleteConfirmUntil ? "Click again to delete" : "Delete employee"}
              </button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
