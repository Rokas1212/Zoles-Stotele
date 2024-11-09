import React from "react";
import "./Navbar.css"; // Custom CSS file for additional styling

const Navbar: React.FC = () => {
  return (
    <nav className="navbar navbar-expand-lg navbar-light bg-light">
      <a className="navbar-brand" href="/">
        ŽS
      </a>
      <button
        className="navbar-toggler"
        type="button"
        data-toggle="collapse"
        data-target="#navbarNav"
        aria-controls="navbarNav"
        aria-expanded="false"
        aria-label="Toggle navigation"
      >
        <span className="navbar-toggler-icon"></span>
      </button>
      <div className="collapse navbar-collapse" id="navbarNav">
        <ul className="navbar-nav ml-auto">
          <li className="nav-item">
            <a className="nav-link" href="/prisijungimas">
              Prisijungimas
            </a>
          </li>
          <li className="nav-item">
            <a className="nav-link" href="/registracija">
              Registracija
            </a>
          </li>
          <li className="nav-item">
            <a className="nav-link" href="/profilis">
              Profilis
            </a>
          </li>
        </ul>
      </div>
    </nav>
  );
};

export default Navbar;
