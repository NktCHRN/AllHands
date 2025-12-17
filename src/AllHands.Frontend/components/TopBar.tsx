"use client";

import Link from "next/link";
import { useRouter, usePathname } from "next/navigation";
import { useCurrentUser } from "@/hooks/currentUser";
import { useEffect, useMemo, useState } from "react";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const EMPLOYEES_API = `${API_ROOT}/api/v1/employees`;

function toStr(v: any) {
  return String(v ?? "").trim();
}

function pick(obj: any, ...keys: string[]) {
  for (const k of keys) {
    const v = obj?.[k];
    if (v !== undefined && v !== null) return v;
  }
  return undefined;
}

function getPermsLower(user: any): string[] {
  const raw =
    (user?.permissions as string[] | null) ??
    (user?.Permissions as string[] | null) ??
    [];
  return Array.isArray(raw) ? raw.map((p) => String(p).toLowerCase()) : [];
}

function hasAny(perms: string[], needed: string[]) {
  const set = new Set(perms);
  return needed.some((p) => set.has(p.toLowerCase()));
}

export default function TopBar() {
  const router = useRouter();
  const pathname = usePathname();
  const { isLoggedIn, loading, logout, user } = useCurrentUser() as any;

  const [navOpen, setNavOpen] = useState(false);
  const [mgmtOpen, setMgmtOpen] = useState(false);
  const [isManager, setIsManager] = useState(false);

  const perms = useMemo(() => getPermsLower(user), [user]);

  const canManageNews = useMemo(
    () => hasAny(perms, ["news.create", "news.edit", "news.delete"]),
    [perms]
  );

  const canManageTimeOffByPerms = useMemo(
    () =>
      hasAny(perms, [
        "timeoffrequest.adminapprove",
        "timeoffbalance.edit",
        "timeofftype.create",
        "timeofftype.edit",
        "timeofftype.delete",
      ]),
    [perms]
  );

  const canManageTimeOff = canManageTimeOffByPerms || isManager;
  const canSeeManagement = canManageNews || canManageTimeOff;

  useEffect(() => {
    let cancelled = false;

    const checkManager = async () => {
      if (!isLoggedIn) {
        setIsManager(false);
        return;
      }

      const userEmployeeId = toStr(pick(user, "employeeId", "EmployeeId", "id", "Id"));
      if (!userEmployeeId) {
        setIsManager(false);
        return;
      }

      try {
        const params = new URLSearchParams({
          perPage: "1",
          page: "1",
          managerId: userEmployeeId,
        });

        const res = await fetch(`${EMPLOYEES_API}?${params.toString()}`, {
          method: "GET",
          credentials: "include",
        });

        if (!res.ok) {
          if (!cancelled) setIsManager(false);
          return;
        }

        const json = await res.json();
        const root = json?.data ?? json?.Data ?? json;
        const payload = root?.data ?? root?.Data ?? root;

        const items =
          payload?.items ??
          payload?.Items ??
          payload?.data ??
          payload?.Data ??
          (Array.isArray(payload) ? payload : []);

        if (!cancelled) setIsManager(Array.isArray(items) && items.length > 0);
      } catch {
        if (!cancelled) setIsManager(false);
      }
    };

    void checkManager();
    return () => {
      cancelled = true;
    };
  }, [isLoggedIn, user]);

  useEffect(() => {
    if (!navOpen) setMgmtOpen(false);
  }, [navOpen]);

  useEffect(() => {
    if (!canSeeManagement) setMgmtOpen(false);
  }, [canSeeManagement]);

  const handleLogout = async () => {
    setNavOpen(false);
    setMgmtOpen(false);
    await logout();
    router.push("/login");
  };

  const closeMenu = () => {
    setNavOpen(false);
    setMgmtOpen(false);
  };

  return (
    <div className="topBar">
      <div className="topBarLeft">
        <span className="topBarBrand">AllHands HR</span>
      </div>
      <div className="topBarRight">
        {pathname !== "/" && (
          <Link href="/" className="navLink">
            Home
          </Link>
        )}
        {pathname !== "/about" && (
          <Link href="/about" className="navLink">
            About
          </Link>
        )}
        {pathname !== "/contact" && (
          <Link href="/contact" className="navLink">
            Contact Us
          </Link>
        )}
        {!loading && !isLoggedIn && pathname !== "/login" && (
          <Link href="/login" className="navLink">
            Login
          </Link>
        )}
        {!loading && isLoggedIn && (
          <div className="navDropdownWrapper">
            <button
              type="button"
              className="navDropdownToggle"
              onClick={() => setNavOpen((o) => !o)}
            >
              My Pages ▾
            </button>
            {navOpen && (
              <div className="navDropdownMenu">
                <Link href="/account" className="navDropdownItem" onClick={closeMenu}>
                  My Profile
                </Link>
                <Link href="/company" className="navDropdownItem" onClick={closeMenu}>
                  Company
                </Link>
                <Link href="/time-off/requests" className="navDropdownItem" onClick={closeMenu}>
                  My Time-Off Requests
                </Link>
                <Link href="/employees" className="navDropdownItem" onClick={closeMenu}>
                  Employees
                </Link>
                {canSeeManagement && (
                  <div
                    style={{ position: "relative" }}
                    onMouseEnter={() => setMgmtOpen(true)}
                    onMouseLeave={() => setMgmtOpen(false)}
                  >
                    <button
                      type="button"
                      className="navDropdownItem"
                      onClick={() => setMgmtOpen((v) => !v)}
                      aria-expanded={mgmtOpen}
                    >
                      Management {mgmtOpen ? "▾" : "▸"}
                    </button>
                    {mgmtOpen && (
                      <div
                        style={{
                          position: "absolute",
                          right: "100%",
                          top: 0,
                          minWidth: 220,
                          borderRadius: 14,
                          padding: 8,
                          background: "rgba(10, 8, 24, 0.95)",
                          boxShadow: "0 20px 50px rgba(0,0,0,0.45)",
                          backdropFilter: "blur(8px)",
                          zIndex: 9999,
                        }}
                      >
                        {canManageNews && (
                          <Link href="/management/news" className="navDropdownItem" onClick={closeMenu}>
                            News
                          </Link>
                        )}
                        {canManageTimeOff && (
                          <Link href="/management/time-off" className="navDropdownItem" onClick={closeMenu}>
                            Time-off
                          </Link>
                        )}
                      </div>
                    )}
                  </div>
                )}
                <button type="button" className="navDropdownItem" onClick={handleLogout}>
                  Log out
                </button>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
}
