"use client";

import { useState } from "react";
import TopBar from "@/components/TopBar";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const ACCOUNT_API = `${API_ROOT}/api/v1/account`;

export default function ForgotPassword() {
    const [email, setEmail] = useState("");
    const [info, setInfo] = useState("");
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    const handleReset = async () => {
        setInfo("");
        setError("");

        if (!email) {
            setError("Please enter your email.");
            return;
        }

        try {
            setLoading(true);

            const res = await fetch(`${ACCOUNT_API}/reset-password`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify({ email }),
            });

            if (!res.ok) {
                let message = "Failed to send reset link";

                try {
                    const contentType = res.headers.get("Content-Type") || "";
                    if (contentType.includes("application/json")) {
                        const data = await res.json();
                        if (data?.error?.errorMessage) {
                            message = data.error.errorMessage;
                        }
                    } else {
                        const text = await res.text();
                        console.error("Non-JSON error response:", text);
                    }
                } catch { }

                throw new Error(message);
            }

            setInfo("If this email exists, a reset link has been sent.");
        } catch (e: any) {
            setError(e.message || "Failed to send reset link.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="appBackground">
            <TopBar />

            <div className="loginRegisterContainer">
                <h1 style={{ marginBottom: "5px", fontSize: "38px", fontWeight: "bold" }}>
                    Forgot Password
                </h1>

                <input
                    type="email"
                    placeholder="Enter your email"
                    className="inputText"
                    style={{ width: "360px", fontSize: "20px" }}
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                />

                <button
                    className="button"
                    style={{ marginTop: "10px" }}
                    onClick={handleReset}
                    disabled={loading}
                >
                    {loading ? "Sending..." : "Send Reset Link"}
                </button>

                {info && (
                    <div style={{ color: "#7CFC00", marginTop: "10px", fontWeight: "bold" }}>
                        {info}
                    </div>
                )}

                {error && <p className="error">{error}</p>}
            </div>
        </div>
    );
}
