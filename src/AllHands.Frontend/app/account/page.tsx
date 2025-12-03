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

type PositionDto = {
  Id: string;
  Name: string;
};

type CompanyDto = {
  Id: string;
  Name: string;
};

type EmployeeDto = {
  Id: string;
  FirstName: string;
  MiddleName?: string | null;
  LastName: string;
  Email: string;
  PhoneNumber?: string | null;
  Position: PositionDto;
};

type RoleDto = {
  Id: string;
  Name: string;
  IsDefault: boolean;
};

type EmployeeDetailsDto = {
  EmployeeId: string;
  FirstName: string;
  MiddleName?: string | null;
  LastName: string;
  Email: string;
  PhoneNumber?: string | null;
  Status: string;
  WorkStartDate: string;
  Manager: EmployeeDto;
  Position: PositionDto;
  Company: CompanyDto;
  Role: RoleDto | null;
};

type AccountDto = {
  EmployeeId: string;
  FirstName: string;
  MiddleName?: string | null;
  LastName: string;
  Email: string;
  Company: CompanyDto;
};

type GetAccountsResult = {
  Accounts: AccountDto[];
};

type ErrorResponse = {
  ErrorMessage?: string;
};

type ApiResponse<T> = {
  Data: T | null;
  Error: ErrorResponse | null;
};

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

  const [accounts, setAccounts] = useState<AccountDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [resetSending, setResetSending] = useState(false);
  const [resetInfo, setResetInfo] = useState<string | null>(null);
  const [navOpen, setNavOpen] = useState(false);

  const loadData = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);

      const profileRes = await fetch(`${ACCOUNT_API}/details`, {
        method: "GET",
        credentials: "include",
      });

      if (!profileRes.ok) {
        throw new Error("Failed to load profile.");
      }

      const profileJson =
        (await profileRes.json()) as ApiResponse<EmployeeDetailsDto>;
      const user = profileJson.Data;

      if (!user) {
        const msg = profileJson.Error?.ErrorMessage || "Profile data is empty.";
        throw new Error(msg);
      }

      setProfile({
        lastName: user.LastName ?? "",
        firstName: user.FirstName ?? "",
        middleName: user.MiddleName ?? "",
        email: user.Email ?? "",
        phoneNumber: user.PhoneNumber ?? "",
        position: user.Position?.Name ?? "",
        company: user.Company?.Name ?? "",
        companyId: user.Company?.Id ?? "",
        managerName: user.Manager
          ? `${user.Manager.FirstName} ${user.Manager.LastName}`
          : "",
        roleName: user.Role?.Name ?? "",
      });

      const accountsRes = await fetch(ACCOUNTS_API, {
        method: "GET",
        credentials: "include",
      });

      if (!accountsRes.ok) {
        throw new Error("Failed to load accounts.");
      }

      const accountsJson =
        (await accountsRes.json()) as ApiResponse<GetAccountsResult>;
      setAccounts(accountsJson.Data?.Accounts ?? []);

      try {
        const avatarRes = await fetch(`${ACCOUNT_API}/avatar`, {
          method: "GET",
          credentials: "include",
        });

        if (avatarRes.ok) {
          const blob = await avatarRes.blob();
          const url = URL.createObjectURL(blob);
          setPhotoPreview(url);
        } else {
          setPhotoPreview(null);
        }
      } catch {
        setPhotoPreview(null);
      }
    } catch (e: any) {
      setError(e?.message || "Unexpected error while loading data.");
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadData();
  }, [loadData]);

  const LoadFileDialog = () => {
    fileInputRef.current?.click();
  };

  const uploadPhoto = async (file: File) => {
    const formData = new FormData();
    formData.append("File", file);

    const res = await fetch(`${ACCOUNT_API}/avatar`, {
      method: "PUT",
      credentials: "include",
      body: formData,
    });

    if (!res.ok) {
      throw new Error("Failed to upload photo.");
    }
  };

  const onFileSelected = async (e: ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    const url = URL.createObjectURL(file);
    setPhotoPreview(url);

    try {
      await uploadPhoto(file);
    } catch (err: any) {
      setError(err?.message || "Could not upload photo.");
    }
  };

  const changeField =
    (field: keyof typeof profile) =>
      (e: ChangeEvent<HTMLInputElement>) => {
        setProfile((prev) => ({ ...prev, [field]: e.target.value }));
      };

  const handleSave = async () => {
    try {
      setSaving(true);
      setError(null);

      const payload = {
        FirstName: profile.firstName,
        MiddleName: profile.middleName || null,
        LastName: profile.lastName,
        PhoneNumber: profile.phoneNumber || null,
      };

      const res = await fetch(ACCOUNT_API, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(payload),
      });

      if (!res.ok) {
        throw new Error("Failed to save profile.");
      }

      alert("Profile saved successfully.");
    } catch (e: any) {
      setError(e?.message || "Failed to save changes.");
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
        body: JSON.stringify({ CompanyId: companyId }),
      });

      if (!res.ok) {
        throw new Error("Failed to switch company profile.");
      }

      await loadData();
    } catch (e: any) {
      setError(e?.message || "Failed to switch company profile.");
    } finally {
      setIsLoading(false);
    }
  };

  const handleRequestPasswordReset = async () => {
    try {
      setResetSending(true);
      setError(null);
      setResetInfo(null);

      const targetEmail = profile.email;

      const res = await fetch(`${ACCOUNT_API}/reset-password`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ Email: targetEmail }),
      });

      if (!res.ok) {
        let message = "Failed to send password reset link.";
        try {
          const data = (await res.json()) as ApiResponse<unknown>;
          if (data.Error?.ErrorMessage) {
            message = data.Error.ErrorMessage;
          }
        } catch { }
        throw new Error(message);
      }

      setResetInfo("We sent a password reset link to your email.");
    } catch (e: any) {
      setError(e?.message || "Failed to send password reset link.");
    } finally {
      setResetSending(false);
    }
  };

  const handleLogout = () => {
    setNavOpen(false);
    router.push("/");
  };

  const isManager = profile.position
    .toLowerCase()
    .includes("manager");

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
            }}
            onClick={LoadFileDialog}
          />
          <span className="profileAvatarLabel">Upload Photo</span>

          <input
            type="file"
            accept="image/*"
            ref={fileInputRef}
            style={{ display: "none" }}
            onChange={onFileSelected}
          />

          {isLoading ? (
            <p>Loading profile...</p>
          ) : (
            <>
              <div
                style={{
                  display: "flex",
                  flexDirection: "column",
                  gap: "12px",
                }}
              >
                <div className="accRow">
                  <label className="accLable">Last Name</label>
                  <input
                    className="accInput"
                    value={profile.lastName}
                    onChange={changeField("lastName")}
                  />
                </div>
                <div className="accRow">
                  <label className="accLable">First Name</label>
                  <input
                    className="accInput"
                    value={profile.firstName}
                    onChange={changeField("firstName")}
                  />
                </div>
                <div className="accRow">
                  <label className="accLable">Middle Name</label>
                  <input
                    className="accInput"
                    value={profile.middleName}
                    onChange={changeField("middleName")}
                  />
                </div>
                <div className="accRow">
                  <label className="accLable">Email</label>
                  <input className="accInput" value={profile.email} disabled />
                </div>
                <div className="accRow">
                  <label className="accLable">Phone Number</label>
                  <input
                    className="accInput"
                    value={profile.phoneNumber}
                    onChange={changeField("phoneNumber")}
                  />
                </div>
                <div className="accRow">
                  <label className="accLable">Company</label>
                  <input
                    className="accInput"
                    value={profile.company}
                    disabled
                  />
                </div>
                <div className="accRow">
                  <label className="accLable">Manager</label>
                  <input
                    className="accInput"
                    value={profile.managerName}
                    disabled
                  />
                </div>
                <div className="accRow">
                  <label className="accLable">Position</label>
                  <input
                    className="accInput"
                    value={profile.position}
                    disabled
                  />
                </div>
                {profile.roleName && (
                  <div className="accRow">
                    <label className="accLable">Role</label>
                    <input
                      className="accInput"
                      value={profile.roleName}
                      disabled
                    />
                  </div>
                )}
              </div>

              {error && <p className="profileStatusError">{error}</p>}
              {resetInfo && (
                <p className="profileStatusSuccess">{resetInfo}</p>
              )}

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

          {accounts.map((acc) => {
            const isActive = acc.Company.Id === profile.companyId;
            return (
              <button
                key={acc.Company.Id}
                onClick={() => handleCompanySelect(acc.Company.Id)}
                className={
                  "companyButton" +
                  (isActive ? " companyButtonActive" : "")
                }
              >
                {acc.Company.Name}
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
