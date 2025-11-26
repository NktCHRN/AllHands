import Link from "next/link";

export default function About() {
  return (
    <div
      style={{
        backgroundColor: "#0D081E",
        minHeight: "100vh",
      }}
    >
      <div className="topBar">
        <span
          style={{
            color: "black",
            fontWeight: "bold",
          }}
        >
          AllHands HR
        </span>

        <div
          style={{
            display: "flex",
            gap: "40px",
            alignItems: "center",
          }}
        >
          <Link href="/" className="navLink">Home</Link>
          <Link href="/contact" className="navLink">Contact us</Link>
          <Link href="/login" className="navLink">Login</Link>
        </div>
      </div>
      <div
        style={{
          display: "flex",
          flexDirection: "column",
          gap: "60px",
          padding: "60px clamp(20px, 8vw, 140px)",
          alignItems: "center",
        }}
      >
        <h1 className="sectionLabel">Who we are</h1>
        <div className="infoBox">
          <h2>
            AllHands is a modern workspace solution created by a team of developers
            and designers who understand the challenges of internal company processes.
            We combine technology, analytics, and thoughtful UX to build tools that 
            help companies operate smoother and more transparently. Our mission is to 
            replace spreadsheets, scattered notes, and outdated HR systems with one clean, 
            intelligent platform that every team member can rely on.
          </h2>
        </div>
        <h1 className="sectionLabel">What we do</h1>
        <div className="infoBox">
          <h2>
            AllHands provides companies with a centralized platform to manage employee 
            information, vacation balances, leave requests, and overall availability.
          </h2>
        </div>
        <h1 className="sectionLabel">Why Companies Choose Us</h1>
        <div className="infoBox">
          <h2>
            Fast onboarding: Teams can start using AllHands in minutes — no complicated setup.<br />
            Reduced administrative load: HR teams save hours of manual checking and updating.<br />
            Centralized information: All data is stored in one place instead of multiple tools.<br />
          </h2>
        </div>
        <h1 className="sectionLabel">Our values</h1>
        <div className="infoBox">
          <h2>
            Transparency: We believe that clear access to information strengthens trust within teams.<br />
            Simplicity: The best tools are those that don’t require instruction — just use them.<br />
            Reliability: Companies depend on their internal systems, so we build everything with stability in mind.<br />
            Continuous improvement: We constantly refine features based on real user needs.<br />
          </h2>
        </div>
      </div>
    </div>
  );
}
