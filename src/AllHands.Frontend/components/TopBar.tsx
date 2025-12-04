"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useCurrentUser } from "@/hooks/currentUser";

export default function TopBar() {
  const pathname = usePathname();
  const router = useRouter();
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
        {!loading && isLoggedIn && (
          <button type="button" className="navLink" onClick={handleLogout}>
            Logout
          </button>
        )}
        {!loading && !isLoggedIn && pathname !== "/login" && (
          <Link href="/login" className="navLink">
            Login
          </Link>
        )}
      </div>
    </div>
  );
}
