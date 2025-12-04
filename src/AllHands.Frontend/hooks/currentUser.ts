"use client";

import { useEffect, useState } from "react";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const ACCOUNT_API = `${API_ROOT}/api/v1/account`;

type AccountDetails = {
  Roles?: string[] | null;
};

type ErrorResponse = {
  ErrorMessage?: string;
};

type ApiResponse<T> = {
  Data?: T | null;
  Error?: ErrorResponse | null;
};

let cachedUser: AccountDetails | null = null;
let cachedLoaded = false;

export function useCurrentUser() {
  const [user, setUser] = useState<AccountDetails | null>(cachedUser);
  const [loading, setLoading] = useState(!cachedLoaded);

  useEffect(() => {
    if (cachedLoaded) return;

    let cancelled = false;

    async function load() {
      try {
        setLoading(true);

        const res = await fetch(`${ACCOUNT_API}/details`, {
          method: "GET",
          credentials: "include",
        });

        if (!res.ok) {
          if (!cancelled) {
            cachedUser = null;
            cachedLoaded = true;
            setUser(null);
          }
          return;
        }

        const json = (await res.json()) as ApiResponse<AccountDetails>;
        const data = json.Data ?? null;

        if (!cancelled) {
          cachedUser = data;
          cachedLoaded = true;
          setUser(data);
        }
      } catch {
        if (!cancelled) {
          cachedUser = null;
          cachedLoaded = true;
          setUser(null);
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    void load();

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
    cachedLoaded = false;
    setUser(null);
  };

  const isLoggedIn = !!user;

  return { user, isLoggedIn, loading, logout };
}
