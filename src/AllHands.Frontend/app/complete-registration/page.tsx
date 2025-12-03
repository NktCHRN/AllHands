"use client";

import Link from "next/link";
import { useSearchParams, useRouter } from "next/navigation";
import { useState } from "react";
import TopBar from "@/components/TopBar";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const ACCOUNT_API = `${API_ROOT}/api/v1/account`;

export default function CompleteRegistration() {
  const searchParams = useSearchParams();
  const router = useRouter();

  const invitationId =
    searchParams.get("invitationId") ?? searchParams.get("InvitationId");
  const token =
    searchParams.get("token") ?? searchParams.get("InvitationToken");

  const [password, setPassword] = useState("");
  const [confirm, setConfirm] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const validate = () => {
    if (password.length < 6) return "Password must be at least 6 characters.";
    if (!/[A-Za-z]/.test(password)) return "Password must contain a letter.";
    if (!/[0-9]/.test(password)) return "Password must contain a number.";
    if (!/[!@#$%^&*()_\-+=<>{}[\]|:;"',.?/~`]/.test(password))
      return "Password must contain a special character.";
    if (password !== confirm) return "Passwords do not match.";
    return "";
  };

  const handleSubmit = async () => {
    setError("");

    if (!invitationId || !token) {
      setError("This registration link is invalid or has expired.");
      return;
    }

    const v = validate();
    if (v) {
      setError(v);
      return;
    }

    setLoading(true);

    try {
      const res = await fetch(`${ACCOUNT_API}/register/invitation`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({
          Password: password,
          InvitationId: invitationId,
          InvitationToken: token,
        }),
      });

      if (!res.ok) {
        let msg = "Registration failed.";
        try {
          const data = await res.json();
          if (typeof data === "string") {
            msg = data;
          } else if (data?.Error?.ErrorMessage) {
            msg = data.Error.ErrorMessage;
          } else if (data?.message) {
            msg = data.message;
          }
        } catch { }
        throw new Error(msg);
      }

      router.push("/account");
    } catch (e: any) {
      setError(e?.message || "Registration error.");
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
            fontSize: "clamp(40px, 4vw, 70px)",
            fontWeight: "bold",
            margin: 0,
          }}
        >
          Registration
        </h2>

        <input
          type="password"
          placeholder="Password"
          className="inputText"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />

        <input
          type="password"
          placeholder="Confirm Password"
          className="inputText"
          value={confirm}
          onChange={(e) => setConfirm(e.target.value)}
        />

        <button
          className="button"
          onClick={handleSubmit}
          disabled={loading}
        >
          {loading ? "Processing..." : "Complete registration"}
        </button>

        {error && (
          <p className="error" style={{ marginTop: "10px" }}>
            {error}
          </p>
        )}
      </div>
    </div>
  );
}
