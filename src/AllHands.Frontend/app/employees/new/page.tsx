"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import TopBar from "@/components/TopBar";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const EMPLOYEES_API = `${API_ROOT}/api/v1/employees`;
const POSITIONS_API = `${API_ROOT}/api/v1/positions`;

type ErrorResponse = {
  errorMessage?: string;
};

type PositionsApiInnerDto = {
  id: string;
  name: string;
};

type PositionsApiData = {
  data: PositionsApiInnerDto[];
  totalCount: number;
};

type PositionsApiResponse = {
  data: PositionsApiData | null;
  error: ErrorResponse | null;
  isSuccessful: boolean;
};

type PositionDto = {
  Id: string;
  Name: string;
};

const MOCK_POSITIONS: PositionDto[] = [
  { Id: "00000000-0000-0000-0000-000000000001", Name: "Software Engineer" },
  { Id: "00000000-0000-0000-0000-000000000002", Name: "HR Manager" },
  { Id: "00000000-0000-0000-0000-000000000003", Name: "Product Manager" },
  { Id: "00000000-0000-0000-0000-000000000004", Name: "QA Engineer" },
];

export default function NewEmployeePage() {
  const router = useRouter();

  const [positions, setPositions] = useState<PositionDto[]>([]);
  const [firstName, setFirstName] = useState("");
  const [middleName, setMiddleName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");
  const [positionId, setPositionId] = useState("");
  const [workStartDate, setWorkStartDate] = useState("");
  const [managerId, setManagerId] = useState(
    "00000000-0000-0000-0000-000000000000",
  );
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const loadPositions = async () => {
    if (!API_ROOT) {
      setPositions(MOCK_POSITIONS);
      return;
    }

    try {
      const params = new URLSearchParams();
      params.set("Page", "1");
      params.set("PerPage", "100");

      const res = await fetch(`${POSITIONS_API}?${params.toString()}`, {
        method: "GET",
        credentials: "include",
      });

      if (!res.ok) {
        setPositions(MOCK_POSITIONS);
        return;
      }

      const json = (await res.json()) as PositionsApiResponse;

      const apiItems = json.data?.data ?? [];

      if (apiItems.length > 0) {
        const mapped: PositionDto[] = apiItems.map((p) => ({
          Id: p.id,
          Name: p.name,
        }));
        setPositions(mapped);
      } else {
        setPositions(MOCK_POSITIONS);
      }
    } catch {
      setPositions(MOCK_POSITIONS);
    }
  };

  useEffect(() => {
    void loadPositions();
  }, []);

  const handleSubmit = async () => {
    if (!firstName || !lastName || !email || !positionId || !workStartDate) {
      setError("Please fill in all required fields.");
      setSuccess(null);
      return;
    }

    try {
      setLoading(true);
      setError(null);
      setSuccess(null);

      const body = {
        PositionId: positionId,
        ManagerId: managerId || "00000000-0000-0000-0000-000000000000",
        Email: email,
        FirstName: firstName,
        MiddleName: middleName || null,
        LastName: lastName,
        PhoneNumber: phoneNumber || null,
        WorkStartDate: workStartDate,
      };

      const res = await fetch(EMPLOYEES_API, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include",
        body: JSON.stringify(body),
      });

      if (!res.ok) {
        let message = "Failed to create employee.";
        try {
          const json = (await res.json()) as {
            error?: ErrorResponse | null;
          };
          if (json.error?.errorMessage) {
            message = json.error.errorMessage;
          }
        } catch { }
        setError(message);
        return;
      }

      setSuccess("Employee created successfully.");
      setFirstName("");
      setMiddleName("");
      setLastName("");
      setEmail("");
      setPhoneNumber("");
      setPositionId("");
      setWorkStartDate("");
    } catch {
      setError("Network error. Please try again.");
    } finally {
      setLoading(false);
    }
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
            maxWidth: "900px",
            background: "rgba(15, 10, 40, 0.9)",
            borderRadius: "34px",
            padding: "50px 60px",
            color: "#fbeab8",
            boxShadow: "0 40px 90px rgba(0,0,0,0.7)",
          }}
        >
          <h1
            style={{
              fontSize: "40px",
              fontWeight: 900,
              marginBottom: "20px",
            }}
          >
            Add employee
          </h1>

          <div
            style={{
              display: "flex",
              flexDirection: "column",
              gap: "16px",
            }}
          >
            <div className="accRow">
              <span className="accLable">First name</span>
              <input
                className="accInput"
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
              />
            </div>

            <div className="accRow">
              <span className="accLable">Middle name</span>
              <input
                className="accInput"
                value={middleName}
                onChange={(e) => setMiddleName(e.target.value)}
              />
            </div>

            <div className="accRow">
              <span className="accLable">Last name</span>
              <input
                className="accInput"
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
              />
            </div>

            <div className="accRow">
              <span className="accLable">Email</span>
              <input
                className="accInput"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
            </div>

            <div className="accRow">
              <span className="accLable">Phone</span>
              <input
                className="accInput"
                value={phoneNumber}
                onChange={(e) => setPhoneNumber(e.target.value)}
              />
            </div>

            <div className="accRow">
              <span className="accLable">Position</span>
              <select
                className="accInput"
                value={positionId}
                onChange={(e) => setPositionId(e.target.value)}
              >
                <option value="">Select position</option>
                {positions.map((p) => (
                  <option key={p.Id} value={p.Id}>
                    {p.Name}
                  </option>
                ))}
              </select>
            </div>

            <div className="accRow">
              <span className="accLable">Start date</span>
              <input
                type="date"
                className="accInput"
                value={workStartDate}
                onChange={(e) => setWorkStartDate(e.target.value)}
              />
            </div>
          </div>

          {error && (
            <div
              style={{
                marginTop: "16px",
                color: "#ff7a7a",
              }}
            >
              {error}
            </div>
          )}

          {success && (
            <div
              style={{
                marginTop: "16px",
                color: "#90ee90",
              }}
            >
              {success}
            </div>
          )}

          <div
            style={{
              marginTop: "26px",
              display: "flex",
              gap: "12px",
            }}
          >
            <button
              className="profileButtonPrimary"
              onClick={handleSubmit}
              disabled={loading}
              style={{ opacity: loading ? 0.7 : 1 }}
            >
              {loading ? "Creating..." : "Create employee"}
            </button>
            <button
              className="profileButtonSecondary"
              onClick={() => router.push("/employees")}
            >
              Cancel
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
