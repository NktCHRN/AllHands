"use client";

import Link from "next/link";
import { useRouter, usePathname } from "next/navigation";
import { useCurrentUser } from "@/hooks/currentUser";

export default function TopBar() {
  const router = useRouter();
  const pathname = usePathname();
  const { isLoggedIn, loading, logout } = useCurrentUser();

  const handleLogout = async () => {
    await logout();
    router.push("/login");
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

        {!loading &&
          (!isLoggedIn ? (
            pathname !== "/login" && (
              <Link href="/login" className="navLink">
                Login
              </Link>
            )
          ) : (
            <>
              {pathname !== "/account" && (
                <Link href="/account" className="navLink">
                  My Profile
                </Link>
              )}
              <button
                type="button"
                onClick={handleLogout}
                className="navLink"
                style={{ background: "transparent", border: "none", padding: 0 }}
              >
                Logout
              </button>
            </>
          ))}
      </div>
    </div>
  );
}
