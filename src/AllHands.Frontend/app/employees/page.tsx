"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import TopBar from "@/components/TopBar";
import { useCurrentUser } from "@/hooks/currentUser";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const EMPLOYEES_API = `${API_ROOT}/api/v1/employees`;
const PER_PAGE_ALL = 1000;

type EmployeeStatus = "Undefined" | "Unactivated" | "Active" | "Fired";

type PositionDto = {
  Id: string;
  Name: string;
};

type EmployeeSearchDto = {
  Id: string;
  FirstName: string;
  MiddleName?: string | null;
  LastName: string;
  Email: string;
  PhoneNumber?: string | null;
  Status: EmployeeStatus;
  Position?: PositionDto | null;
};

type ErrorResponse = {
  errorMessage?: string;
  ErrorMessage?: string;
};

type EmployeesApiEmployee = {
  id: string;
  firstName: string;
  middleName?: string | null;
  lastName: string;
  email: string;
  phoneNumber?: string | null;
  status: EmployeeStatus;
  position?: {
    id: string;
    name: string;
  } | null;
};

type EmployeesApiData = {
  data: EmployeesApiEmployee[];
  totalCount: number;
};

type EmployeesApiResponse = {
  data?: EmployeesApiData | null;
  Data?: EmployeesApiData | null;
  error?: ErrorResponse | null;
  Error?: ErrorResponse | null;
};

export default function EmployeesPage() {
  const router = useRouter();
  const { user } = useCurrentUser();

  const [employees, setEmployees] = useState<EmployeeSearchDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState<"All" | EmployeeStatus>("All");

  const rawPerms =
    ((user as any)?.permissions as string[] | undefined) ??
    ((user as any)?.Permissions as string[] | undefined) ??
    [];
  const perms = Array.isArray(rawPerms) ? rawPerms.map((p) => p.toLowerCase()) : [];

  const loadEmployees = async () => {
    try {
      setLoading(true);
      setError(null);

      const params = new URLSearchParams();
      params.set("Page", "1");
      params.set("PerPage", String(PER_PAGE_ALL));
      if (statusFilter !== "All") params.set("Status", statusFilter);

      const res = await fetch(`${EMPLOYEES_API}?${params.toString()}`, {
        method: "GET",
        credentials: "include",
      });

      if (!res.ok) throw new Error("Failed to load employees");

      const raw = (await res.json()) as EmployeesApiResponse;
      const payload = raw.data ?? raw.Data ?? null;

      if (!payload) {
        const msg =
          raw.error?.errorMessage ||
          raw.error?.ErrorMessage ||
          raw.Error?.errorMessage ||
          raw.Error?.ErrorMessage ||
          "No data returned";
        throw new Error(msg);
      }

      const mapped: EmployeeSearchDto[] = (payload.data ?? []).map((e) => ({
        Id: e.id,
        FirstName: e.firstName,
        MiddleName: e.middleName ?? null,
        LastName: e.lastName,
        Email: e.email,
        PhoneNumber: e.phoneNumber ?? null,
        Status: e.status,
        Position: e.position
          ? { Id: e.position.id, Name: e.position.name }
          : null,
      }));

      let result = mapped;

      if (statusFilter !== "All") {
        result = result.filter((e) => e.Status === statusFilter);
      }

      const trimmedLower = search.trim().toLowerCase();
      if (trimmedLower) {
        result = result.filter((e) => {
          const name = [e.FirstName, e.MiddleName, e.LastName]
            .filter(Boolean)
            .join(" ")
            .toLowerCase();
          const email = e.Email.toLowerCase();
          return name.includes(trimmedLower) || email.includes(trimmedLower);
        });
      }

      setEmployees(result);
    } catch (e: any) {
      setError(e?.message || "Unexpected error while loading employees");
      setEmployees([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadEmployees();
  }, []);

  const handleAddEmployee = () => router.push("/employees/new");
  const handleApplyFilters = () => void loadEmployees();

  const statusColor = (s: EmployeeStatus) =>
    s === "Active"
      ? "#7CFC9A"
      : s === "Unactivated"
      ? "#ffd27f"
      : s === "Fired"
      ? "#ff6b6b"
      : "#cccccc";

  const formatStatus = (s: EmployeeStatus) => s;

  const formatName = (e: EmployeeSearchDto) =>
    [e.FirstName, e.MiddleName, e.LastName].filter(Boolean).join(" ");

  return (
    <div className="appBackground">
      <TopBar />

      <div className="pageWrapper">
        <div className="pageCard">
          <div className="employeesHeader">
            <div>
              <h1 className="employeesTitle">Employees</h1>
              <p className="employeesSubtitle">View all employees, search and filter them.</p>
            </div>

            <button className="profileButtonPrimary" onClick={handleAddEmployee}>
              Add employee
            </button>
          </div>

          <div className="employeesFilters">
            <input
              type="text"
              placeholder="Search by name or email..."
              className="accInput"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />

            <select
              className="accInput"
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value as "All" | EmployeeStatus)}
            >
              <option value="All">All statuses</option>
              <option value="Active">Active</option>
              <option value="Unactivated">Unactivated</option>
              <option value="Fired">Fired</option>
              <option value="Undefined">Undefined</option>
            </select>

            <button
              className="profileButtonSecondary"
              onClick={handleApplyFilters}
              disabled={loading}
            >
              {loading ? "Loading..." : "Apply filters"}
            </button>

            <span className="employeesTotal">Total: {employees.length}</span>
          </div>

          {error && <div className="errorMessage">{error}</div>}

          <div className="employeesTableWrapper">
            <table className="employeesTable">
              <thead>
                <tr>
                  <th className="tableHead">Name</th>
                  <th className="tableHead">Email</th>
                  <th className="tableHead">Phone</th>
                  <th className="tableHead">Position</th>
                  <th className="tableHead">Status</th>
                  <th className="tableHead">Actions</th>
                </tr>
              </thead>
              <tbody>
                {employees.length === 0 && !loading && (
                  <tr>
                    <td colSpan={6} className="emptyTable">No employees found.</td>
                  </tr>
                )}

                {employees.map((e) => (
                  <tr key={e.Id} className="tableRow">
                    <td className="tableCell">{formatName(e)}</td>
                    <td className="tableCell">{e.Email}</td>
                    <td className="tableCell">{e.PhoneNumber || "—"}</td>
                    <td className="tableCell">{e.Position?.Name || "—"}</td>
                    <td className="tableCell">
                      <span
                        className="statusBadge"
                        style={{
                          borderColor: statusColor(e.Status),
                          color: statusColor(e.Status),
                        }}
                      >
                        {formatStatus(e.Status)}
                      </span>
                    </td>
                    <td className="tableCell">
                      <button
                        className="profileButtonSecondary smallButton"
                        onClick={() => router.push(`/employees/${e.Id}`)}
                      >
                        View
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  );
}
