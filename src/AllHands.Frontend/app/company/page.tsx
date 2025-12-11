"use client";

import { useEffect, useState } from "react";
import TopBar from "@/components/TopBar";

const API_ROOT = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const COMPANY_API = `${API_ROOT}/api/v1/company`;
const COMPANY_LOGO_API = `${API_ROOT}/api/v1/company/logo`;
const NEWS_API = `${API_ROOT}/api/v1/news`;
const TIME_OFF_BALANCES_API = `${API_ROOT}/api/v1/time-off/balances`;
const EMPLOYEE_TIME_OFF_REQUESTS_API = `${API_ROOT}/api/v1/time-off/employees/requests`;

type CompanyDto = {
  name?: string;
  legalName?: string;
};

type NewsItemDto = {
  id: string;
  text: string;
  createdAt?: string;
  author?: {
    firstName?: string;
    lastName?: string;
    email?: string;
  };
};

type TimeOffBalanceDto = {
  typeId?: string;
  typeName?: string;
  typeEmoji?: string | null;
  remainingDays?: number;
  usedDays?: number;
};

type TimeOffRequestDto = {
  id: string;
  employeeName?: string;
  typeName?: string;
  typeEmoji?: string | null;
  startDate: string;
  endDate: string;
  status?: string;
};

export default function CompanyPage() {
  const [company, setCompany] = useState<CompanyDto | null>(null);
  const [news, setNews] = useState<NewsItemDto[]>([]);
  const [balances, setBalances] = useState<TimeOffBalanceDto[]>([]);
  const [upcomingRequests, setUpcomingRequests] = useState<TimeOffRequestDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);

      const opts: RequestInit = { method: "GET", credentials: "include" };

      const companyRes = await fetch(COMPANY_API, opts);
      const newsRes = await fetch(NEWS_API, opts);
      const balancesRes = await fetch(TIME_OFF_BALANCES_API, opts);

      const reqParams = new URLSearchParams();
      reqParams.set("Page", "1");
      reqParams.set("PerPage", "1000");
      const requestsRes = await fetch(
        `${EMPLOYEE_TIME_OFF_REQUESTS_API}?${reqParams.toString()}`,
        opts
      );

      if (!companyRes.ok || !newsRes.ok || !balancesRes.ok) {
        setError("Помилка завантаження даних");
        return;
      }

      const companyJson: any = await companyRes.json();
      const newsJson: any = await newsRes.json();
      const balancesJson: any = await balancesRes.json();

      setCompany(companyJson.data ?? null);
      setNews(newsJson.data?.data ?? []);
      setBalances(balancesJson.data ?? []);

      let allRequests: TimeOffRequestDto[] = [];
      if (requestsRes.ok) {
        const requestsJson: any = await requestsRes.json();
        const raw = requestsJson.data ?? requestsJson.Data ?? null;
        const items =
          Array.isArray(raw?.items) ? raw.items : Array.isArray(raw) ? raw : [];
        allRequests = items;
      } else {
        allRequests = [];
      }

      const today = new Date();
      const from = new Date(today);
      const to = new Date(today);
      from.setDate(from.getDate() - 5);
      to.setDate(to.getDate() + 5);

      const filtered = allRequests.filter((r) => {
        if (r.status && r.status !== "Approved") return false;
        const start = new Date(r.startDate);
        const end = new Date(r.endDate);
        return end >= from && start <= to;
      });

      setUpcomingRequests(filtered);
    } catch (e: any) {
      setError(e?.message || "Сталася помилка");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadData();
  }, []);

  const logoUrl = COMPANY_LOGO_API;

  return (
    <div className="appBackground">
      <TopBar />
      <div className="companyPageWrapper">
        <div className="companyPageCard">
          {loading && <p className="companyTextMuted">Завантаження...</p>}
          {error && <p className="errorMessage">{error}</p>}
          {!loading && !error && (
            <>
              <div className="companyRowSplit">
                <div className="companyCol">
                  <div className="companyCard">
                    <div className="companyHeader">
                      <div className="companyLogo">
                        <img src={logoUrl} alt={company?.name ?? "Company logo"} />
                      </div>
                      <div>
                        <h1 className="companyName">{company?.name ?? "Компанія"}</h1>
                        {company?.legalName && (
                          <p className="companyLegalName">{company.legalName}</p>
                        )}
                      </div>
                    </div>
                  </div>
                  <div className="companyCard">
                    <h2 className="companySectionTitle">Баланс відпусток</h2>
                    {balances.length === 0 && (
                      <p className="companyTextMuted">Немає даних</p>
                    )}
                    <ul className="companyList">
                      {balances.map((b) => (
                        <li key={b.typeId ?? b.typeName} className="companyListItem">
                          <div className="companyListItemRow">
                            <div>
                              <p className="companyPillTitle">
                                {b.typeEmoji && <span>{b.typeEmoji} </span>}
                                {b.typeName ?? "Тип відпустки"}
                              </p>
                              {b.usedDays !== undefined && (
                                <p className="companyPillMeta">
                                  Використано: {b.usedDays}
                                </p>
                              )}
                            </div>
                            <div className="companyPillMeta">
                              Залишок: {b.remainingDays ?? 0} днів
                            </div>
                          </div>
                        </li>
                      ))}
                    </ul>
                  </div>
                </div>
                <div className="companyCol">
                  <div className="companyCard">
                    <h2 className="companySectionTitle">Хто у відпустці (±5 днів)</h2>
                    {upcomingRequests.length === 0 && (
                      <p className="companyTextMuted">
                        Немає відпусток у цьому періоді
                      </p>
                    )}
                    <ul className="companyList">
                      {upcomingRequests.map((r) => (
                        <li key={r.id} className="companyListItem">
                          <p className="companyPillTitle">
                            {r.employeeName ?? "Співробітник"}
                          </p>
                          <p className="companyPillMeta">
                            {r.typeEmoji && <span>{r.typeEmoji} </span>}
                            {r.typeName ?? "Відпустка"}
                          </p>
                          <p className="companyDateRange">
                            {new Date(r.startDate).toLocaleDateString()} –{" "}
                            {new Date(r.endDate).toLocaleDateString()}
                          </p>
                        </li>
                      ))}
                    </ul>
                  </div>
                </div>
              </div>
              <div className="companyRow">
                <div className="companyCard">
                  <h2 className="companySectionTitle">Новини</h2>
                  {news.length === 0 && (
                    <p className="companyTextMuted">Поки що немає новин</p>
                  )}
                  <ul className="companyList">
                    {news.map((item) => (
                      <li key={item.id} className="companyListItem">
                        <div className="companyNewsHeader">
                          <p className="companyNewsTitle">
                            {item.text || "Новина"}
                          </p>
                          {item.createdAt && (
                            <span className="companyNewsDate">
                              {new Date(item.createdAt).toLocaleDateString()}
                            </span>
                          )}
                        </div>
                        {item.author && (
                          <p className="companyPillMeta">
                            Автор:{" "}
                            {[
                              item.author.firstName,
                              item.author.lastName,
                            ]
                              .filter(Boolean)
                              .join(" ") || item.author.email}
                          </p>
                        )}
                      </li>
                    ))}
                  </ul>
                </div>
              </div>
            </>
          )}
        </div>
      </div>
    </div>
  );
}
