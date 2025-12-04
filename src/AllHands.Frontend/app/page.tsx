import Link from "next/link";
import Image from "next/image";
import TopBar from "@/components/TopBar";

export default function Menu() {
  return (
    <div style={{ 
          backgroundColor: "#0D081E", 
          minHeight: "100vh" }}>
      <TopBar />
      <div style={{ 
        minHeight: "calc(100vh - 100px)", 
        display: "flex",    
        justifyContent: "center",
        alignItems: "center",
        textAlign: "center",
        padding: "75px",
        color: "#FBEAB8",}}>
        <div>
          <h1 style={{fontSize: "clamp(30px, 3.5vw, 90px)"}}>
            TRANSFORM YOUR HR. EMPOWER YOUR TEAM
          </h1>
          <h1 style={{fontSize: "clamp(10px, 3.5vw, 50px)"}}>
            AllHands is an advanced, forward-thinking system designed to elevate productivity
            and streamline performance across your company
          </h1>
        </div>
        <Image
          src="/img_menu.png"  
          width={900}               
          height={900}              
          alt="Company logo"  
          style={{
            width: "50vw",
            height: "auto",
            objectFit: "contain",}}  
        />
      </div>
    </div>
  );
}
