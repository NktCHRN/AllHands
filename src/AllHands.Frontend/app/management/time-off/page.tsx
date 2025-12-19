"use client";

import TopBar from "@/components/TopBar";
import { useCurrentUser } from "@/hooks/currentUser";
import { useMemo, useState } from "react";

type TimeOffStatus = "Pending" | "Cancelled" | "Approved" | "Rejected";

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
  status: TimeOffStatus;
  workingDaysCount?: number | null;
  rejectionReason?: string | null;
  employee: EmployeeMiniDto;
  approver?: ApproverMiniDto | null;
};

type ReasonModalState = { open: false } | { open: true; requestId: string; title: string };

function toStr(v: any) {
  return String(v ?? "").trim();
}

function lower(v: any) {
  return String(v ?? "").toLowerCase().trim();
}

function pickStr(obj: any, ...keys: string[]) {
  for (const k of keys) {
    const v = obj?.[k];
    if (v !== undefined && v !== null && String(v).trim() !== "") return String(v);
  }
  return "";
}

function pickNullableStr(obj: any, ...keys: string[]) {
  const v = pickStr(obj, ...keys);
  return v ? v : null;
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

function statusColor(status: TimeOffStatus) {
  if (status === "Approved") return "#7CFC9A";
  if (status === "Rejected") return "#ff6b6b";
  if (status === "Cancelled") return "#cccccc";
  if (status === "Pending") return "#ffd27f";
  return "#b388ff";
}

function iso(d: Date) {
  return d.toISOString().slice(0, 10);
}

export default function TimeOffManagementLocalPage() {
  const { user, loading: userLoading } = useCurrentUser();
  const perms = useMemo(() => getPermsLower(user), [user]);

  const canApprove =
    perms.includes("timeoffpage.edit") ||
    perms.includes("timeoff.approve.detailadminapprove") ||
    perms.includes("admin");
  const canReject =
    perms.includes("timeoffpage.edit") ||
    perms.includes("timeoff.approve.detailadminapprove") ||
    perms.includes("admin");

  const me: ApproverMiniDto = useMemo(() => {
    const u: any = user ?? {};
    return {
      id: pickStr(u, "id", "Id") || "local-approver",
      firstName: pickStr(u, "firstName", "FirstName") || "Approver",
      middleName: pickNullableStr(u, "middleName", "MiddleName"),
      lastName: pickStr(u, "lastName", "LastName"),
      email: pickStr(u, "email", "Email") || "local@demo",
    };
  }, [user]);

  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [status, setStatus] = useState<"" | TimeOffStatus>("");
  const [employeeQ, setEmployeeQ] = useState("");

  const perPage = 10;
  const [page, setPage] = useState(1);

  const [rows, setRows] = useState<TimeOffRequestDto[]>(() => {
    const baseEmployee: EmployeeMiniDto = {
      id: "e1",
      firstName: "Anastasiia",
      middleName: "Vadymivna",
      lastName: "Linchuk",
      email: "stacy.linchuk@gmail.com",
    };

    const vacation: TimeOffTypeDto = { id: "t1", order: 1, name: "Vacation", emoji: "🌴", daysPerYear: 24 };
    const sick: TimeOffTypeDto = { id: "t2", order: 2, name: "Sick leave (documented)", emoji: "🧾", daysPerYear: 0 };

    return [
      {
        id: "r1",
        startDate: iso(new Date(2025, 11, 20)),
        endDate: iso(new Date(2026, 0, 1)),
        type: vacation,
        status: "Pending",
        workingDaysCount: 7,
        employee: baseEmployee,
        approver: null,
      },
      {
        id: "r2",
        startDate: iso(new Date(2025, 11, 12)),
        endDate: iso(new Date(2025, 11, 17)),
        type: sick,
        status: "Approved",
        workingDaysCount: 4,
        employee: { ...baseEmployee, id: "e2", firstName: "Linchuka", lastName: "Leonidivna Inna", email: "linchuk.anastasia.lavi@gmail.com" },
        approver: me,
      },
    ];
  });

  const [reasonModal, setReasonModal] = useState<ReasonModalState>({ open: false });
  const [rejectId, setRejectId] = useState<string | null>(null);
  const [reasonText, setReasonText] = useState("");

  const filtered = useMemo(() => {
    const q = lower(employeeQ);
    const byName = !q
      ? rows
      : rows.filter((r) => {
          const full = lower(`${r.employee.firstName} ${r.employee.middleName ?? ""} ${r.employee.lastName} ${r.employee.email}`);
          return full.includes(q);
        });

    const byStatus = status ? byName.filter((r) => r.status === status) : byName;
    return byStatus;
  }, [rows, employeeQ, status]);

  const total = filtered.length;
  const totalPages = Math.max(1, Math.ceil(total / Math.max(1, perPage)));
  const safePage = Math.min(Math.max(1, page), totalPages);

  const pageRows = useMemo(() => {
    const start = (safePage - 1) * perPage;
    return filtered.slice(start, start + perPage);
  }, [filtered, safePage, perPage]);

  const hasPrev = safePage > 1;
  const hasNext = safePage < totalPages;

  async function approve(id: string) {
    if (!canApprove) return;
    setBusy(true);
    setError(null);

    try {
      setRows((prev) =>
        prev.map((r) =>
          r.id !== id || r.status !== "Pending"
            ? r
            : {
                ...r,
                status: "Approved",
                approver: me,
                rejectionReason: null,
              }
        )
      );
    } catch (e: any) {
      setError(toStr(e?.message ?? "Approve failed"));
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
      setRows((prev) =>
        prev.map((r) =>
          r.id !== rejectId || r.status !== "Pending"
            ? r
            : {
                ...r,
                status: "Rejected",
                approver: me,
                rejectionReason: reason,
              }
        )
      );
      setReasonModal({ open: false });
      setRejectId(null);
    } catch (e: any) {
      setError(toStr(e?.message ?? "Reject failed"));
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="appBackground">
      <TopBar />
      <div className="timeOffPageWrapper">
        <div className="timeOffCard">
          <div className="timeOffHeader">
            <div>
              <h1 className="timeOffHeaderTitle">Time-Off Management (Local UI)</h1>
              <p className="timeOffHeaderSubtitle">Mocked requests — approve / reject without backend.</p>
            </div>

            <div className="employeesFilters" style={{ marginBottom: 0, justifyContent: "flex-end" }}>
              <input
                className="accInput"
                value={employeeQ}
                onChange={(e) => (setPage(1), setEmployeeQ(e.target.value))}
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
              </select>

              <button
                className="profileButtonSecondary"
                type="button"
                onClick={() => null}
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
                {pageRows.length === 0 ? (
                  <tr>
                    <td className="timeOffTd" colSpan={6} style={{ opacity: 0.75 }}>
                      No requests found.
                    </td>
                  </tr>
                ) : (
                  pageRows.map((r) => {
                    const isPending = r.status === "Pending";
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
                              borderColor: statusColor(r.status),
                              color: statusColor(r.status),
                            }}
                          >
                            {r.status}
                          </span>

                          <div style={{ marginTop: 8, opacity: 0.85, fontSize: 14 }}>
                            Approved by: <span style={{ fontWeight: 700 }}>{fmtName(r.approver)}</span>
                          </div>

                          {r.status === "Rejected" && r.rejectionReason ? (
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
                Page {safePage} of {totalPages}
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
