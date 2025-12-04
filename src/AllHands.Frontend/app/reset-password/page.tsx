"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import TopBar from "@/components/TopBar";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const ACCOUNT_API = `${API_ROOT}/api/v1/account`;

type Props = {
  searchParams: {
    token?: string;
    email?: string;
  };
};

export default function ResetPassword({ searchParams }: Props) {
  const router = useRouter();

  const token = searchParams.token ?? "";
  const email = searchParams.email ?? "";

  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async () => {
    setError("");
    setSuccess("");

    if (!token || !email) {
      setError("Invalid or missing reset link.");
      return;
    }

    if (!newPassword || !confirmPassword) {
      setError("Please fill in both fields.");
      return;
    }

    if (newPassword !== confirmPassword) {
      setError("New passwords do not match.");
      return;
    }

    if (newPassword.length < 6) {
      setError("New password must be at least 6 characters.");
      return;
    }

    setLoading(true);

    try {
      const res = await fetch(`${ACCOUNT_API}/password`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          email,
          token,
          newPassword,
        }),
      });

      if (!res.ok) {
        let message = "Failed to change password.";

        try {
          const ct = res.headers.get("Content-Type") || "";
          if (ct.includes("application/json")) {
            const data = await res.json();
            if (data?.error?.errorMessage) {
              message = data.error.errorMessage;
            } else if (data?.message) {
              message = data.message;
            }
          } else {
            await res.text();
          }
        } catch { }

        throw new Error(message);
      }

      setSuccess("Password changed successfully!");

      setTimeout(() => {
        router.push("/login");
      }, 1500);
    } catch (e: any) {
      setError(e?.message || "Failed to change password.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div
      style={{
        backgroundColor: "#0D081E",
        minHeight: "100vh",
      }}
    >
      <TopBar />
      <div className="loginRegisterContainer">
        <h2
          style={{
            fontSize: "clamp(35px, 4vw, 60px)",
            fontWeight: "bold",
            margin: 0,
          }}
        >
          Reset Password
        </h2>
        <input
          type="password"
          placeholder="New password"
          className="inputText"
          value={newPassword}
          onChange={(e) => setNewPassword(e.target.value)}
        />
        <input
          type="password"
          placeholder="Confirm new password"
          className="inputText"
          value={confirmPassword}
          onChange={(e) => setConfirmPassword(e.target.value)}
        />
        <button
          className="button"
          type="button"
          disabled={loading}
          onClick={handleSubmit}
        >
          {loading ? "Saving..." : "Save New Password"}
        </button>
        {error && (
          <p className="error" style={{ marginTop: "10px" }}>
            {error}
          </p>
        )}
        {success && (
          <p
            style={{
              color: "#7CFC00",
              marginTop: "10px",
              fontWeight: "bold",
            }}
          >
            {success}
          </p>
        )}
      </div>
    </div>
  );
}
