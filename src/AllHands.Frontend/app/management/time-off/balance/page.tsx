"use client";

import TopBar from "@/components/TopBar";
import { useCurrentUser } from "@/hooks/currentUser";
import { useEffect, useMemo, useState } from "react";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";

const EMPLOYEES_API = `${API_ROOT}/api/v1/employees`;
const TIMEOFF_TYPES_API = `${API_ROOT}/api/v1/time-off/types`;
const BALANCES_API = (employeeId: string) => `${API_ROOT}/api/v1/time-off/employees/${employeeId}/balances`;
const HISTORY_API = (employeeId: string) => `${API_ROOT}/api/v1/time-off/employees/${employeeId}/balances/history`;
const EDIT_BALANCE_API = (employeeId: string, typeId: string) =>
    `${API_ROOT}/api/v1/time-off/employees/${employeeId}/balances/types/${typeId}`;

type TimeOffTypeDto = {
    id: string;
    order?: number | null;
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
    status?: string | null;
};

type BalanceDto = {
    typeId: string;
    balance: number;
    daysPerYear?: number | null;
};

type EditModalState =
    | { open: false }
    | { open: true; employeeId: string; employeeName: string; typeId: string; typeName: string };

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

function fmtName(p?: Partial<EmployeeMiniDto> | null) {
    if (!p) return "—";
    const parts = [p.firstName, p.middleName, p.lastName].filter(Boolean);
    return parts.length ? parts.join(" ") : "—";
}

function safeErrorMessage(raw: string) {
    const s = toStr(raw);
    const looksLikeHtml =
        s.includes("<!DOCTYPE") ||
        s.includes("<html") ||
        s.includes("<head") ||
        s.includes("<body");
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
            "Content-Type": "application/json",
            ...(init?.headers ?? {}),
        },
    });

    const contentType = (res.headers.get("content-type") ?? "").toLowerCase();

    if (!res.ok) {
        const text = await res.text().catch(() => "");
        throw new Error(safeErrorMessage(text || `HTTP ${res.status}`));
    }

    if (res.status === 204) {
        return null as any;
    }

    if (!contentType.includes("application/json")) {
        const text = await res.text().catch(() => "");
        throw new Error(
            safeErrorMessage(text || `Expected JSON but got "${contentType || "unknown"}". Check API route.`)
        );
    }

    return (await res.json()) as T;
}

export default function TimeOffBalancePage() {
    const { user, loading: userLoading } = useCurrentUser();
    const perms = useMemo(() => getPermsLower(user), [user]);
    const canEdit = perms.includes("timeoffbalance.edit") || perms.includes("admin");

    const [busy, setBusy] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const [employees, setEmployees] = useState<EmployeeMiniDto[]>([]);
    const [types, setTypes] = useState<TimeOffTypeDto[]>([]);
    const [balances, setBalances] = useState<BalanceDto[]>([]);

    const [employeeQ, setEmployeeQ] = useState("");
    const [selectedEmployeeId, setSelectedEmployeeId] = useState<string>("");

    const [editModal, setEditModal] = useState<EditModalState>({ open: false });
    const [deltaText, setDeltaText] = useState("");
    const [daysPerYearText, setDaysPerYearText] = useState("");
    const [reasonText, setReasonText] = useState("");

    const selectedEmployee = useMemo(
        () => employees.find((e) => e.id === selectedEmployeeId) ?? null,
        [employees, selectedEmployeeId]
    );

    const typeMap = useMemo(() => {
        const m: Record<string, TimeOffTypeDto> = {};
        for (const t of types) m[t.id] = t;
        return m;
    }, [types]);

    const filteredEmployees = useMemo(() => {
        const q = lower(employeeQ);
        if (!q) return employees;
        return employees.filter((e) => lower(`${e.firstName} ${e.middleName ?? ""} ${e.lastName} ${e.email}`).includes(q));
    }, [employees, employeeQ]);

    const mergedRows = useMemo(() => {
        const rows = balances.map((b) => ({
            ...b,
            type: typeMap[b.typeId],
        }));
        rows.sort((a, b) => (a.type?.order ?? 9999) - (b.type?.order ?? 9999));
        return rows;
    }, [balances, typeMap]);

    async function loadEmployees() {
        const qs = new URLSearchParams();
        qs.set("PerPage", "1000");
        qs.set("Page", "1");

        const json = await apiFetchJson<any>(`${EMPLOYEES_API}?${qs.toString()}`, { method: "GET" });

        const data = (json?.data ?? json) as any;
        const list =
            (data?.employees as EmployeeMiniDto[] | null) ??
            (data?.data as EmployeeMiniDto[] | null) ??
            (data?.items as EmployeeMiniDto[] | null) ??
            [];

        setEmployees(Array.isArray(list) ? list : []);
    }

    async function loadTypes() {
        const json = await apiFetchJson<any>(TIMEOFF_TYPES_API, { method: "GET" });
        const data = (json?.data ?? json) as any;
        const list =
            (data?.types as TimeOffTypeDto[] | null) ??
            (data?.data as TimeOffTypeDto[] | null) ??
            (Array.isArray(data) ? (data as TimeOffTypeDto[]) : []);

        setTypes(Array.isArray(list) ? list : []);
    }

    async function loadBalances(employeeId: string) {
        const json = await apiFetchJson<any>(BALANCES_API(employeeId), { method: "GET" });
        const data = (json?.data ?? json) as any;
        const list =
            (data?.balances as BalanceDto[] | null) ??
            (data?.data as BalanceDto[] | null) ??
            (Array.isArray(data) ? (data as BalanceDto[]) : []);

        setBalances(Array.isArray(list) ? list : []);
    }

    useEffect(() => {
        (async () => {
            setError(null);
            try {
                setBusy(true);
                await Promise.all([loadEmployees(), loadTypes()]);
            } catch (e: any) {
                setError(safeErrorMessage(e?.message ?? "Failed to load data"));
            } finally {
                setBusy(false);
            }
        })();
    }, []);

    useEffect(() => {
        if (!selectedEmployeeId) {
            setBalances([]);
            return;
        }
        (async () => {
            setError(null);
            try {
                setBusy(true);
                await loadBalances(selectedEmployeeId);
            } catch (e: any) {
                setError(safeErrorMessage(e?.message ?? "Failed to load balances"));
            } finally {
                setBusy(false);
            }
        })();
    }, [selectedEmployeeId]);

    function openEdit(typeId: string) {
        if (!canEdit) return;
        if (!selectedEmployee) return;

        const t = typeMap[typeId];
        const title = t?.name ?? "Time-off type";

        setDeltaText("");
        setDaysPerYearText("");
        setReasonText("");

        setEditModal({
            open: true,
            employeeId: selectedEmployee.id,
            employeeName: fmtName(selectedEmployee),
            typeId: typeId,
            typeName: title,
        });
    }

    async function submitEdit() {
        if (!editModal.open) return;
        if (!canEdit) return;

        const deltaRaw = toStr(deltaText);
        const daysRaw = toStr(daysPerYearText);
        const reason = toStr(reasonText);

        const delta = deltaRaw ? Number(deltaRaw) : 0;
        const daysPerYear = daysRaw ? Number(daysRaw) : 0;

        if (!reason) {
            setError("Reason is required.");
            return;
        }

        if (Number.isNaN(delta) || Number.isNaN(daysPerYear)) {
            setError("Delta / days per year must be a number.");
            return;
        }

        setError(null);
        setBusy(true);

        try {
            await apiFetchJson<void>(EDIT_BALANCE_API(editModal.employeeId, editModal.typeId), {
                method: "PUT",
                body: JSON.stringify({
                    delta: delta,
                    daysPerYear: daysRaw ? daysPerYear : 0,
                    reason: reason,
                }),
            });

            setEditModal({ open: false });
            await loadBalances(editModal.employeeId);
        } catch (e: any) {
            setError(safeErrorMessage(e?.message ?? "Failed to edit balance"));
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
                            <h1 className="timeOffHeaderTitle">Time-Off Balance</h1>
                            <p className="timeOffHeaderSubtitle">
                                Select employee and manage time-off balances.
                            </p>
                        </div>

                        <div className="employeesFilters" style={{ marginBottom: 0, justifyContent: "flex-end" }}>
                            <input
                                className="accInput"
                                value={employeeQ}
                                onChange={(e) => setEmployeeQ(e.target.value)}
                                placeholder="Search employee..."
                                disabled={busy || userLoading}
                                style={{ minWidth: 260 }}
                            />

                            <button
                                className="profileButtonSecondary"
                                type="button"
                                onClick={() => (busy ? null : (setSelectedEmployeeId(""), setBalances([])))}
                                disabled={busy || userLoading}
                                style={{ opacity: busy || userLoading ? 0.55 : 1 }}
                            >
                                Clear
                            </button>

                            <button
                                className="profileButtonSecondary"
                                type="button"
                                onClick={() => (busy ? null : (setError(null), loadEmployees()))}
                                disabled={busy || userLoading}
                                style={{ opacity: busy || userLoading ? 0.55 : 1 }}
                            >
                                Refresh
                            </button>
                        </div>
                    </div>

                    {!canEdit ? (
                        <div className="errorMessage">
                            You don’t have permission to edit balances.
                        </div>
                    ) : null}
                    {error ? (
                        <div className="errorMessage" style={{ whiteSpace: "pre-wrap" }}>
                            {error}
                        </div>
                    ) : null}
                    <div style={{ display: "flex", gap: 18, alignItems: "flex-start" }}>
                        <div style={{ width: 420, maxWidth: "100%" }}>
                            <div className="employeesTableWrapper">
                                <table className="employeesTable">
                                    <thead>
                                        <tr>
                                            <th className="tableHead">Employees</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {filteredEmployees.length === 0 ? (
                                            <tr>
                                                <td className="emptyTable">No employees found.</td>
                                            </tr>
                                        ) : (
                                            filteredEmployees.map((e) => {
                                                const active = e.id === selectedEmployeeId;
                                                return (
                                                    <tr
                                                        key={e.id}
                                                        className="tableRow"
                                                        onClick={() => (busy ? null : setSelectedEmployeeId(e.id))}
                                                        style={{
                                                            cursor: busy ? "not-allowed" : "pointer",
                                                            background: active ? "rgba(179, 136, 255, 0.14)" : "transparent",
                                                        }}
                                                    >
                                                        <td className="tableCell">
                                                            <div style={{ fontWeight: 900 }}>{fmtName(e)}</div>
                                                            <div style={{ opacity: 0.75, fontSize: 13 }}>{e.email}</div>
                                                        </td>
                                                    </tr>
                                                );
                                            })
                                        )}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <div style={{ flex: 1, minWidth: 520 }}>
                            {!selectedEmployee ? (
                                <div style={{ opacity: 0.75, fontSize: 16, paddingTop: 8 }}>
                                    Select an employee to see balances.
                                </div>
                            ) : (
                                <>
                                    <div style={{ fontSize: 22, fontWeight: 900, marginBottom: 10 }}>
                                        {fmtName(selectedEmployee)}
                                    </div>

                                    <div className="timeOffTableWrapper">
                                        <table className="timeOffTable">
                                            <thead>
                                                <tr>
                                                    <th className="timeOffTh">Type</th>
                                                    <th className="timeOffTh">Balance</th>
                                                    <th className="timeOffTh">Days / Year</th>
                                                    <th className="timeOffTh">Actions</th>
                                                </tr>
                                            </thead>

                                            <tbody>
                                                {mergedRows.length === 0 ? (
                                                    <tr>
                                                        <td className="timeOffTd" colSpan={4} style={{ opacity: 0.75 }}>
                                                            No balances found.
                                                        </td>
                                                    </tr>
                                                ) : (
                                                    mergedRows.map((r) => {
                                                        const t = r.type;
                                                        return (
                                                            <tr key={r.typeId}>
                                                                <td className="timeOffTd">
                                                                    {t?.emoji ? `${t.emoji} ` : ""}
                                                                    {t?.name ?? r.typeId}
                                                                </td>

                                                                <td className="timeOffTd">{r.balance}</td>

                                                                <td className="timeOffTd">
                                                                    {r.daysPerYear ?? t?.daysPerYear ?? "—"}
                                                                </td>

                                                                <td className="timeOffTd timeOffTd--actions" style={{ flexWrap: "nowrap" }}>
                                                                    <button
                                                                        className="profileButtonSecondary"
                                                                        type="button"
                                                                        disabled={busy || !canEdit}
                                                                        onClick={() => openEdit(r.typeId)}
                                                                        style={{
                                                                            padding: "8px 16px",
                                                                            fontSize: 14,
                                                                            opacity: busy || !canEdit ? 0.5 : 1,
                                                                            cursor: busy || !canEdit ? "not-allowed" : "pointer",
                                                                        }}
                                                                    >
                                                                        Edit
                                                                    </button>

                                                                    <button
                                                                        className="profileButtonSecondary"
                                                                        type="button"
                                                                        disabled={busy}
                                                                        onClick={() => (busy ? null : apiFetchJson<any>(`${HISTORY_API(selectedEmployee.id)}?page=1&perPage=10`, { method: "GET" }).then(() => null).catch(() => null))}
                                                                        style={{
                                                                            padding: "8px 16px",
                                                                            fontSize: 14,
                                                                            opacity: busy ? 0.5 : 1,
                                                                            cursor: busy ? "not-allowed" : "pointer",
                                                                        }}
                                                                    >
                                                                        History
                                                                    </button>
                                                                </td>
                                                            </tr>
                                                        );
                                                    })
                                                )}
                                            </tbody>
                                        </table>
                                    </div>
                                </>
                            )}
                        </div>
                    </div>

                </div>
            </div>

            {editModal.open ? (
                <div
                    onClick={() => (busy ? null : setEditModal({ open: false }))}
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
                        <div style={{ fontSize: 22, fontWeight: 900 }}>
                            Edit balance
                        </div>
                        <div style={{ marginTop: 10, opacity: 0.85, fontSize: 15 }}>
                            <div style={{ fontWeight: 900 }}>{editModal.employeeName}</div>
                            <div style={{ marginTop: 6 }}>
                                Type: <span style={{ fontWeight: 900 }}>{editModal.typeName}</span>
                            </div>
                        </div>
                        <div style={{ marginTop: 14, display: "flex", flexDirection: "column", gap: 12 }}>
                            <div className="accRow">
                                <label className="accLable">Delta</label>
                                <input
                                    className="accInput"
                                    value={deltaText}
                                    onChange={(e) => setDeltaText(e.target.value)}
                                    disabled={busy}
                                    placeholder="+2 / -1"
                                />
                            </div>
                            <div className="accRow">
                                <label className="accLable">Days/year</label>
                                <input
                                    className="accInput"
                                    value={daysPerYearText}
                                    onChange={(e) => setDaysPerYearText(e.target.value)}
                                    disabled={busy}
                                    placeholder="Optional"
                                />
                            </div>
                            <div className="accRow">
                                <label className="accLable">Reason</label>
                                <input
                                    className="accInput"
                                    value={reasonText}
                                    onChange={(e) => setReasonText(e.target.value)}
                                    disabled={busy}
                                    placeholder="Required"
                                />
                            </div>
                        </div>
                        <div style={{ marginTop: 14, display: "flex", justifyContent: "flex-end", gap: 10 }}>
                            <button
                                className="profileButtonSecondary"
                                type="button"
                                onClick={() => setEditModal({ open: false })}
                                disabled={busy}
                                style={{ opacity: busy ? 0.55 : 1 }}
                            >
                                Close
                            </button>
                            <button
                                className="profileButtonPrimary"
                                type="button"
                                onClick={() => submitEdit()}
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
