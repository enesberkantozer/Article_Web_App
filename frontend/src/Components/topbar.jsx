import React from "react";
import { Link } from "react-router-dom";
import logo from "../logo.svg";

const Topbar = () => {
    return (
        <nav className="navbar navbar-expand navbar-dark bg-dark" aria-label="Dark offcanvas navbar"
            style={{ padding: "2vh 5vw", position: "fixed", top: "0px", zIndex: 1, width: "84%", margin: "0 8vw", borderBottomLeftRadius: "50px", borderBottomRightRadius: "50px" }}>
            <Link className="navbar-brand" style={{paddingRight: "1vw"}} to="/">
                <img src={logo} width="30" height="30" className="d-inline-block align-top"
                    alt="Site Logosu" />
                Makale Yükleme Sistemi
            </Link>
            <button className="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNavAltMarkup" aria-controls="navbarNavAltMarkup" aria-expanded="false" aria-label="Toggle navigation">
                <span className="navbar-toggler-icon"></span>
            </button>
            <div className="collapse navbar-collapse" id="navbarNavAltMarkup" style={{display: "flex", justifyContent: "flex-end"}}>
                <div className="navbar-nav">
                    <Link className="nav-item nav-link active" to="/makaleYukle">Makale Yükle</Link>
                    <Link className="nav-item nav-link active" to="/durumKontrol">Durum Kontrol</Link>
                    <Link className="nav-item nav-link active" to="/yonetici">Yönetici</Link>
                    <Link className="nav-item nav-link active" to="/hakem">Hakem</Link>
                </div>
            </div>
        </nav >
    );
}

export default Topbar;