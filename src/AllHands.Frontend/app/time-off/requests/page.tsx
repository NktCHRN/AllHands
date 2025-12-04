"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import TopBar from "@/components/TopBar";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const TIME_OFF_REQUESTS_API = `${API_ROOT}/api/v1/time-off/requests`;
const TIME_OFF_EMPLOYEE_REQUESTS_API = `${API_ROOT}/api/v1/time-off/employees/requests`;
const ACCOUNT_API = `${API_ROOT}/api/v1/account`;

type TimeOffStatus = "Pending" | "Approved" | "Rejected" | "Cancelled";

type TimeOffRequestDto = {
  Id: string;
  TypeName: string;
  TypeEmoji?: string | null;
  StartDate: string;
  EndDate: string;
  WorkingDays: number;
  Status: TimeOffStatus;
  ApprovedByName?: string | null;
  RejectionReason?: string | null;
  CreatedAt: string;
};

const PER_PAGE = 10;
const ADMIN_PERMISSION = "timeoffrequest.adminapprove";

export default function TimeOffRequestsPage() {
  const router = useRouter();

  const [requests, setRequests] = useState<TimeOffRequestDto[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [accountLoading, setAccountLoading] = useState(true);
  const [canAdminApprove, setCanAdminApprove] = useState(false);

  const [cancelingId, setCancelingId] = useState<string | null>(null);
  const [approvingId, setApprovingId] = useState<string | null>(null);
  const [rejectingId, setRejectingId] = useState<string | null>(null);

  const loadAccount = async () => {
    try {
      setAccountLoading(true);

      const res = await fetch(`${ACCOUNT_API}/details`, {
        method: "GET",
        credentials: "include",
      });

      if (!res.ok) {
        throw new Error("Failed to load account details.");
      }

      const json = (await res.json()) as any;

      const data =
        json?.data ??
        json?.Data ??
        json;

      const permissions: string[] =
        data?.permissions ??
        data?.Permissions ??
        []; 

      setCanAdminApprove(permissions.includes(ADMIN_PERMISSION));
    } catch {
      setCanAdminApprove(false);
    } finally {
      setAccountLoading(false);
    }
  };

  const loadRequests = async (pageNumber: number, adminScope: boolean) => {
    try {
      setLoading(true);
      setError(null);

      const baseUrl = adminScope
        ? TIME_OFF_REQUESTS_API
        : TIME_OFF_EMPLOYEE_REQUESTS_API;

      const url = `${baseUrl}?PerPage=${PER_PAGE}&Page=${pageNumber}`;
      const res = await fetch(url, {
        method: "GET",
        credentials: "include",
      });

      if (!res.ok) {
        throw new Error("Failed to load time-off requests.");
      }

      const json = (await res.json()) as any;

      const backendError =
        json?.error?.errorMessage ??
        json?.Error?.ErrorMessage ??
        json?.errorMessage ??
        json?.ErrorMessage ??
        null;

      if (backendError) {
        setError(backendError);
      }

      const payload =
        json?.data ??
        json?.Data ??
        json;

      const items: TimeOffRequestDto[] =
        payload?.Items ??
        payload?.items ??
        (Array.isArray(payload) ? payload : []);

      const currentPage =
        payload?.Page ?? payload?.page ?? pageNumber ?? 1;

      const pagesTotal =
        payload?.TotalPages ?? payload?.totalPages ?? 1;

      setRequests(items || []);
      setPage(currentPage);
      setTotalPages(pagesTotal || 1);
    } catch (e: any) {
      setError(e?.message || "Unexpected error while loading requests.");
      setRequests([]);
      setPage(1);
      setTotalPages(1);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadAccount();
  }, []);

  useEffect(() => {
    if (accountLoading) return;
    void loadRequests(1, canAdminApprove);
  }, [accountLoading, canAdminApprove]);

  const reloadCurrentPage = async () => {
    await loadRequests(page, canAdminApprove);
  };

  const handleCancel = async (id: string) => {
    try {
      setCancelingId(id);
      setError(null);

      const res = await fetch(`${TIME_OFF_REQUESTS_API}/${id}/cancel`, {
        method: "POST",
        credentials: "include",
      });

      if (!res.ok) {
        let message = "Failed to cancel request.";
        try {
          const data = (await res.json()) as any;
          const backendError =
            data?.error?.errorMessage ??
            data?.Error?.ErrorMessage ??
            data?.errorMessage ??
            data?.ErrorMessage;
          if (backendError) {
            message = backendError;
          }
        } catch {}
        throw new Error(message);
      }

      await reloadCurrentPage();
    } catch (e: any) {
      setError(e?.message || "Unexpected error while cancelling request.");
    } finally {
      setCancelingId(null);
    }
  };

  const handleApprove = async (id: string) => {
    try {
      setApprovingId(id);
      setError(null);

      const res = await fetch(`${TIME_OFF_REQUESTS_API}/${id}/approve`, {
        method: "POST",
        credentials: "include",
      });

      if (!res.ok) {
        let message = "Failed to approve request.";
        try {
          const data = (await res.json()) as any;
          const backendError =
            data?.error?.errorMessage ??
            data?.Error?.ErrorMessage ??
            data?.errorMessage ??
            data?.ErrorMessage;
          if (backendError) {
            message = backendError;
          }
        } catch {}
        throw new Error(message);
      }

      await reloadCurrentPage();
    } catch (e: any) {
      setError(e?.message || "Unexpected error while approving request.");
    } finally {
      setApprovingId(null);
    }
  };

  const handleReject = async (id: string) => {
    try {
      setRejectingId(id);
      setError(null);

      const res = await fetch(`${TIME_OFF_REQUESTS_API}/${id}/reject`, {
        method: "POST",
        credentials: "include",
      });

      if (!res.ok) {
        let message = "Failed to reject request.";
        try {
          const data = (await res.json()) as any;
          const backendError =
            data?.error?.errorMessage ??
            data?.Error?.ErrorMessage ??
            data?.errorMessage ??
            data?.ErrorMessage;
          if (backendError) {
            message = backendError;
          }
        } catch {}
        throw new Error(message);
      }

      await reloadCurrentPage();
    } catch (e: any) {
      setError(e?.message || "Unexpected error while rejecting request.");
    } finally {
      setRejectingId(null);
    }
  };

  const formatDateRange = (start: string, end: string) => {
    const s = new Date(start);
    const e = new Date(end);
    const opts: Intl.DateTimeFormatOptions = {
      year: "numeric",
      month: "short",
      day: "numeric",
    };
    const sText = Number.isNaN(s.getTime())
      ? start
      : s.toLocaleDateString(undefined, opts);
    const eText = Number.isNaN(e.getTime())
      ? end
      : e.toLocaleDateString(undefined, opts);
    if (sText === eText) return sText;
    return `${sText} – ${eText}`;
  };

  const formatStatus = (status: TimeOffStatus) => {
    if (status === "Pending") return "Pending";
    if (status === "Approved") return "Approved";
    if (status === "Rejected") return "Rejected";
    if (status === "Cancelled") return "Cancelled";
    return status;
  };

  const statusColor = (status: TimeOffStatus) => {
    if (status === "Approved") return "#7CFC9A";
    if (status === "Rejected") return "#ff6b6b";
    if (status === "Cancelled") return "#cccccc";
    return "#ffd27f";
  };

  const hasPrev = page > 1;
  const hasNext = page < totalPages;

  const handlePrevPage = () => {
    if (!hasPrev || loading) return;
    void loadRequests(page - 1, canAdminApprove);
  };

  const handleNextPage = () => {
    if (!hasNext || loading) return;
    void loadRequests(page + 1, canAdminApprove);
  };

  const title = canAdminApprove ? "All Time-Off Requests" : "My Time-Off Requests";
  const subtitle = canAdminApprove
    ? "View and manage all time-off requests in the company."
    : "View the history of your time-off requests and manage pending ones.";

  return (
    <div className="appBackground">
      <TopBar />
      <div
        style={{
          padding: "50px 30px",
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          minHeight: "calc(100vh - 90px)",
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
                {title}
              </h1>
              <p
                style={{
                  opacity: 0.85,
                  fontSize: "15px",
                }}
              >
                {subtitle}
              </p>
            </div>
          </div>

          {accountLoading || loading ? (
            <p>Loading requests...</p>
          ) : requests.length === 0 ? (
            <p>No time-off requests yet.</p>
          ) : (
            <div
              style={{
                width: "100%",
                overflowX: "auto",
                marginBottom: "24px",
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
                      Type
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
                      Dates
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
                      Working days
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
                      Approved by / reason
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
                  {requests.map((r) => (
                    <tr key={r.Id}>
                      <td
                        style={{
                          padding: "10px 12px",
                          borderBottom: "1px solid rgba(255,255,255,0.04)",
                        }}
                      >
                        {r.TypeEmoji ? `${r.TypeEmoji} ` : ""}
                        {r.TypeName}
                      </td>
                      <td
                        style={{
                          padding: "10px 12px",
                          borderBottom: "1px solid rgba(255,255,255,0.04)",
                        }}
                      >
                        {formatDateRange(r.StartDate, r.EndDate)}
                      </td>
                      <td
                        style={{
                          padding: "10px 12px",
                          borderBottom: "1px solid rgba(255,255,255,0.04)",
                        }}
                      >
                        {r.WorkingDays}
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
                            border: `1px solid ${statusColor(r.Status)}`,
                            color: statusColor(r.Status),
                            fontSize: "12px",
                            fontWeight: 600,
                          }}
                        >
                          {formatStatus(r.Status)}
                        </span>
                      </td>
                      <td
                        style={{
                          padding: "10px 12px",
                          borderBottom: "1px solid rgba(255,255,255,0.04)",
                          maxWidth: "260px",
                        }}
                      >
                        {r.Status === "Rejected" && r.RejectionReason
                          ? r.RejectionReason
                          : r.ApprovedByName || "—"}
                      </td>
                      <td
                        style={{
                          padding: "10px 12px",
                          borderBottom: "1px solid rgba(255,255,255,0.04)",
                          display: "flex",
                          gap: "8px",
                          alignItems: "center",
                        }}
                      >
                        {r.Status === "Pending" ? (
                          canAdminApprove ? (
                            <>
                              <button
                                className="profileButtonPrimary"
                                style={{
                                  fontSize: "12px",
                                  padding: "6px 14px",
                                  opacity:
                                    approvingId === r.Id || loading ? 0.7 : 1,
                                }}
                                disabled={
                                  approvingId === r.Id ||
                                  rejectingId === r.Id ||
                                  loading
                                }
                                onClick={() => handleApprove(r.Id)}
                              >
                                {approvingId === r.Id
                                  ? "Approving..."
                                  : "Approve"}
                              </button>
                              <button
                                className="profileButtonSecondary"
                                style={{
                                  fontSize: "12px",
                                  padding: "6px 14px",
                                  opacity:
                                    rejectingId === r.Id || loading ? 0.7 : 1,
                                }}
                                disabled={
                                  rejectingId === r.Id ||
                                  approvingId === r.Id ||
                                  loading
                                }
                                onClick={() => handleReject(r.Id)}
                              >
                                {rejectingId === r.Id
                                  ? "Rejecting..."
                                  : "Reject"}
                              </button>
                            </>
                          ) : (
                            <button
                              className="profileButtonSecondary"
                              style={{
                                fontSize: "12px",
                                padding: "6px 14px",
                                opacity: cancelingId === r.Id ? 0.7 : 1,
                              }}
                              disabled={cancelingId === r.Id || loading}
                              onClick={() => handleCancel(r.Id)}
                            >
                              {cancelingId === r.Id ? "Cancelling..." : "Cancel"}
                            </button>
                          )
                        ) : (
                          <span style={{ opacity: 0.6 }}>—</span>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}

          {error && (
            <p
              style={{
                marginTop: "8px",
                color: "tomato",
              }}
            >
              {error}
            </p>
          )}

          {totalPages > 1 && (
            <div
              style={{
                marginTop: "20px",
                display: "flex",
                justifyContent: "flex-end",
                gap: "12px",
                alignItems: "center",
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
              <span
                style={{
                  fontSize: "13px",
                  opacity: 0.8,
                }}
              >
                Page {page} of {totalPages}
              </span>
              <button
                className="profileButtonSecondary"
                onClick={handleNextPage}
                disabled={!hasNext || loading}
                style={{ opacity: !hasNext || loading ? 0.5 : 1 }}
              >
                Next
              </button>
            </div>
          )}

          <div
            style={{
              marginTop: "26px",
              display: "flex",
              justifyContent: "flex-start",
            }}
          >
            <button
              className="profileButtonPrimary"
              onClick={() => router.push("/time-off/request")}
            >
              Create new request
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
