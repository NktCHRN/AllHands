"use client";

import Link from "next/link";
import { useRouter, usePathname } from "next/navigation";
import { useCurrentUser } from "@/hooks/currentUser";
import { useState } from "react";

export default function TopBar() {
  const router = useRouter();
  const pathname = usePathname();
  const { isLoggedIn, loading, logout } = useCurrentUser();
  const [navOpen, setNavOpen] = useState(false);

  const handleLogout = async () => {
    setNavOpen(false);
    await logout();
    router.push("/login");
  };

  const closeMenu = () => setNavOpen(false);

  return (
    <div className="topBar">
      <div className="topBarLeft">
        <span className="topBarBrand">AllHands HR</span>
      </div>

      <div className="topBarRight">
        {pathname !== "/" && <Link href="/" className="navLink">Home</Link>}
        {pathname !== "/about" && <Link href="/about" className="navLink">About</Link>}
        {pathname !== "/contact" && <Link href="/contact" className="navLink">Contact Us</Link>}

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
                <Link href="/management" className="navDropdownItem" onClick={closeMenu}>
                  Dashboard
                </Link>
                <Link href="/time-off/request" className="navDropdownItem" onClick={closeMenu}>
                  Request Time Off
                </Link>
                <Link href="/time-off/requests" className="navDropdownItem" onClick={closeMenu}>
                  My Time-Off Requests
                </Link>
                <Link href="/employees" className="navDropdownItem" onClick={closeMenu}>
                  Employees
                </Link>
                <Link href="/employees/new" className="navDropdownItem" onClick={closeMenu}>
                  Create Employee
                </Link>

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
