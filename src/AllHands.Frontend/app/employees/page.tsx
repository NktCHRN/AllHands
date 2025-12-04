"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import TopBar from "@/components/TopBar";

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

type PagedResponse<T> = {
  Items: T[];
  Page: number;
  PerPage: number;
  TotalItems: number;
  TotalPages: number;
};

type ErrorResponse = {
  ErrorMessage?: string;
};

type ApiResponse<T> = {
  Data?: T | null;
  Error?: ErrorResponse | null;
};

export default function EmployeesPage() {
  const router = useRouter();
  const [employees, setEmployees] = useState<EmployeeSearchDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState<"All" | EmployeeStatus>("All");

  const loadEmployees = async () => {
    try {
      setLoading(true);
      setError(null);

      const params = new URLSearchParams();
      params.set("Page", "1");
      params.set("PerPage", String(PER_PAGE_ALL));

      const trimmed = search.trim();
      if (trimmed) params.set("Search", trimmed);
      if (statusFilter !== "All") params.set("Status", statusFilter);

      const res = await fetch(`${EMPLOYEES_API}?${params.toString()}`, {
        method: "GET",
        credentials: "include",
      });

      if (!res.ok) throw new Error("Failed to load employees");

      const json = (await res.json()) as ApiResponse<PagedResponse<EmployeeSearchDto>>;
      if (!json.Data) throw new Error(json.Error?.ErrorMessage || "No data returned");

      setEmployees(json.Data.Items);
    } catch (e: any) {
      setError(e?.message || "Unexpected error while loading employees");
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
              onChange={(e) => setStatusFilter(e.target.value as any)}
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
                    <td colSpan={6} className="emptyTable">
                      No employees found.
                    </td>
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
