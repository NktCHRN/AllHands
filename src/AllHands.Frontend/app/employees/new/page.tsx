"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import TopBar from "@/components/TopBar";
import { useCurrentUser } from "@/hooks/currentUser";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const EMPLOYEES_API = `${API_ROOT}/api/v1/employees`;
const POSITIONS_API = `${API_ROOT}/api/v1/positions`;
const ROLES_API = `${API_ROOT}/api/v1/roles`;

const EMPLOYEE_CREATE_PERMISSIONS = ["employee.create", "employee.edit"];

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

type RolesApiInnerDto = {
  id: string;
  name: string;
  permissions?: string[];
};

type RolesApiData = {
  data: RolesApiInnerDto[];
  totalCount: number;
};

type RolesApiResponse = {
  data: RolesApiData | null;
  error: ErrorResponse | null;
  isSuccessful: boolean;
};

type RoleDto = {
  Id: string;
  Name: string;
  Permissions: string[];
};

export default function NewEmployeePage() {
  const router = useRouter();
  const { user, loading: userLoading } = useCurrentUser();

  const [positions, setPositions] = useState<PositionDto[]>([]);
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [firstName, setFirstName] = useState("");
  const [middleName, setMiddleName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");
  const [positionId, setPositionId] = useState("");
  const [roleId, setRoleId] = useState("");
  const [workStartDate, setWorkStartDate] = useState("");
  const [managerId, setManagerId] = useState(
    "00000000-0000-0000-0000-000000000000",
  );
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [canCreateEmployees, setCanCreateEmployees] = useState(false);

  const loadPositions = async () => {
    try {
      const params = new URLSearchParams();
      params.set("Page", "1");
      params.set("PerPage", "100");

      const res = await fetch(`${POSITIONS_API}?${params.toString()}`, {
        method: "GET",
        credentials: "include",
      });

      if (!res.ok) {
        setPositions([]);
        return;
      }

      const json = (await res.json()) as PositionsApiResponse;
      const apiItems = json.data?.data ?? [];

      const mapped: PositionDto[] = apiItems.map((p) => ({
        Id: p.id,
        Name: p.name,
      }));

      setPositions(mapped);
    } catch {
      setPositions([]);
    }
  };

  const loadRoles = async () => {
    try {
      const params = new URLSearchParams();
      params.set("Page", "1");
      params.set("PerPage", "100");

      const res = await fetch(`${ROLES_API}?${params.toString()}`, {
        method: "GET",
        credentials: "include",
      });

      if (!res.ok) {
        setRoles([]);
        return;
      }

      const json = (await res.json()) as RolesApiResponse;
      const apiItems = json.data?.data ?? [];

      const mapped: RoleDto[] = apiItems.map((r) => ({
        Id: r.id,
        Name: r.name,
        Permissions: r.permissions ?? [],
      }));

      setRoles(mapped);
    } catch {
      setRoles([]);
    }
  };

  useEffect(() => {
    void loadPositions();
    void loadRoles();
  }, []);

  useEffect(() => {
    if (!user || !roles.length) {
      setCanCreateEmployees(false);
      return;
    }

    const userRoleNames = (user.roles ?? []).map((n) => n.toLowerCase());
    const allowedPerms = EMPLOYEE_CREATE_PERMISSIONS.map((p) => p.toLowerCase());

    const hasPermission = roles.some((role) => {
      if (!userRoleNames.includes(role.Name.toLowerCase())) {
        return false;
      }
      const rolePerms = role.Permissions.map((p) => p.toLowerCase());
      return allowedPerms.some((p) => rolePerms.includes(p));
    });

    setCanCreateEmployees(hasPermission);
  }, [user, roles]);

  const handleSubmit = async () => {
    if (!canCreateEmployees) {
      setError("You do not have permission to create employees.");
      setSuccess(null);
      return;
    }

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
      setRoleId("");
      setWorkStartDate("");
    } catch {
      setError("Network error. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  const renderNoPermission = () => {
    if (userLoading) {
      return null;
    }

    if (canCreateEmployees) {
      return null;
    }

    return (
      <div
        style={{
          marginBottom: "20px",
          padding: "12px 16px",
          borderRadius: "12px",
          background: "rgba(255, 122, 122, 0.12)",
          color: "#ff7a7a",
          fontSize: "14px",
        }}
      >
        You do not have permission to create employees. Please contact your
        administrator.
      </div>
    );
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
          {renderNoPermission()}
          <div
            style={{
              display: "flex",
              flexDirection: "column",
              gap: "16px",
              opacity: canCreateEmployees ? 1 : 0.6,
              pointerEvents: canCreateEmployees ? "auto" : "none",
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
              <span className="accLable">Role</span>
              <select
                className="accInput"
                value={roleId}
                onChange={(e) => setRoleId(e.target.value)}
              >
                <option value="">Select role</option>
                {roles.map((r) => (
                  <option key={r.Id} value={r.Id}>
                    {r.Name}
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
              disabled={loading || !canCreateEmployees}
              style={{ opacity: loading || !canCreateEmployees ? 0.7 : 1 }}
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
