"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import TopBar from "@/components/TopBar";
import { useCurrentUser } from "@/hooks/currentUser";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const TIME_OFF_REQUESTS_API = `${API_ROOT}/api/v1/time-off/requests`;
const TIME_OFF_EMPLOYEE_REQUESTS_API = `${API_ROOT}/api/v1/time-off/employees/requests`;

type TimeOffStatus = "Pending" | "Approved" | "Rejected" | "Cancelled";

type TimeOffRequestDto = {
  id: string;
  typeName: string;
  typeEmoji?: string | null;
  startDate: string;
  endDate: string;
  workingDays: number;
  status: TimeOffStatus;
  approvedByName?: string | null;
  rejectionReason?: string | null;
  createdAt: string;
};

const PER_PAGE = 10;

export default function TimeOffRequestsPage() {
  const router = useRouter();
  const { user } = useCurrentUser();

  const [requests, setRequests] = useState<TimeOffRequestDto[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [cancelingId, setCancelingId] = useState<string | null>(null);

  const normalizeRequestItem = (raw: any): TimeOffRequestDto => {
    const type = raw.type ?? raw.Type ?? {};
    const approvedBy = raw.approvedBy ?? raw.ApprovedBy ?? {};
    const id = raw.id ?? raw.Id ?? "";
    const startDate = raw.startDate ?? raw.StartDate ?? "";
    const endDate = raw.endDate ?? raw.EndDate ?? "";
    const createdAt = raw.createdAt ?? raw.CreatedAt ?? "";
    const status: TimeOffStatus = raw.status ?? raw.Status ?? "Pending";
    const typeName =
      raw.typeName ?? raw.TypeName ?? type.name ?? type.Name ?? "";
    const typeEmoji =
      raw.typeEmoji ?? raw.TypeEmoji ?? type.emoji ?? type.Emoji ?? null;
    const workingDays =
      raw.workingDays ??
      raw.WorkingDays ??
      raw.workingDaysCount ??
      raw.WorkingDaysCount ??
      0;

    const approvedByFirst =
      approvedBy.firstName ?? approvedBy.FirstName ?? "";
    const approvedByMiddle =
      approvedBy.middleName ?? approvedBy.MiddleName ?? "";
    const approvedByLast =
      approvedBy.lastName ?? approvedBy.LastName ?? "";
    const approvedByFromObject = [approvedByFirst, approvedByMiddle, approvedByLast]
      .filter(Boolean)
      .join(" ");

    const approvedByName =
      raw.approvedByName ??
      raw.ApprovedByName ??
      (approvedByFromObject || null);

    const rejectionReason =
      raw.rejectionReason ?? raw.RejectionReason ?? null;

    return {
      id,
      startDate,
      endDate,
      createdAt,
      status,
      typeName,
      typeEmoji,
      workingDays,
      approvedByName,
      rejectionReason,
    };
  };

  const normalizeRequestsPayload = (json: any): {
    items: TimeOffRequestDto[];
    page: number;
    totalPages: number;
  } => {
    const backendError =
      json?.error?.errorMessage ??
      json?.Error?.ErrorMessage ??
      json?.errorMessage ??
      json?.ErrorMessage ??
      null;
    if (backendError) {
      setError(backendError);
    }
    const root = json?.data ?? json?.Data ?? json;
    const payload = root?.data ?? root?.Data ?? root;
    const rawItems: any[] =
      payload?.Items ??
      payload?.items ??
      (Array.isArray(payload) ? payload : []);
    const items = rawItems.map(normalizeRequestItem);
    const currentPage = payload?.Page ?? payload?.page ?? 1;
    const pagesTotal = payload?.TotalPages ?? payload?.totalPages ?? 1;
    return {
      items,
      page: currentPage,
      totalPages: pagesTotal,
    };
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

  const loadRequests = async (pageNumber: number, employeeId?: string) => {
    try {
      setLoading(true);
      setError(null);
      const params = new URLSearchParams();
      params.set("PerPage", String(PER_PAGE));
      params.set("Page", String(pageNumber));
      if (employeeId) {
        params.set("EmployeeId", employeeId);
      }
      const url = `${TIME_OFF_EMPLOYEE_REQUESTS_API}?${params.toString()}`;
      const res = await fetch(url, {
        method: "GET",
        credentials: "include",
      });
      if (!res.ok) {
        throw new Error("Failed to load time-off requests.");
      }
      const json = (await res.json()) as any;
      const { items, page: currentPage, totalPages } =
        normalizeRequestsPayload(json);
      setRequests(items || []);
      setPage(currentPage);
      setTotalPages(totalPages || 1);
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
    if (!user?.employeeId) return;
    void loadRequests(1, user.employeeId);
  }, [user?.employeeId]);

  const reloadCurrentPage = async () => {
    await loadRequests(page, user?.employeeId || undefined);
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

  const hasPrev = page > 1;
  const hasNext = page < totalPages;

  const handlePrevPage = () => {
    if (!hasPrev || loading) return;
    void loadRequests(page - 1, user?.employeeId || undefined);
  };

  const handleNextPage = () => {
    if (!hasNext || loading) return;
    void loadRequests(page + 1, user?.employeeId || undefined);
  };

  const title = "My Time-Off Requests";
  const subtitle =
    "View the history of your time-off requests and manage pending ones.";

  return (
    <div className="appBackground">
      <TopBar />
      <div className="timeOffPageWrapper">
        <div className="timeOffCard">
          <div className="timeOffHeader">
            <div>
              <h1 className="timeOffHeaderTitle">{title}</h1>
              <p className="timeOffHeaderSubtitle">{subtitle}</p>
            </div>
          </div>
          {loading ? (
            <p>Loading requests...</p>
          ) : requests.length === 0 ? (
            <p>No time-off requests yet.</p>
          ) : (
            <div className="timeOffTableWrapper">
              <table className="timeOffTable">
                <thead>
                  <tr>
                    <th className="timeOffTh">Type</th>
                    <th className="timeOffTh">Dates</th>
                    <th className="timeOffTh">Working days</th>
                    <th className="timeOffTh">Status</th>
                    <th className="timeOffTh">Approved by / reason</th>
                    <th className="timeOffTh">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {requests.map((r) => (
                    <tr key={r.id}>
                      <td className="timeOffTd">
                        {r.typeEmoji ? `${r.typeEmoji} ` : ""}
                        {r.typeName}
                      </td>
                      <td className="timeOffTd">
                        {formatDateRange(r.startDate, r.endDate)}
                      </td>
                      <td className="timeOffTd">{r.workingDays}</td>
                      <td className="timeOffTd">
                        <span
                          className="timeOffStatusPill"
                          style={{
                            borderColor: statusColor(r.status),
                            color: statusColor(r.status),
                          }}
                        >
                          {formatStatus(r.status)}
                        </span>
                      </td>
                      <td className="timeOffTd timeOffTd--narrow">
                        {r.status === "Rejected" && r.rejectionReason
                          ? r.rejectionReason
                          : r.approvedByName || "—"}
                      </td>
                      <td className="timeOffTd timeOffTd--actions">
                        {r.status === "Pending" ? (
                          <button
                            className="profileButtonSecondary"
                            style={{
                              fontSize: "12px",
                              padding: "6px 14px",
                              opacity: cancelingId === r.id ? 0.7 : 1,
                            }}
                            disabled={cancelingId === r.id || loading}
                            onClick={() => handleCancel(r.id)}
                          >
                            {cancelingId === r.id ? "Cancelling..." : "Cancel"}
                          </button>
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
            <div className="timeOffPagination">
              <button
                className="profileButtonSecondary"
                onClick={handlePrevPage}
                disabled={!hasPrev || loading}
                style={{ opacity: !hasPrev || loading ? 0.5 : 1 }}
              >
                Previous
              </button>
              <span className="timeOffPaginationInfo">
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
          <div className="timeOffFooter">
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
