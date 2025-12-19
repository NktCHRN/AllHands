"use client";

import Link from "next/link";
import {
  useEffect,
  useRef,
  useState,
  useCallback,
  type ChangeEvent,
} from "react";
import { useRouter } from "next/navigation";
import TopBar from "@/components/TopBar";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const ACCOUNT_API = `${API_ROOT}/api/v1/account`;
const ACCOUNTS_API = `${API_ROOT}/api/v1/accounts`;

export default function Account() {
  const router = useRouter();
  const fileInputRef = useRef<HTMLInputElement | null>(null);

  const [photoPreview, setPhotoPreview] = useState<string | null>(null);

  const [profile, setProfile] = useState({
    lastName: "",
    firstName: "",
    middleName: "",
    email: "",
    phoneNumber: "",
    position: "",
    company: "",
    companyId: "",
    managerName: "",
    roleName: "",
  });

  const [accounts, setAccounts] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [resetSending, setResetSending] = useState(false);
  const [resetInfo, setResetInfo] = useState<string | null>(null);

  const photoPreviewRef = useRef<string | null>(null);

  const loadData = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);

      const profileRes = await fetch(`${ACCOUNT_API}/details`, {
        method: "GET",
        credentials: "include",
      });

      if (!profileRes.ok) throw new Error("Failed to load profile");

      const profileJson = await profileRes.json();
      const user = profileJson.data;

      if (!user) throw new Error(profileJson.error?.errorMessage || "Profile empty");

      setProfile({
        lastName: user.lastName ?? "",
        firstName: user.firstName ?? "",
        middleName: user.middleName ?? "",
        email: user.email ?? "",
        phoneNumber: user.phoneNumber ?? "",
        position: user.position?.name ?? "",
        company: user.company?.name ?? "",
        companyId: user.company?.id ?? "",
        managerName: user.manager
          ? `${user.manager.firstName} ${user.manager.lastName}`
          : "",
        roleName: user.role?.name ?? "",
      });

      const accountsRes = await fetch(ACCOUNTS_API, {
        method: "GET",
        credentials: "include",
      });

      const accountsJson = await accountsRes.json();
      setAccounts(accountsJson.data?.accounts ?? []);

      const avatarRes = await fetch(`${ACCOUNT_API}/avatar`, {
        method: "GET",
        credentials: "include",
      });

      if (avatarRes.ok) {
        const blob = await avatarRes.blob();
        const url = URL.createObjectURL(blob);

        if (photoPreviewRef.current) URL.revokeObjectURL(photoPreviewRef.current);
        photoPreviewRef.current = url;

        setPhotoPreview(url);
      } else {
        if (photoPreviewRef.current) URL.revokeObjectURL(photoPreviewRef.current);
        photoPreviewRef.current = null;

        setPhotoPreview(null);
      }
    } catch (e: any) {
      setError(e.message || "Unexpected error");
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadData();
  }, [loadData]);

  useEffect(() => {
    return () => {
      if (photoPreviewRef.current) URL.revokeObjectURL(photoPreviewRef.current);
    };
  }, []);

  const LoadFileDialog = () => fileInputRef.current?.click();

  const uploadPhoto = async (file: File) => {
    const form = new FormData();
    form.append("File", file);

    const res = await fetch(`${ACCOUNT_API}/avatar`, {
      method: "PUT",
      credentials: "include",
      body: form,
    });

    if (!res.ok) throw new Error("Failed to upload photo");
  };

  const onFileSelected = async (e: ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    const url = URL.createObjectURL(file);

    if (photoPreviewRef.current) URL.revokeObjectURL(photoPreviewRef.current);
    photoPreviewRef.current = url;

    setPhotoPreview(url);

    try {
      await uploadPhoto(file);
    } catch (err: any) {
      setError(err.message);
    } finally {
      e.target.value = "";
    }
  };

  const changeField =
    (field: keyof typeof profile) =>
    (e: ChangeEvent<HTMLInputElement>) => {
      setProfile((p) => ({ ...p, [field]: e.target.value }));
    };

  const handleSave = async () => {
    try {
      setSaving(true);
      setError(null);

      const payload = {
        firstName: profile.firstName,
        middleName: profile.middleName || null,
        lastName: profile.lastName,
        phoneNumber: profile.phoneNumber || null,
      };

      const res = await fetch(ACCOUNT_API, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(payload),
      });

      if (!res.ok) throw new Error("Failed to save profile");
      alert("Profile saved successfully");
    } catch (e: any) {
      setError(e.message);
    } finally {
      setSaving(false);
    }
  };

  const handleCompanySelect = async (companyId: string) => {
    try {
      setIsLoading(true);
      setError(null);

      const res = await fetch(`${ACCOUNT_API}/relogin`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ companyId }),
      });

      if (!res.ok) throw new Error("Failed to switch company");

      await loadData();
    } catch (e: any) {
      setError(e.message);
    } finally {
      setIsLoading(false);
    }
  };

  const handleRequestPasswordReset = async () => {
    try {
      setResetSending(true);
      setError(null);
      setResetInfo(null);

      const res = await fetch(`${ACCOUNT_API}/reset-password`, {
        method: "POST",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email: profile.email }),
      });

      if (!res.ok) {
        const data = await res.json();
        throw new Error(data.error?.errorMessage || "Failed to send reset link");
      }

      setResetInfo("We sent a password reset link.");
    } catch (e: any) {
      setError(e.message);
    } finally {
      setResetSending(false);
    }
  };

  return (
    <div className="appBackground">
      <TopBar />

      <div className="profileLayout">
        <div className="profileMain">
          <h1 className="profileTitle">My Profile</h1>

          <div
            className="profileAvatar"
            style={{
              backgroundImage: photoPreview
                ? `url(${photoPreview})`
                : "linear-gradient(#b388ff,#fbeab8)",
              backgroundSize: "cover",
              backgroundPosition: "center",
              backgroundRepeat: "no-repeat",
              aspectRatio: "1 / 1",
            }}
            onClick={LoadFileDialog}
          />

          <span className="profileAvatarLabel">Upload Photo</span>

          <input
            type="file"
            ref={fileInputRef}
            accept="image/*"
            style={{ display: "none" }}
            onChange={onFileSelected}
          />

          {isLoading ? (
            <p>Loading profile...</p>
          ) : (
            <>
              <div style={{ display: "flex", flexDirection: "column", gap: "12px" }}>
                <div className="accRow">
                  <label className="accLable">Last Name</label>
                  <input className="accInput" value={profile.lastName} onChange={changeField("lastName")} />
                </div>

                <div className="accRow">
                  <label className="accLable">First Name</label>
                  <input className="accInput" value={profile.firstName} onChange={changeField("firstName")} />
                </div>

                <div className="accRow">
                  <label className="accLable">Middle Name</label>
                  <input className="accInput" value={profile.middleName} onChange={changeField("middleName")} />
                </div>

                <div className="accRow">
                  <label className="accLable">Email</label>
                  <input className="accInput" value={profile.email} disabled />
                </div>

                <div className="accRow">
                  <label className="accLable">Phone Number</label>
                  <input className="accInput" value={profile.phoneNumber} onChange={changeField("phoneNumber")} />
                </div>

                <div className="accRow">
                  <label className="accLable">Company</label>
                  <input className="accInput" value={profile.company} disabled />
                </div>

                <div className="accRow">
                  <label className="accLable">Manager</label>
                  <input className="accInput" value={profile.managerName} disabled />
                </div>

                <div className="accRow">
                  <label className="accLable">Position</label>
                  <input className="accInput" value={profile.position} disabled />
                </div>

                {profile.roleName && (
                  <div className="accRow">
                    <label className="accLable">Role</label>
                    <input className="accInput" value={profile.roleName} disabled />
                  </div>
                )}
              </div>

              {error && <p className="profileStatusError">{error}</p>}
              {resetInfo && <p className="profileStatusSuccess">{resetInfo}</p>}

              <div className="profileButtons">
                <button
                  onClick={handleSave}
                  disabled={saving}
                  className="profileButtonPrimary"
                  style={{ opacity: saving ? 0.7 : 1 }}
                >
                  {saving ? "Saving..." : "Save Changes"}
                </button>

                <button
                  onClick={handleRequestPasswordReset}
                  disabled={resetSending}
                  className="profileButtonSecondary"
                  style={{ opacity: resetSending ? 0.7 : 1 }}
                >
                  {resetSending ? "Sending..." : "Reset Password"}
                </button>
              </div>
            </>
          )}
        </div>

        <div className="profileSide">
          <h2 className="companiesTitle">Your Companies</h2>

          {accounts.map((acc: any) => {
            const isActive = acc.company.id === profile.companyId;
            return (
              <button
                key={acc.company.id}
                onClick={() => handleCompanySelect(acc.company.id)}
                className={
                  "companyButton" + (isActive ? " companyButtonActive" : "")
                }
              >
                {acc.company.name}
              </button>
            );
          })}

          <span className="companiesHint">
            Selecting a company reloads the profile for that company.
          </span>
        </div>
      </div>
    </div>
  );
}
