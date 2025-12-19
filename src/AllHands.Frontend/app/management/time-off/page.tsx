"use client";

import TopBar from "@/components/TopBar";
import { useCurrentUser } from "@/hooks/currentUser";
import { useEffect, useMemo, useState } from "react";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const LIST_API = `${API_ROOT}/api/v1/time-off/employees/requests`;
const APPROVE_API = (id: string) => `${API_ROOT}/api/v1/time-off/requests/${id}/approve`;
const REJECT_API = (id: string) => `${API_ROOT}/api/v1/time-off/requests/${id}/reject`;

type TimeOffStatus = "Undefined" | "Pending" | "Cancelled" | "Approved" | "Rejected";

type TimeOffTypeDto = {
  id: string;
  order: number;
  name: string;
  emoji?: string | null;
  daysPerYear?: number | null;
};

type EmployeeMiniDto = {
  id: string;
  firstName: string;
  middleName?: string | null;
  lastName: string;
  email: string;
};

type ApproverMiniDto = {
  id: string;
  firstName: string;
  middleName?: string | null;
  lastName: string;
  email: string;
};

type TimeOffRequestDto = {
  id: string;
  startDate: string;
  endDate: string;
  type: TimeOffTypeDto;
  status: TimeOffStatus | string;
  workingDaysCount?: number | null;
  rejectionReason?: string | null;
  employee: EmployeeMiniDto;
  approver?: ApproverMiniDto | null;
};

type PagedResponse<T> = {
  data?: {
    data?: T[];
    total?: number;
    page?: number;
    perPage?: number;
  };
  isSuccessful?: boolean;
  error?: any;
};

type ReasonModalState = { open: false } | { open: true; requestId: string; title: string };

function toStr(v: any) {
  return String(v ?? "").trim();
}

function lower(v: any) {
  return String(v ?? "").toLowerCase().trim();
}

function getPermsLower(user: any): string[] {
  const raw = (user?.permissions as string[] | null) ?? (user?.Permissions as string[] | null) ?? [];
  if (!Array.isArray(raw)) return [];
  return raw.map((x) => lower(x));
}

function fmtName(
  p?:
    | {
        firstName?: string;
        middleName?: string | null;
        lastName?: string;
      }
    | null
) {
  if (!p) return "—";
  const parts = [p.firstName, p.middleName, p.lastName].filter(Boolean);
  return parts.length ? parts.join(" ") : "—";
}

function formatDateRange(start: string, end: string) {
  const s = new Date(start);
  const e = new Date(end);
  const opts: Intl.DateTimeFormatOptions = { year: "numeric", month: "short", day: "numeric" };
  const sText = Number.isNaN(s.getTime()) ? start : s.toLocaleDateString(undefined, opts);
  const eText = Number.isNaN(e.getTime()) ? end : e.toLocaleDateString(undefined, opts);
  if (sText === eText) return sText;
  return `${sText} – ${eText}`;
}

function normalizeStatus(s: any): TimeOffStatus | "Unknown" {
  const v = lower(s);

  if (v === "undefined") return "Undefined";
  if (v === "pending") return "Pending";
  if (v === "cancelled" || v === "canceled") return "Cancelled";
  if (v === "approved") return "Approved";
  if (v === "rejected") return "Rejected";

  return "Unknown";
}

function statusColor(status: TimeOffStatus | "Unknown") {
  if (status === "Approved") return "#7CFC9A";
  if (status === "Rejected") return "#ff6b6b";
  if (status === "Cancelled") return "#cccccc";
  if (status === "Pending") return "#ffd27f";
  if (status === "Undefined") return "#b388ff";
  return "#b388ff";
}

function safeErrorMessage(raw: string) {
  const s = toStr(raw);
  const looksLikeHtml = s.includes("<!DOCTYPE") || s.includes("<html") || s.includes("<head") || s.includes("<body");
  if (looksLikeHtml) {
    const apiHint = API_ROOT
      ? `API base: ${API_ROOT}`
      : "NEXT_PUBLIC_API_BASE_URL is empty (fetch goes to frontend and returns HTML).";
    return `Request returned HTML instead of JSON. Check API URL / proxy / auth. ${apiHint}`;
  }
  if (s.length > 800) return `${s.slice(0, 800)}…`;
  return s || "Request failed";
}

async function apiFetchJson<T>(input: RequestInfo, init?: RequestInit): Promise<T> {
  const res = await fetch(input, {
    ...init,
    credentials: "include",
    headers: {
      Accept: "application/json",
      ...(init?.body ? { "Content-Type": "application/json" } : {}),
      ...(init?.headers ?? {}),
    },
  });

  if (!res.ok) {
    const text = await res.text().catch(() => "");
    throw new Error(safeErrorMessage(text || `HTTP ${res.status}`));
  }

  if (res.status === 204) return null as any;

  const contentType = (res.headers.get("content-type") ?? "").toLowerCase();
  if (!contentType.includes("application/json")) {
    const text = await res.text().catch(() => "");
    throw new Error(safeErrorMessage(text || `Expected JSON but got "${contentType || "unknown"}".`));
  }

  return (await res.json()) as T;
}

export default function TimeOffManagementPage() {
  const { user, loading: userLoading } = useCurrentUser();
  const perms = useMemo(() => getPermsLower(user), [user]);

  const canApprove =
    perms.includes("timeoffpage.edit") || perms.includes("timeoff.approve.detailadminapprove") || perms.includes("admin");
  const canReject =
    perms.includes("timeoffpage.edit") || perms.includes("timeoff.approve.detailadminapprove") || perms.includes("admin");

  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [status, setStatus] = useState<"" | TimeOffStatus>("");
  const [employeeQ, setEmployeeQ] = useState("");

  const [page, setPage] = useState(1);
  const perPage = 10;

  const [rows, setRows] = useState<TimeOffRequestDto[]>([]);
  const [total, setTotal] = useState(0);

  const [reasonModal, setReasonModal] = useState<ReasonModalState>({ open: false });
  const [rejectId, setRejectId] = useState<string | null>(null);
  const [reasonText, setReasonText] = useState("");

  const filteredRows = useMemo(() => {
    const q = lower(employeeQ);
    if (!q) return rows;
    return rows.filter((r) => {
      const full = lower(`${r.employee.firstName} ${r.employee.middleName ?? ""} ${r.employee.lastName} ${r.employee.email}`);
      return full.includes(q);
    });
  }, [rows, employeeQ]);

  const totalPages = useMemo(() => {
    const pp = Math.max(1, perPage);
    const t = Math.max(0, total);
    return Math.max(1, Math.ceil(t / pp));
  }, [total, perPage]);

  async function load() {
    setError(null);

    try {
      const qs = new URLSearchParams();
      qs.set("PerPage", String(perPage));
      qs.set("Page", String(page));
      if (status) qs.set("Status", status);

      const url = `${LIST_API}?${qs.toString()}`;
      const json = await apiFetchJson<PagedResponse<TimeOffRequestDto>>(url, { method: "GET" });

      const data = (json?.data?.data ?? []) as TimeOffRequestDto[];
      const t = Number(json?.data?.total ?? data.length ?? 0);

      setRows(Array.isArray(data) ? data : []);
      setTotal(Number.isFinite(t) ? t : 0);
    } catch (e: any) {
      setRows([]);
      setTotal(0);
      setError(safeErrorMessage(e?.message ?? "Failed to load requests"));
    }
  }

  useEffect(() => {
    load();
  }, [page, status]);

  async function approve(id: string) {
    if (!canApprove) return;

    setBusy(true);
    setError(null);

    try {
      await apiFetchJson<void>(APPROVE_API(id), { method: "POST", body: "{}" });
      await load();
    } catch (e: any) {
      setError(safeErrorMessage(e?.message ?? "Approve failed"));
    } finally {
      setBusy(false);
    }
  }

  function openReject(id: string) {
    if (!canReject) return;
    setReasonText("");
    setRejectId(id);
    setReasonModal({ open: true, requestId: id, title: "Reject request (reason required)" });
  }

  async function submitReject() {
    if (!rejectId) return;

    const reason = toStr(reasonText);
    if (!reason) {
      setError("Reason is required for reject.");
      return;
    }

    setBusy(true);
    setError(null);

    try {
      await apiFetchJson<void>(REJECT_API(rejectId), {
        method: "POST",
        body: JSON.stringify({ id: rejectId, reason }),
      });
      setReasonModal({ open: false });
      setRejectId(null);
      await load();
    } catch (e: any) {
      setError(safeErrorMessage(e?.message ?? "Reject failed"));
    } finally {
      setBusy(false);
    }
  }

  const hasPrev = page > 1;
  const hasNext = page < totalPages;

  return (
    <div className="appBackground">
      <TopBar />

      <div className="timeOffPageWrapper">
        <div className="timeOffCard">
          <div className="timeOffHeader">
            <div>
              <h1 className="timeOffHeaderTitle">Time-Off Management</h1>
              <p className="timeOffHeaderSubtitle">View all employee requests and manage approvals / rejections.</p>
            </div>

            <div className="employeesFilters" style={{ marginBottom: 0, justifyContent: "flex-end" }}>
              <input
                className="accInput"
                value={employeeQ}
                onChange={(e) => setEmployeeQ(e.target.value)}
                placeholder="Search by name or email..."
                disabled={busy}
                style={{ minWidth: 260 }}
              />

              <select
                className="accInput"
                value={status}
                onChange={(e) => {
                  setPage(1);
                  setStatus((e.target.value as any) || "");
                }}
                disabled={busy}
                style={{ width: 220, cursor: busy ? "not-allowed" : "pointer" }}
              >
                <option value="" style={{ background: "#150d2f", color: "#fbeab8" }}>
                  All statuses
                </option>
                <option value="Pending" style={{ background: "#150d2f", color: "#fbeab8" }}>
                  Pending
                </option>
                <option value="Approved" style={{ background: "#150d2f", color: "#fbeab8" }}>
                  Approved
                </option>
                <option value="Rejected" style={{ background: "#150d2f", color: "#fbeab8" }}>
                  Rejected
                </option>
                <option value="Cancelled" style={{ background: "#150d2f", color: "#fbeab8" }}>
                  Cancelled
                </option>
                <option value="Undefined" style={{ background: "#150d2f", color: "#fbeab8" }}>
                  Undefined
                </option>
              </select>

              <button
                className="profileButtonSecondary"
                type="button"
                onClick={() => load()}
                disabled={busy || userLoading}
                style={{
                  padding: "12px 24px",
                  opacity: busy || userLoading ? 0.55 : 1,
                  cursor: busy || userLoading ? "not-allowed" : "pointer",
                }}
              >
                Refresh
              </button>
            </div>
          </div>
          {error ? (
            <div className="errorMessage" style={{ whiteSpace: "pre-wrap" }}>
              {error}
            </div>
          ) : null}
          <div className="timeOffTableWrapper">
            <table className="timeOffTable">
              <thead>
                <tr>
                  <th className="timeOffTh">Type</th>
                  <th className="timeOffTh">Employee</th>
                  <th className="timeOffTh">Dates</th>
                  <th className="timeOffTh">Working days</th>
                  <th className="timeOffTh">Status</th>
                  <th className="timeOffTh">Actions</th>
                </tr>
              </thead>
              <tbody>
                {filteredRows.length === 0 ? (
                  <tr>
                    <td className="timeOffTd" colSpan={6} style={{ opacity: 0.75 }}>
                      No requests found.
                    </td>
                  </tr>
                ) : (
                  filteredRows.map((r) => {
                    const normStatus = normalizeStatus(r.status);
                    const isPending = normStatus === "Pending";

                    const canActApprove = canApprove && isPending;
                    const canActReject = canReject && isPending;

                    return (
                      <tr key={r.id}>
                        <td className="timeOffTd">
                          {r.type?.emoji ? `${r.type.emoji} ` : "🏖️ "}
                          {r.type?.name ?? "—"}
                        </td>

                        <td className="timeOffTd">
                          <div style={{ fontWeight: 700 }}>{fmtName(r.employee)}</div>
                          <div style={{ opacity: 0.75, fontSize: 14 }}>{r.employee.email}</div>
                        </td>

                        <td className="timeOffTd">{formatDateRange(r.startDate, r.endDate)}</td>

                        <td className="timeOffTd">{r.workingDaysCount ?? "—"}</td>

                        <td className="timeOffTd">
                          <span
                            className="timeOffStatusPill"
                            style={{
                              borderColor: statusColor(normStatus),
                              color: statusColor(normStatus),
                            }}
                          >
                            {normStatus === "Unknown" ? String(r.status) : normStatus}
                          </span>

                          <div style={{ marginTop: 8, opacity: 0.85, fontSize: 14 }}>
                            Approved by: <span style={{ fontWeight: 700 }}>{fmtName(r.approver)}</span>
                          </div>

                          {normalizeStatus(r.status) === "Rejected" && r.rejectionReason ? (
                            <div style={{ marginTop: 6, opacity: 0.9, fontSize: 14 }}>
                              Reason: <span style={{ fontWeight: 700 }}>{r.rejectionReason}</span>
                            </div>
                          ) : null}
                        </td>

                        <td className="timeOffTd timeOffTd--actions" style={{ flexWrap: "nowrap" }}>
                          <button
                            className="profileButtonSecondary"
                            type="button"
                            disabled={busy || !canActApprove}
                            onClick={() => approve(r.id)}
                            style={{
                              padding: "8px 16px",
                              fontSize: 14,
                              opacity: busy || !canActApprove ? 0.5 : 1,
                              cursor: busy || !canActApprove ? "not-allowed" : "pointer",
                              borderColor: "rgba(124, 252, 154, 0.8)",
                              color: "rgba(124, 252, 154, 0.95)",
                            }}
                          >
                            Approve
                          </button>
                          <button
                            className="profileButtonSecondary"
                            type="button"
                            disabled={busy || !canActReject}
                            onClick={() => openReject(r.id)}
                            style={{
                              padding: "8px 16px",
                              fontSize: 14,
                              opacity: busy || !canActReject ? 0.5 : 1,
                              cursor: busy || !canActReject ? "not-allowed" : "pointer",
                              borderColor: "rgba(255, 107, 107, 0.8)",
                              color: "rgba(255, 107, 107, 0.95)",
                            }}
                          >
                            Reject
                          </button>
                        </td>
                      </tr>
                    );
                  })
                )}
              </tbody>
            </table>
          </div>
          {totalPages > 1 ? (
            <div className="timeOffPagination">
              <button
                className="profileButtonSecondary"
                type="button"
                onClick={() => setPage((p) => Math.max(1, p - 1))}
                disabled={busy || !hasPrev}
                style={{ opacity: busy || !hasPrev ? 0.5 : 1 }}
              >
                Previous
              </button>
              <span className="timeOffPaginationInfo">
                Page {page} of {totalPages}
              </span>
              <button
                className="profileButtonSecondary"
                type="button"
                onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                disabled={busy || !hasNext}
                style={{ opacity: busy || !hasNext ? 0.5 : 1 }}
              >
                Next
              </button>
            </div>
          ) : null}
          <div style={{ marginTop: 10, opacity: 0.75, fontSize: 14 }}>Total: {total}</div>
        </div>
      </div>

      {reasonModal.open ? (
        <div
          onClick={() => (busy ? null : (setReasonModal({ open: false }), setRejectId(null)))}
          style={{
            position: "fixed",
            inset: 0,
            background: "rgba(0,0,0,0.55)",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            padding: 18,
            zIndex: 50,
          }}
        >
          <div
            onClick={(e) => e.stopPropagation()}
            style={{
              width: "min(680px, 100%)",
              background: "rgba(15, 10, 40, 0.92)",
              border: "1px solid rgba(255,255,255,0.14)",
              borderRadius: 18,
              boxShadow: "0 20px 80px rgba(0,0,0,0.65)",
              padding: 18,
              color: "#fbeab8",
            }}
          >
            <div style={{ fontSize: 22, fontWeight: 900 }}>{reasonModal.title}</div>
            <div style={{ marginTop: 10, opacity: 0.85, fontSize: 15 }}>Please provide a rejection reason (required).</div>
            <textarea
              className="accInput"
              value={reasonText}
              onChange={(e) => setReasonText(e.target.value)}
              disabled={busy}
              placeholder="Reason..."
              style={{ marginTop: 12, width: "100%", minHeight: 120, resize: "vertical" }}
            />

            <div style={{ marginTop: 12, display: "flex", justifyContent: "flex-end", gap: 10 }}>
              <button
                className="profileButtonSecondary"
                type="button"
                onClick={() => (setReasonModal({ open: false }), setRejectId(null))}
                disabled={busy}
                style={{ opacity: busy ? 0.55 : 1 }}
              >
                Close
              </button>
              <button
                className="profileButtonPrimary"
                type="button"
                onClick={() => submitReject()}
                disabled={busy || !toStr(reasonText)}
                style={{ opacity: busy || !toStr(reasonText) ? 0.55 : 1 }}
              >
                Submit
              </button>
            </div>
          </div>
        </div>
      ) : null}
    </div>
  );
}
