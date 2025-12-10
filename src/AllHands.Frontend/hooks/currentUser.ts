"use client";

import { useEffect, useState } from "react";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const ACCOUNT_API = `${API_ROOT}/api/v1/account`;

export type AccountDetails = {
  employeeId: string;
  firstName: string;
  middleName?: string | null;
  lastName: string;
  email: string;
  phoneNumber?: string | null;
  status: string;
  roles?: string[] | null;
  permissions?: string[] | null;
};

let cachedUser: AccountDetails | null = null;

export function useCurrentUser() {
  const [user, setUser] = useState<AccountDetails | null>(cachedUser);
  const [loading, setLoading] = useState(!cachedUser);

  useEffect(() => {
    let cancelled = false;

    async function fetchUser() {
      try {
        setLoading(true);

        const res = await fetch(`${ACCOUNT_API}`, {
          method: "GET",
          credentials: "include",
        });

        if (!res.ok) {
          if (!cancelled) {
            cachedUser = null;
            setUser(null);
          }
          return;
        }

        const json = await res.json();
        const data: AccountDetails | null = json.data ?? json.Data ?? json ?? null;

        if (!cancelled) {
          cachedUser = data;
          setUser(data);
        }
      } catch (err) {
        if (!cancelled) {
          cachedUser = null;
          setUser(null);
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    }

    fetchUser();

    return () => {
      cancelled = true;
    };
  }, []);

  const logout = async () => {
    try {
      await fetch(`${ACCOUNT_API}/logout`, {
        method: "POST",
        credentials: "include",
      });
    } catch {}

    cachedUser = null;
    setUser(null);
  };

  const isLoggedIn = !!user;

  return { user, isLoggedIn, loading, logout };
}
