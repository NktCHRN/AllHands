"use client";

import Image from "next/image";
import TopBar from "@/components/TopBar";
import { useCurrentUser } from "@/hooks/currentUser";
import { useEffect, useMemo, useState } from "react";

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
  const [err, setErr] = useState<string>("");

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
      setLogoUrl(prev => {
        if (prev) URL.revokeObjectURL(prev);
        return null;
      });
      return;
    }

    let cancelled = false;

    async function loadCompany() {
      setLoading(true);
      setErr("");

      try {
        const headers: Record<string, string> = {};
        if (token) headers["Authorization"] = `Bearer ${token}`;

        const res = await fetch(COMPANY_API, {
          method: "GET",
          headers,
          cache: "no-store",
          credentials: "include",
        });

        if (!res.ok) {
          const text = await res.text().catch(() => "");
          throw new Error(`Company request failed: ${res.status} ${text}`);
        }

        const json = await res.json();
        const dto: CompanyDto | null = json?.data ?? null;

        let nextLogoUrl: string | null = null;

        try {
          const logoRes = await fetch(COMPANY_LOGO_API, {
            method: "GET",
            headers,
            cache: "no-store",
            credentials: "include",
          });

          if (logoRes.ok) {
            const blob = await logoRes.blob();
            nextLogoUrl = URL.createObjectURL(blob);
          }
        } catch {}

        if (cancelled) {
          if (nextLogoUrl) URL.revokeObjectURL(nextLogoUrl);
          return;
        }

        setCompany(dto);

        setLogoUrl(prev => {
          if (prev) URL.revokeObjectURL(prev);
          return nextLogoUrl;
        });
      } catch (e: any) {
        if (!cancelled) {
          setErr(e?.message ? String(e.message) : "Failed to load company");
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    loadCompany();

    return () => {
      cancelled = true;
    };
  }, [isAuthed, token]);

  const baseWrapStyle: React.CSSProperties = {
    backgroundColor: "#0D081E",
    minHeight: "100vh",
  };

  if (!isAuthed) {
    return (
      <div
        style={{
          backgroundColor: "#0D081E",
          minHeight: "100vh",
        }}
      >
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
            <h1
              style={{
                fontSize: "clamp(30px, 3.5vw, 90px)",
              }}
            >
              TRANSFORM YOUR HR. EMPOWER YOUR TEAM
            </h1>
            <h1
              style={{
                fontSize: "clamp(10px, 3.5vw, 50px)",
              }}
            >
              AllHands is an advanced, forward-thinking system designed to elevate productivity and streamline performance across your company
            </h1>
          </div>
          <Image
            src="/img_menu.png"
            width={900}
            height={900}
            alt="Company logo"
            style={{
              width: "50vw",
              height: "auto",
              objectFit: "contain",
            }}
            priority
          />
        </div>
      </div>
    );
  }

  return (
    <div
      style={{
        ...baseWrapStyle,
      }}
    >
      <TopBar />
      <div
        style={{
          minHeight: "calc(100vh - 100px)",
          padding: "55px 75px",
          color: "#FBEAB8",
          display: "flex",
          justifyContent: "space-between",
          alignItems: "flex-start",
          gap: 60,
          flexWrap: "nowrap",
        }}
      >
        <div
          style={{
            flex: "1 1 auto",
            maxWidth: 760,
          }}
        >
          <div
            style={{
              opacity: 0.85,
              fontSize: 14,
              marginBottom: 10,
            }}
          >
            Company overview
          </div>
          <h1
            style={{
              margin: 0,
              fontSize: "clamp(34px, 3.2vw, 64px)",
              lineHeight: 1.1,
            }}
          >
            {company?.name ?? "Company"}
          </h1>
          {(userLoading || loading) && (
            <div
              style={{
                marginTop: 14,
                opacity: 0.85,
                fontSize: 16,
              }}
            >
              Loading company data…
            </div>
          )}
          {!!err && (
            <div
              style={{
                marginTop: 16,
                background: "rgba(255, 60, 60, 0.12)",
                border: "1px solid rgba(255, 60, 60, 0.35)",
                padding: 14,
                borderRadius: 12,
                color: "#ffd3d3",
                maxWidth: 900,
                wordBreak: "break-word",
              }}
            >
              {err}
            </div>
          )}
          {!!company && !err && (
            <div
              style={{
                marginTop: 18,
                background: "rgba(251, 234, 184, 0.06)",
                border: "1px solid rgba(251, 234, 184, 0.18)",
                borderRadius: 16,
                padding: 22,
                lineHeight: 1.55,
              }}
            >
              <div
                style={{
                  marginBottom: 14,
                  opacity: 0.92,
                  fontSize: 18,
                }}
              >
                {company.description ? company.description : "—"}
              </div>
              <div
                style={{
                  display: "grid",
                  gap: 12,
                  marginTop: 10,
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
            </div>
          )}
        </div>
        <div
          style={{
            flex: "0 0 460px",
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
          }}
        >
          <div
            style={{
              width: "100%",
              background: "rgba(251, 234, 184, 0.06)",
              border: "1px solid rgba(251, 234, 184, 0.18)",
              borderRadius: 16,
              padding: 22,
              display: "flex",
              justifyContent: "center",
              alignItems: "center",
              minHeight: 320,
            }}
          >
            {logoUrl ? (
              <Image
                src={logoUrl}
                width={520}
                height={520}
                alt="Company logo"
                style={{
                  width: "100%",
                  maxWidth: 420,
                  height: "auto",
                  objectFit: "contain",
                }}
              />
            ) : (
              <div
                style={{
                  opacity: 0.8,
                  textAlign: "center",
                  padding: 24,
                }}
              >
                Logo not available
              </div>
            )}
          </div>
          <div
            style={{
              marginTop: 12,
              opacity: 0.8,
              fontSize: 13,
              alignSelf: "flex-start",
            }}
          >
            You’re signed in as: {toStr((user as any)?.email ?? (user as any)?.Email ?? "—")}
          </div>
        </div>
      </div>
    </div>
  );
}

function Row({ label, value }: { label: string; value: string }) {
  return (
    <div
      style={{
        display: "flex",
        gap: 14,
        alignItems: "baseline",
        flexWrap: "wrap",
      }}
    >
      <div
        style={{
          width: 240,
          opacity: 0.75,
        }}
      >
        {label}
      </div>
      <div
        style={{
          opacity: 0.95,
          wordBreak: "break-word",
        }}
      >
        {value}
      </div>
    </div>
  );
}
