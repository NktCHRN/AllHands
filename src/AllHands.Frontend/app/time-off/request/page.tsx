"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState, FormEvent } from "react";
import TopBar from "@/components/TopBar";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const TIME_OFF_TYPES_API = `${API_ROOT}/api/v1/time-off/types`;
const TIME_OFF_REQUESTS_API = `${API_ROOT}/api/v1/time-off/requests`;

type TimeOffTypeDto = {
  Id: string;
  Order: number;
  Name: string;
  Emoji: string;
  DaysPerYear: number;
};

type ErrorResponse = {
  ErrorMessage?: string;
};

type ApiResponse<T> = {
  Data: T | null;
  Error: ErrorResponse | null;
};

export default function RequestTimeOff() {
  const router = useRouter();

  const [navOpen, setNavOpen] = useState(false);
  const [types, setTypes] = useState<TimeOffTypeDto[]>([]);
  const [typesLoading, setTypesLoading] = useState(true);

  const [selectedTypeId, setSelectedTypeId] = useState("");
  const [selectedTypeName, setSelectedTypeName] = useState("");

  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [otherReason, setOtherReason] = useState("");

  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const isOtherType =
    selectedTypeName.toLowerCase() === "other" ||
    selectedTypeName.toLowerCase() === "інше";

  useEffect(() => {
    const loadTypes = async () => {
      try {
        setTypesLoading(true);
        setError(null);

        const res = await fetch(TIME_OFF_TYPES_API, {
          method: "GET",
          credentials: "include",
        });

        if (!res.ok) throw new Error("Failed to load time-off types.");

        const json = (await res.json()) as ApiResponse<TimeOffTypeDto[]>;
        const list = json.Data ?? [];

        list.sort((a, b) => a.Order - b.Order);
        setTypes(list);

        if (list.length > 0) {
          setSelectedTypeId(list[0].Id);
          setSelectedTypeName(list[0].Name);
        }
      } catch (e: any) {
        setError(e?.message || "Unexpected error while loading time-off types.");
      } finally {
        setTypesLoading(false);
      }
    };

    loadTypes();
  }, []);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);

    if (!selectedTypeId) {
      setError("Please select a time-off type.");
      return;
    }

    if (!startDate || !endDate) {
      setError("Please select both start and end dates.");
      return;
    }

    const start = new Date(startDate);
    const end = new Date(endDate);

    if (Number.isNaN(start.getTime()) || Number.isNaN(end.getTime())) {
      setError("Dates are invalid.");
      return;
    }

    if (start > end) {
      setError("Start date must be earlier than or equal to end date.");
      return;
    }

    if (isOtherType && !otherReason.trim()) {
      setError("Please provide a reason for 'Other' type.");
      return;
    }

    try {
      setSubmitting(true);

      const payload: any = {
        TypeId: selectedTypeId,
        StartDate: startDate,
        EndDate: endDate,
      };

      if (isOtherType && otherReason.trim()) {
        payload.Reason = otherReason.trim();
      }

      const res = await fetch(TIME_OFF_REQUESTS_API, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(payload),
      });

      if (!res.ok) {
        let message = "Failed to submit time-off request.";

        try {
          const data = (await res.json()) as ApiResponse<unknown>;
          if (data.Error?.ErrorMessage) message = data.Error.ErrorMessage;
        } catch {}

        throw new Error(message);
      }

      setSuccess("Your time-off request has been submitted.");
    } catch (e: any) {
      setError(e?.message || "Unexpected error while sending request.");
    } finally {
      setSubmitting(false);
    }
  };

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
            maxWidth: "1000px",
            background: "rgba(15, 10, 40, 0.85)",
            borderRadius: "34px",
            padding: "70px 90px",
            color: "#fbeab8",
            boxShadow: "0 40px 90px rgba(0,0,0,0.65)",
          }}
        >
          <h1
            style={{
              fontSize: "52px",
              fontWeight: 900,
              marginBottom: "10px",
              letterSpacing: "0.6px",
            }}
          >
            Request Time Off
          </h1>

          <p style={{ marginBottom: "40px", opacity: 0.85, fontSize: "17px" }}>
            Choose a time-off type and select the dates to submit your request.
          </p>

          {typesLoading ? (
            <p>Loading time-off types...</p>
          ) : (
            <form
              onSubmit={handleSubmit}
              style={{
                display: "flex",
                flexDirection: "column",
                gap: "30px",
              }}
            >
              <div className="accRow">
                <label className="accLable">Type</label>
                <select
                  className="accInput"
                  value={selectedTypeId}
                  onChange={(e) => {
                    const id = e.target.value;
                    setSelectedTypeId(id);
                    const found = types.find((t) => t.Id === id);
                    setSelectedTypeName(found?.Name ?? "");
                  }}
                  style={{
                    appearance: "none",
                    fontSize: "16px",
                    height: "48px",
                  }}
                >
                  {types.map((t) => (
                    <option key={t.Id} value={t.Id}>
                      {t.Emoji ? `${t.Emoji} ${t.Name}` : t.Name}
                    </option>
                  ))}
                </select>
              </div>

              {isOtherType && (
                <div className="accRow">
                  <label className="accLable">Reason</label>
                  <input
                    className="accInput"
                    value={otherReason}
                    onChange={(e) => setOtherReason(e.target.value)}
                    placeholder="Please describe your reason"
                    style={{ height: "48px", fontSize: "16px" }}
                  />
                </div>
              )}

              <div style={{ display: "flex", gap: "30px", flexWrap: "wrap" }}>
                <div className="accRow" style={{ flex: 1, minWidth: "260px" }}>
                  <label className="accLable">Start date</label>
                  <input
                    type="date"
                    className="accInput"
                    value={startDate}
                    onChange={(e) => setStartDate(e.target.value)}
                    style={{ height: "48px", fontSize: "16px" }}
                  />
                </div>

                <div className="accRow" style={{ flex: 1, minWidth: "260px" }}>
                  <label className="accLable">End date</label>
                  <input
                    type="date"
                    className="accInput"
                    value={endDate}
                    onChange={(e) => setEndDate(e.target.value)}
                    style={{ height: "48px", fontSize: "16px" }}
                  />
                </div>
              </div>

              <div style={{ marginTop: "20px" }}>
                <button
                  type="submit"
                  className="profileButtonPrimary"
                  disabled={submitting}
                  style={{
                    opacity: submitting ? 0.7 : 1,
                    fontSize: "16px",
                    padding: "12px 28px",
                  }}
                >
                  {submitting ? "Submitting..." : "Submit request"}
                </button>
              </div>

              {error && (
                <p style={{ marginTop: "10px", color: "tomato" }}>
                  {error}
                </p>
              )}

              {success && (
                <p style={{ marginTop: "10px", color: "#90ee90" }}>
                  {success}
                </p>
              )}
            </form>
          )}
        </div>
      </div>
    </div>
  );
}
