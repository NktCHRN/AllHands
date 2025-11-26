import Link from "next/link";

export default function Login() {
  return (
    <div style={{
      backgroundColor: "#0D081E",
      minHeight: "100vh"
    }}>
      <div
        className="topBar"
      >
        <span style={{
          color: "black",
          fontWeight: "bold"
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
          <Link href="/" className="navLink">
            Home
          </Link>
          <Link href="/about" className="navLink">
            About
          </Link>
          <Link href="/contact" className="navLink">
            Contact Us
          </Link>
        </div>
      </div>
      <div className="loginContainer">
        <h2 style={{
          fontSize: "clamp(35px, 4vw, 60px)",
          fontWeight: "bold",
          margin: 0
        }}>
          Log in
        </h2>
        <input
          id="login"
          type="text"
          placeholder="login"
          className="inputText"
        />
        <input
          id="password"
          type="password"
          placeholder="password"
          className="inputText"
        />
        <button
          className="button"
          type="submit"
          >
          Login
        </button>
      </div>
    </div>
  );
}
