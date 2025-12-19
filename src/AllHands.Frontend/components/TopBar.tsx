"use client";

import Link from "next/link";
import { useRouter, usePathname } from "next/navigation";
import { useCurrentUser } from "@/hooks/currentUser";
import { useEffect, useState } from "react";

export default function TopBar() {
  const router = useRouter();
  const pathname = usePathname();
  const { isLoggedIn, loading, logout } = useCurrentUser() as any;
  const [navOpen, setNavOpen] = useState(false);
  const [mgmtOpen, setMgmtOpen] = useState(false);

  useEffect(() => {
    if (!navOpen) setMgmtOpen(false);
  }, [navOpen]);

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
          <Link href="/" className="navLink">Home</Link>
        )}
        {pathname !== "/about" && (
          <Link href="/about" className="navLink">About</Link>
        )}
        {pathname !== "/contact" && (
          <Link href="/contact" className="navLink">Contact Us</Link>
        )}
        {!loading && !isLoggedIn && pathname !== "/login" && (
          <Link href="/login" className="navLink">Login</Link>
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
                <Link
                  href="/time-off/requests"
                  className="navDropdownItem"
                  onClick={closeMenu}
                >
                  My Time-Off Requests
                </Link>
                <Link href="/employees" className="navDropdownItem" onClick={closeMenu}>
                  Employees
                </Link>
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
                      <Link
                        href="/management/news"
                        className="navDropdownItem"
                        onClick={closeMenu}
                      >
                        News
                      </Link>
                      <Link
                        href="/management/time-off"
                        className="navDropdownItem"
                        onClick={closeMenu}
                      >
                        Time-off
                      </Link>
                    </div>
                  )}
                </div>
                <button
                  type="button"
                  className="navDropdownItem"
                  onClick={handleLogout}
                >
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
