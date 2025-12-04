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
  workStartDate?: string | null;
  manager?: {
    id: string;
    firstName: string;
    middleName?: string | null;
    lastName: string;
    email: string;
    phoneNumber?: string | null;
    position?: { id: string; name: string } | null;
  } | null;

  position?: { id: string; name: string } | null;
  company?: { id: string; name?: string | null } | null;

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
      if (cachedUser) {
        setUser(cachedUser);
        setLoading(false);
        return;
      }

      try {
        setLoading(true);

        const res = await fetch(`${ACCOUNT_API}/details`, {
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

        const data: AccountDetails | null =
          json.data ?? json.Data ?? null;

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
