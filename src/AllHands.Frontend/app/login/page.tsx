"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import Link from "next/link";
import TopBar from "@/components/TopBar";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const ACCOUNT_API = `${API_ROOT}/api/v1/account`;

export default function LoginPage() {
  const router = useRouter();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async () => {
    if (!email || !password) {
      setError("Please enter email and password");
      return;
    }

    try {
      setError("");
      setLoading(true);

      const res = await fetch(`${ACCOUNT_API}/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ email, password }),
      });

      if (!res.ok) {
        let message = "Invalid login or password";
        try {
          const data = await res.json();
          if (data?.error?.errorMessage) message = data.error.errorMessage;
        } catch {}
        setError(message);
        setLoading(false);
        return;
      }

      setLoading(false);
      router.push("/account");
    } catch {
      setError("Network error. Please try again.");
      setLoading(false);
    }
  };

  return (
    <div className="appBackground">
      <TopBar />
      <div className="loginRegisterContainer">
        <h1 style={{ marginBottom: "5px", fontSize: "42px", fontWeight: "bold" }}>
          Log in
        </h1>

        <div style={{ display: "flex", flexDirection: "column", gap: "10px" }}>
          <input
            type="email"
            placeholder="login"
            className="inputText"
            style={{ width: "360px", fontSize: "20px" }}
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />

          <input
            type="password"
            placeholder="password"
            className="inputText"
            style={{ width: "360px", fontSize: "20px" }}
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>

        <button
          className="button"
          style={{ marginTop: "5px" }}
          onClick={handleSubmit}
          disabled={loading}
        >
          {loading ? "Logging..." : "Login"}
        </button>

        <Link
          href="/forgot-password"
          style={{
            marginTop: "10px",
            fontSize: "16px",
            color: "#cfaaff",
            textDecoration: "underline",
            cursor: "pointer",
          }}
        >
          Forgot password?
        </Link>

        {error && <div className="error">{error}</div>}
      </div>
    </div>
  );
}
