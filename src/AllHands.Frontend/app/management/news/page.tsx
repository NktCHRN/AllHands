"use client";

import { useEffect, useMemo, useState } from "react";
import TopBar from "@/components/TopBar";
import { useCurrentUser } from "@/hooks/currentUser";

type NewsDto = {
  id: string;
  text: string;
  createdAt?: string;
  editedAt?: string;
};

function lower(v: any) {
  return String(v ?? "").toLowerCase().trim();
}

function getPermsLower(user: any): string[] {
  const raw =
    (user?.permissions as string[] | null) ??
    (user?.Permissions as string[] | null) ??
    [];
  return Array.isArray(raw) ? raw.map((p) => lower(p)) : [];
}

function hasPerm(perms: string[], perm: string) {
  const set = new Set(perms);
  return set.has(lower(perm));
}

function pickArray(obj: any): any[] {
  const a =
    obj?.data?.data ??
    obj?.data?.Data ??
    obj?.Data?.Data ??
    obj?.Data?.data ??
    obj?.data ??
    obj?.Data ??
    obj;
  return Array.isArray(a) ? a : [];
}

function pickStr(obj: any, ...keys: string[]) {
  for (const k of keys) {
    const v = obj?.[k];
    if (v !== undefined && v !== null) return String(v);
  }
  return "";
}

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const NEWS_API = `${API_ROOT}/api/v1/news`;

export default function NewsManagment() {
  const { user, loading } = useCurrentUser() as any;
  const perms = useMemo(() => getPermsLower(user), [user]);
  const canCreate = useMemo(() => hasPerm(perms, "news.create"), [perms]);
  const canEdit = useMemo(() => hasPerm(perms, "news.edit"), [perms]);
  const canDelete = useMemo(() => hasPerm(perms, "news.delete"), [perms]);
  const [items, setItems] = useState<NewsDto[]>([]);
  const [newText, setNewText] = useState("");
  const [editId, setEditId] = useState<string | null>(null);
  const [editText, setEditText] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  const loadNews = async () => {
    setError(null);
    try {
      const params = new URLSearchParams({ perPage: "100", page: "1" });
      const res = await fetch(`${NEWS_API}?${params.toString()}`, {
        method: "GET",
        credentials: "include",
      });
      if (!res.ok) throw new Error(`Failed to load news (${res.status})`);
      const json = await res.json();
      const list = pickArray(json);
      const mapped: NewsDto[] = list.map((n: any) => ({
        id: pickStr(n, "id", "Id"),
        text: pickStr(n, "text", "Text"),
        createdAt: pickStr(n, "createdAt", "CreatedAt"),
        editedAt: pickStr(n, "editedAt", "EditedAt"),
      }));
      setItems(mapped.filter((x) => x.id && x.text));
    } catch (e: any) {
      setError(e?.message ? String(e.message) : "Failed to load news.");
    }
  };

  useEffect(() => {
    void loadNews();
  }, []);

  const publishNews = async () => {
    setError(null);
    const text = newText.trim();
    if (!text) {
      setError("Text is empty. Please write something.");
      return;
    }
    if (!canCreate) {
      setError("You don’t have permission to create news (news.create).");
      return;
    }
    setBusy(true);
    try {
      const res = await fetch(NEWS_API, {
        method: "POST",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ text }),
      });
      if (!res.ok) throw new Error(`Failed to publish (${res.status})`);
      setNewText("");
      await loadNews();
    } catch (e: any) {
      setError(e?.message ? String(e.message) : "Failed to publish news.");
    } finally {
      setBusy(false);
    }
  };

  const startEdit = (n: NewsDto) => {
    setError(null);
    setEditId(n.id);
    setEditText(n.text);
  };

  const cancelEdit = () => {
    setEditId(null);
    setEditText("");
    setError(null);
  };

  const saveEdit = async () => {
    setError(null);
    if (!editId) return;
    if (!canEdit) {
      setError("You don’t have permission to edit news (news.edit).");
      return;
    }
    const text = editText.trim();
    if (!text) {
      setError("Text is empty. Please write something.");
      return;
    }
    setBusy(true);
    try {
      const res = await fetch(`${NEWS_API}/${encodeURIComponent(editId)}`, {
        method: "PUT",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ id: editId, text }),
      });
      if (!res.ok) throw new Error(`Failed to update (${res.status})`);
      cancelEdit();
      await loadNews();
    } catch (e: any) {
      setError(e?.message ? String(e.message) : "Failed to update news.");
    } finally {
      setBusy(false);
    }
  };

  const removeItem = async (id: string) => {
    setError(null);
    if (!canDelete) {
      setError("You don’t have permission to delete news (news.delete).");
      return;
    }
    setBusy(true);
    try {
      const res = await fetch(`${NEWS_API}/${encodeURIComponent(id)}`, {
        method: "DELETE",
        credentials: "include",
      });
      if (!res.ok) throw new Error(`Failed to delete (${res.status})`);
      setItems((prev) => prev.filter((x) => x.id !== id));
      if (editId === id) cancelEdit();
    } catch (e: any) {
      setError(e?.message ? String(e.message) : "Failed to delete news.");
    } finally {
      setBusy(false);
    }
  };

  const disableCreateUI = loading || busy || !canCreate;
  const disableEditUI = loading || busy || !canEdit;
  const disableCancelUI = loading || busy;
  const disableUpdateBtn = loading || busy || !canEdit;
  const disableDeleteBtn = loading || busy || !canDelete;

  return (
    <div className="appBackground">
      <TopBar />
      <div className="pageWrapper">
        <div className="pageCard">
          <div className="employeesHeader">
            <div>
              <div className="employeesTitle">News management</div>
            </div>
          </div>
          {error && <div className="errorMessage">{error}</div>}
          <div style={{ display: "flex", flexDirection: "column", gap: 18 }}>
            <div
              style={{
                background: "#150d2f",
                borderRadius: 18,
                padding: 18,
                border: "1px solid rgba(255,255,255,0.08)",
              }}
            >
              <div style={{ fontSize: 22, fontWeight: 800, marginBottom: 10 }}>
                New post
              </div>
              {!loading && !canCreate && (
                <div style={{ marginBottom: 12, color: "#ff7a7a" }}>
                  You don’t have permission to create news
                </div>
              )}
              <div style={{ display: "flex", gap: 12, flexWrap: "wrap" }}>
                <input
                  className="inputText"
                  style={{ flex: 1, minWidth: 260 }}
                  placeholder="Write a news text…"
                  value={newText}
                  onChange={(e) => setNewText(e.target.value)}
                  disabled={disableCreateUI}
                />
                <button
                  type="button"
                  className="button"
                  onClick={publishNews}
                  disabled={disableCreateUI}
                  style={{
                    padding: "10px 18px",
                    fontSize: 16,
                    opacity: disableCreateUI ? 0.55 : 1,
                    cursor: disableCreateUI ? "not-allowed" : "pointer",
                  }}
                >
                  Publish
                </button>
              </div>
              {loading && (
                <div style={{ marginTop: 10, opacity: 0.8 }}>Loading user…</div>
              )}
            </div>
            <div
              style={{
                background: "#150d2f",
                borderRadius: 18,
                padding: 18,
                border: "1px solid rgba(255,255,255,0.08)",
              }}
            >
              <div
                style={{
                  display: "flex",
                  alignItems: "baseline",
                  justifyContent: "space-between",
                  gap: 12,
                }}
              >
                <div style={{ fontSize: 22, fontWeight: 800 }}>
                  Published news
                </div>
                <div className="employeesTotal">Total: {items.length}</div>
              </div>
              <div style={{ marginTop: 14 }}>
                {items.length === 0 ? (
                  <div
                    style={{
                      opacity: 0.75,
                      padding: 14,
                      borderRadius: 14,
                      border: "1px dashed rgba(255,255,255,0.15)",
                    }}
                  >
                    No published news yet.
                  </div>
                ) : (
                  <div style={{ display: "flex", flexDirection: "column", gap: 12 }}>
                    {items.map((n) => {
                      const isEditing = editId === n.id;
                      const updated = n.editedAt || n.createdAt || "";
                      return (
                        <div
                          key={n.id}
                          style={{
                            background: "rgba(255,255,255,0.03)",
                            borderRadius: 16,
                            padding: 14,
                            border: "1px solid rgba(255,255,255,0.08)",
                            display: "flex",
                            gap: 12,
                            alignItems: "flex-start",
                            justifyContent: "space-between",
                            flexWrap: "wrap",
                          }}
                        >
                          <div style={{ flex: 1, minWidth: 260 }}>
                            <div
                              style={{
                                display: "flex",
                                justifyContent: "space-between",
                                gap: 10,
                                alignItems: "baseline",
                                marginBottom: 8,
                              }}
                            >
                              <div style={{ fontWeight: 700 }}>Published</div>
                              <div style={{ fontSize: 12, opacity: 0.7 }}>
                                {updated ? `Updated: ${new Date(updated).toLocaleString()}` : ""}
                              </div>
                            </div>
                            {!isEditing && (
                              <div style={{ whiteSpace: "pre-wrap", lineHeight: 1.5 }}>
                                {n.text}
                              </div>
                            )}
                            {isEditing && (
                              <div style={{ display: "flex", gap: 10, flexWrap: "wrap" }}>
                                <input
                                  className="inputText"
                                  style={{ flex: 1, minWidth: 260 }}
                                  value={editText}
                                  onChange={(e) => setEditText(e.target.value)}
                                  disabled={disableEditUI}
                                />
                                <button
                                  type="button"
                                  className="button"
                                  onClick={saveEdit}
                                  disabled={disableEditUI}
                                  style={{
                                    padding: "10px 14px",
                                    fontSize: 14,
                                    opacity: disableEditUI ? 0.55 : 1,
                                    cursor: disableEditUI ? "not-allowed" : "pointer",
                                  }}
                                >
                                  Save changes
                                </button>
                                <button
                                  type="button"
                                  className="button"
                                  onClick={cancelEdit}
                                  disabled={disableCancelUI}
                                  style={{
                                    background: "transparent",
                                    border: "1px solid rgba(255,255,255,0.25)",
                                    padding: "10px 14px",
                                    fontSize: 14,
                                    opacity: disableCancelUI ? 0.55 : 1,
                                    cursor: disableCancelUI ? "not-allowed" : "pointer",
                                  }}
                                >
                                  Cancel
                                </button>
                              </div>
                            )}
                            {!loading && isEditing && !canEdit && (
                              <div style={{ marginTop: 10, color: "#ff7a7a" }}>
                                You don’t have permission to edit news (news.edit).
                              </div>
                            )}
                          </div>
                          {!isEditing && (
                            <div style={{ display: "flex", gap: 10, alignItems: "center" }}>
                              <button
                                type="button"
                                className="button"
                                onClick={() => startEdit(n)}
                                disabled={disableUpdateBtn}
                                style={{
                                  padding: "10px 14px",
                                  fontSize: 14,
                                  opacity: disableUpdateBtn ? 0.55 : 1,
                                  cursor: disableUpdateBtn ? "not-allowed" : "pointer",
                                }}
                              >
                                Update
                              </button>
                              <button
                                type="button"
                                className="button"
                                onClick={() => removeItem(n.id)}
                                disabled={disableDeleteBtn}
                                style={{
                                  padding: "10px 14px",
                                  fontSize: 14,
                                  background: "rgba(255, 122, 122, 0.15)",
                                  borderColor: "rgba(255, 122, 122, 0.5)",
                                  opacity: disableDeleteBtn ? 0.55 : 1,
                                  cursor: disableDeleteBtn ? "not-allowed" : "pointer",
                                }}
                              >
                                Delete
                              </button>
                            </div>
                          )}
                        </div>
                      );
                    })}
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
