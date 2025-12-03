"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import TopBar from "@/components/TopBar";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const EMPLOYEES_API = `${API_ROOT}/api/v1/employees`;

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
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState<"All" | EmployeeStatus>("All");
  const [navOpen, setNavOpen] = useState(false);

  const loadEmployees = async (pageToLoad: number) => {
    try {
      setLoading(true);
      setError(null);

      const params = new URLSearchParams();
      params.set("Page", String(pageToLoad));
      params.set("PerPage", "10");

      const trimmedSearch = search.trim();
      if (trimmedSearch) {
        params.set("Search", trimmedSearch);
      }
      if (statusFilter !== "All") {
        params.set("Status", statusFilter);
      }

      const url = `${EMPLOYEES_API}?${params.toString()}`;

      const res = await fetch(url, {
        method: "GET",
        credentials: "include",
      });

      if (!res.ok) {
        throw new Error("Failed to load employees.");
      }

      const json =
        (await res.json()) as ApiResponse<PagedResponse<EmployeeSearchDto>>;

      const data = json.Data;
      if (!data) {
        const msg = json.Error?.ErrorMessage || "No data returned.";
        throw new Error(msg);
      }

      setEmployees(data.Items);
      setPage(data.Page);
      setTotalPages(data.TotalPages || 1);
    } catch (e: any) {
      setError(e?.message || "Unexpected error while loading employees.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadEmployees(1);
  }, []);

  const handleLogout = () => {
    setNavOpen(false);
    router.push("/");
  };

  const hasPrev = page > 1;
  const hasNext = page < totalPages;

  const handlePrevPage = () => {
    if (!hasPrev || loading) return;
    void loadEmployees(page - 1);
  };

  const handleNextPage = () => {
    if (!hasNext || loading) return;
    void loadEmployees(page + 1);
  };

  const handleApplyFilters = () => {
    void loadEmployees(1);
  };

  const handleAddEmployee = () => {
    router.push("/employees/new");
  };

  const statusColor = (status: EmployeeStatus) => {
    if (status === "Active") return "#7CFC9A";
    if (status === "Unactivated") return "#ffd27f";
    if (status === "Fired") return "#ff6b6b";
    return "#cccccc";
  };

  const formatStatus = (status: EmployeeStatus) => {
    if (status === "Undefined") return "Undefined";
    if (status === "Unactivated") return "Unactivated";
    if (status === "Active") return "Active";
    if (status === "Fired") return "Fired";
    return status;
  };

  const formatName = (e: EmployeeSearchDto) => {
    const parts = [e.FirstName, e.MiddleName, e.LastName].filter(Boolean) as string[];
    return parts.join(" ");
  };

  return (
    <div className="appBackground">
      <TopBar />
      <div
        style={{
          width: "100%",
          minHeight: "calc(100vh - 100px)",
          display: "flex",
          justifyContent: "center",
          alignItems: "flex-start",
          padding: "50px 20px 70px",
          boxSizing: "border-box",
        }}
      >
        <div
          style={{
            width: "100%",
            maxWidth: "1100px",
            background: "rgba(15, 10, 40, 0.9)",
            borderRadius: "34px",
            padding: "60px 70px",
            color: "#fbeab8",
            boxShadow: "0 40px 90px rgba(0,0,0,0.7)",
          }}
        >
          <div
            style={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              gap: "20px",
              marginBottom: "26px",
            }}
          >
            <div>
              <h1
                style={{
                  fontSize: "46px",
                  fontWeight: 900,
                  marginBottom: "6px",
                  letterSpacing: "0.4px",
                }}
              >
                Employees
              </h1>
              <p
                style={{
                  margin: 0,
                  opacity: 0.8,
                  fontSize: "16px",
                }}
              >
                View the list of employees, search and filter them.
              </p>
            </div>

            <button
              className="profileButtonPrimary"
              onClick={handleAddEmployee}
              style={{ whiteSpace: "nowrap" }}
            >
              Add employee
            </button>
          </div>

          <div
            style={{
              display: "flex",
              gap: "12px",
              alignItems: "center",
              marginBottom: "24px",
              flexWrap: "wrap",
            }}
          >
            <input
              type="text"
              placeholder="Search by name or email..."
              className="accInput"
              style={{ maxWidth: "320px" }}
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />

            <select
              className="accInput"
              style={{ maxWidth: "220px" }}
              value={statusFilter}
              onChange={(e) =>
                setStatusFilter(e.target.value as "All" | EmployeeStatus)
              }
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
              style={{ opacity: loading ? 0.7 : 1 }}
            >
              {loading ? "Loading..." : "Apply filters"}
            </button>
          </div>

          {error && (
            <div
              style={{
                marginBottom: "16px",
                color: "#ff7a7a",
                fontSize: "16px",
              }}
            >
              {error}
            </div>
          )}

          <div
            style={{
              overflowX: "auto",
              borderRadius: "18px",
              border: "1px solid rgba(255,255,255,0.08)",
            }}
          >
            <table
              style={{
                width: "100%",
                borderCollapse: "collapse",
                fontSize: "14px",
              }}
            >
              <thead>
                <tr>
                  <th
                    style={{
                      textAlign: "left",
                      padding: "10px 12px",
                      borderBottom: "1px solid rgba(255,255,255,0.08)",
                      fontWeight: 600,
                      fontSize: "13px",
                      textTransform: "uppercase",
                      letterSpacing: "0.08em",
                    }}
                  >
                    Name
                  </th>
                  <th
                    style={{
                      textAlign: "left",
                      padding: "10px 12px",
                      borderBottom: "1px solid rgba(255,255,255,0.08)",
                      fontWeight: 600,
                      fontSize: "13px",
                      textTransform: "uppercase",
                      letterSpacing: "0.08em",
                    }}
                  >
                    Email
                  </th>
                  <th
                    style={{
                      textAlign: "left",
                      padding: "10px 12px",
                      borderBottom: "1px solid rgba(255,255,255,0.08)",
                      fontWeight: 600,
                      fontSize: "13px",
                      textTransform: "uppercase",
                      letterSpacing: "0.08em",
                    }}
                  >
                    Phone
                  </th>
                  <th
                    style={{
                      textAlign: "left",
                      padding: "10px 12px",
                      borderBottom: "1px solid rgba(255,255,255,0.08)",
                      fontWeight: 600,
                      fontSize: "13px",
                      textTransform: "uppercase",
                      letterSpacing: "0.08em",
                    }}
                  >
                    Position
                  </th>
                  <th
                    style={{
                      textAlign: "left",
                      padding: "10px 12px",
                      borderBottom: "1px solid rgba(255,255,255,0.08)",
                      fontWeight: 600,
                      fontSize: "13px",
                      textTransform: "uppercase",
                      letterSpacing: "0.08em",
                    }}
                  >
                    Status
                  </th>
                  <th
                    style={{
                      textAlign: "left",
                      padding: "10px 12px",
                      borderBottom: "1px solid rgba(255,255,255,0.08)",
                      fontWeight: 600,
                      fontSize: "13px",
                      textTransform: "uppercase",
                      letterSpacing: "0.08em",
                    }}
                  >
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody>
                {employees.length === 0 && !loading && (
                  <tr>
                    <td
                      colSpan={6}
                      style={{
                        padding: "16px 12px",
                        textAlign: "center",
                        opacity: 0.7,
                      }}
                    >
                      No employees found.
                    </td>
                  </tr>
                )}
                {employees.map((e) => (
                  <tr key={e.Id}>
                    <td
                      style={{
                        padding: "10px 12px",
                        borderBottom: "1px solid rgba(255,255,255,0.04)",
                      }}
                    >
                      {formatName(e)}
                    </td>
                    <td
                      style={{
                        padding: "10px 12px",
                        borderBottom: "1px solid rgba(255,255,255,0.04)",
                      }}
                    >
                      {e.Email}
                    </td>
                    <td
                      style={{
                        padding: "10px 12px",
                        borderBottom: "1px solid rgba(255,255,255,0.04)",
                      }}
                    >
                      {e.PhoneNumber || "—"}
                    </td>
                    <td
                      style={{
                        padding: "10px 12px",
                        borderBottom: "1px solid rgba(255,255,255,0.04)",
                      }}
                    >
                      {e.Position?.Name || "—"}
                    </td>
                    <td
                      style={{
                        padding: "10px 12px",
                        borderBottom: "1px solid rgba(255,255,255,0.04)",
                      }}
                    >
                      <span
                        style={{
                          padding: "4px 10px",
                          borderRadius: "999px",
                          backgroundColor: "rgba(0,0,0,0.35)",
                          border: `1px solid ${statusColor(e.Status)}`,
                          color: statusColor(e.Status),
                          fontSize: "12px",
                          fontWeight: 600,
                        }}
                      >
                        {formatStatus(e.Status)}
                      </span>
                    </td>
                    <td
                      style={{
                        padding: "10px 12px",
                        borderBottom: "1px solid rgba(255,255,255,0.04)",
                      }}
                    >
                      <button
                        className="profileButtonSecondary"
                        style={{
                          padding: "6px 16px",
                          fontSize: "12px",
                        }}
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

          <div
            style={{
              marginTop: "24px",
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              gap: "12px",
              flexWrap: "wrap",
            }}
          >
            <span
              style={{
                fontSize: "13px",
                opacity: 0.8,
              }}
            >
              Page {page} of {totalPages}
            </span>

            <div
              style={{
                display: "flex",
                gap: "10px",
              }}
            >
              <button
                className="profileButtonSecondary"
                onClick={handlePrevPage}
                disabled={!hasPrev || loading}
                style={{ opacity: !hasPrev || loading ? 0.5 : 1 }}
              >
                Previous
              </button>
              <button
                className="profileButtonSecondary"
                onClick={handleNextPage}
                disabled={!hasNext || loading}
                style={{ opacity: !hasNext || loading ? 0.5 : 1 }}
              >
                Next
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
