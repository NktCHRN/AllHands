"use client";

import Image from "next/image";
import TopBar from "@/components/TopBar";
import { useCurrentUser } from "@/hooks/currentUser";
import { useEffect, useMemo, useRef, useState } from "react";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const COMPANY_API = `${API_ROOT}/api/v1/company`;
const COMPANY_LOGO_API = `${API_ROOT}/api/v1/company/logo`;

type CompanyDto = {
  id: string;
  name: string;
  description?: string | null;
  emailDomain?: string | null;
  isSameDomainValidationEnforced?: boolean;
  ianaTimeZone?: string | null;
  workDays?: string[] | null;
  createdAt?: string | null;
  updatedAt?: string | null;
  deletedAt?: string | null;
};

function toStr(v: any) {
  return String(v ?? "").trim();
}

function fmtIso(iso?: string | null) {
  if (!iso) return "—";
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return iso;
  return d.toLocaleString();
}

export default function HomePage() {
  const { user, loading: userLoading } = useCurrentUser();
  const isAuthed = !!user;

  const [company, setCompany] = useState<CompanyDto | null>(null);
  const [logoUrl, setLogoUrl] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [err, setErr] = useState("");

  const lastLogoUrlRef = useRef<string | null>(null);

  const token = useMemo(() => {
    const t =
      (user as any)?.token ??
      (user as any)?.accessToken ??
      (typeof window !== "undefined" ? localStorage.getItem("token") : null);
    return toStr(t);
  }, [user]);

  useEffect(() => {
    if (!isAuthed) {
      setCompany(null);
      setErr("");
      setLoading(false);
      setLogoUrl((prev) => {
        if (prev) URL.revokeObjectURL(prev);
        return null;
      });
      lastLogoUrlRef.current = null;
      return;
    }

    if (userLoading) return;

    let cancelled = false;

    async function loadCompany() {
      setLoading(true);
      setErr("");

      try {
        const headers: Record<string, string> = {};
        if (token) headers["Authorization"] = `Bearer ${token}`;

        const [companyRes, logoRes] = await Promise.all([
          fetch(COMPANY_API, {
            method: "GET",
            headers,
            cache: "no-store",
            credentials: "include",
          }),
          fetch(COMPANY_LOGO_API, {
            method: "GET",
            headers,
            cache: "no-store",
            credentials: "include",
          }).catch(() => null),
        ]);

        if (!companyRes.ok) {
          const text = await companyRes.text().catch(() => "");
          throw new Error(`Company request failed: ${companyRes.status} ${text}`);
        }

        const companyJson = await companyRes.json();
        const dto: CompanyDto | null = companyJson?.data ?? null;

        let nextLogoUrl: string | null = null;
        if (logoRes && logoRes.ok) {
          const blob = await logoRes.blob();
          nextLogoUrl = URL.createObjectURL(blob);
        }

        if (cancelled) {
          if (nextLogoUrl) URL.revokeObjectURL(nextLogoUrl);
          return;
        }

        setCompany(dto);

        if (nextLogoUrl) {
          setLogoUrl((prev) => {
            if (prev) URL.revokeObjectURL(prev);
            return nextLogoUrl;
          });
          lastLogoUrlRef.current = nextLogoUrl;
        } else {
          setLogoUrl((prev) => prev);
        }
      } catch (e: any) {
        if (!cancelled) setErr(e?.message ? String(e.message) : "Failed to load company");
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    loadCompany();

    return () => {
      cancelled = true;
    };
  }, [isAuthed, userLoading, token]);

  useEffect(() => {
    return () => {
      if (lastLogoUrlRef.current) URL.revokeObjectURL(lastLogoUrlRef.current);
    };
  }, []);

  if (!isAuthed) {
    return (
      <div className="appBackground">
        <TopBar />
        <div
          style={{
            minHeight: "calc(100vh - 100px)",
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            textAlign: "center",
            padding: "75px",
            color: "#FBEAB8",
          }}
        >
          <div>
            <h1 style={{ fontSize: "clamp(30px, 3.5vw, 90px)" }}>
              TRANSFORM YOUR HR. EMPOWER YOUR TEAM
            </h1>
            <h1 style={{ fontSize: "clamp(10px, 3.5vw, 50px)" }}>
              AllHands is an advanced, forward-thinking system designed to elevate productivity and streamline performance across your company
            </h1>
          </div>
          <Image
            src="/img_menu.png"
            width={900}
            height={900}
            alt="Company logo"
            style={{ width: "50vw", height: "auto", objectFit: "contain" }}
            priority
          />
        </div>
      </div>
    );
  }

  return (
    <div className="appBackground">
      <TopBar />

      <div className="pageWrapper">
        <div
          className="pageCard"
          style={{
            maxWidth: 1500,
            width: "min(1500px, calc(100vw - 72px))",
          }}
        >
          <div className="employeesHeader">
            <div style={{ display: "flex", alignItems: "center", gap: 18, flexWrap: "wrap" }}>
              <div
                style={{
                  width: 96,
                  height: 96,
                  borderRadius: 22,
                  background: "rgba(255,255,255,0.06)",
                  border: "1px solid rgba(255,255,255,0.14)",
                  boxShadow: "0 14px 42px rgba(0,0,0,0.28)",
                  display: "flex",
                  justifyContent: "center",
                  alignItems: "center",
                  overflow: "hidden",
                  flex: "0 0 auto",
                }}
              >
                {logoUrl ? (
                  <Image
                    src={logoUrl}
                    width={96}
                    height={96}
                    alt="Company logo"
                    style={{ width: "100%", height: "100%", objectFit: "contain" }}
                  />
                ) : (
                  <div style={{ opacity: 0.92, fontSize: 34, fontWeight: 900 }}>A</div>
                )}
              </div>

              <div
                className="employeesTitle"
                style={{
                  fontSize: "clamp(54px, 4.6vw, 84px)",
                  lineHeight: 1.02,
                  letterSpacing: 0.3,
                }}
              >
                {company?.name ?? "Company"}
              </div>
            </div>
          </div>

          {(userLoading || loading) && (
            <div style={{ marginTop: 14, opacity: 0.85, fontSize: 16 }}>
              Loading company data…
            </div>
          )}
          {err && <div className="errorMessage">{err}</div>}
          {!!company && !err && !loading && (
            <div style={{ marginTop: 18 }}>
              <div
                style={{
                  marginTop: 8,
                  padding: "18px 20px",
                  borderRadius: 18,
                  background: "rgba(255,255,255,0.035)",
                  border: "1px solid rgba(255,255,255,0.075)",
                }}
              >
                <div style={{ fontSize: 26, fontWeight: 900, marginBottom: 12 }}>
                  About company
                </div>

                <div
                  style={{
                    fontSize: 24,
                    lineHeight: 1.85,
                    opacity: 0.97,
                    whiteSpace: "pre-wrap",
                  }}
                >
                  {company.description ? company.description : "—"}
                </div>
              </div>

              <div
                style={{
                  display: "grid",
                  gap: 12,
                  marginTop: 18,
                  fontSize: 17,
                  padding: "16px 20px",
                  borderRadius: 18,
                  background: "rgba(255,255,255,0.025)",
                  border: "1px solid rgba(255,255,255,0.065)",
                }}
              >
                <Row label="Email domain" value={company.emailDomain ?? "—"} />
                <Row
                  label="Domain validation enforced"
                  value={company.isSameDomainValidationEnforced ? "Yes" : "No"}
                />
                <Row label="IANA time zone" value={company.ianaTimeZone ?? "—"} />
                <Row
                  label="Work days"
                  value={(company.workDays ?? []).length ? (company.workDays ?? []).join(", ") : "—"}
                />
                <Row label="Created at" value={fmtIso(company.createdAt)} />
              </div>

              <div style={{ marginTop: 16, opacity: 0.78, fontSize: 14 }}>
                You’re signed in as: {toStr((user as any)?.email ?? (user as any)?.Email ?? "—")}
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

function Row({ label, value }: { label: string; value: string }) {
  return (
    <div style={{
      display: "flex",
      gap: 16,
      alignItems: "baseline",
      flexWrap: "wrap"
    }}>
      <div style={{
        width: 260,
        opacity: 0.78
      }}>
        {label}
      </div>
      <div style={{
        opacity: 0.98,
        wordBreak: "break-word"
      }}>
        {value}
      </div>
    </div>
  );
}
