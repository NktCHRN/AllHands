import Link from "next/link";

export default function Contact() {
  return (
    <div style={{ 
          backgroundColor: "#0D081E", 
          minHeight: "100vh" }}>
      <div
        className="topBar"
      >
        <span style={{ 
                color: "black", 
                fontWeight: "bold" }}
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
          <Link href="/login" className="navLink">
            Login
          </Link>
        </div>
      </div>
       <div style={{ 
              minHeight: "calc(100vh - 100px)", 
              display: "flex",    
              justifyContent: "center",
              textAlign: "center",
              padding: "75px 75px",
              color: "#FBEAB8",}}>
              <div>
                <h1 style={{fontSize: "clamp(70px, 3.5vw, 180px)"}}>
                  Contact Us
                </h1>
                <br/>
                <h1 style={{fontSize: "clamp(20px, 3.5vw, 40px)"}}>
                  Having a technical issue?<br/>Problems with the website, bugs, technical malfunction
                </h1>
                <Link href="https://mail.google.com/mail/?view=cm&fs=1&to=stacy.linchuk@gmail.com" 
                    target="_blank"
                    rel="noopener noreferrer"
                    className="contactLink">
                  stacy.linchuk@gmail.com
                </Link>
                <h1 style={{fontSize: "clamp(20px, 3.5vw, 40px)"}}>
                  Want to become a partner or integrate our product?<br/>Co-branding, sales, partnership inquiries
                </h1>
                <Link href="https://mail.google.com/mail/?view=cm&fs=1&to=14nik20@gmail.com" 
                    target="_blank"
                    rel="noopener noreferrer"
                    className="contactLink">
                  14nik20@gmail.com
                </Link>
                <h1 style={{fontSize: "clamp(20px, 3.5vw, 40px)"}}>
                  Are you a journalist or blogger?<br/>Media inquiries, interviews, PR communications
                </h1>
                <Link href="https://mail.google.com/mail/?view=cm&fs=1&to=14nik20@gmail.com" 
                    target="_blank"
                    rel="noopener noreferrer"
                    className="contactLink">
                  14nik20@gmail.com
                </Link>
                <h1 style={{fontSize: "clamp(20px, 3.5vw, 40px)"}}>
                  <br/>Phone Numbers
                </h1>
                <Link href="tel:+380682767749" 
                    className="contactLink">
                  +38-068-276-77-49 <br/>
                </Link>
                <Link href="tel:+380676381007" 
                    className="contactLink">
                  +38-067-638-10-07
                </Link>
              </div>
            </div>
    </div>
  );
}
