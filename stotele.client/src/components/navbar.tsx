import React from "react";
import { Navbar, Nav, NavDropdown } from "react-bootstrap";
import "./Navbar.css";
import "bootstrap/dist/css/bootstrap.min.css";

const CustomNavbar: React.FC = () => {
  return (
    <Navbar bg="light" expand="lg" className="mb-3">
      <Navbar.Brand href="/">ŽS</Navbar.Brand>
      <Navbar.Toggle aria-controls="basic-navbar-nav" />
      <Navbar.Collapse id="basic-navbar-nav">
        <Nav className="ml-auto">
          <Nav.Link href="/prisijungimas">Prisijungimas</Nav.Link>
          <Nav.Link href="/registracija">Registracija</Nav.Link>
          <Nav.Link href="/profilis">Profilis</Nav.Link>

          <NavDropdown
            title="Administratoriaus Meniu"
            id="administratorius-dropdown"
          >
            <NavDropdown.Item href="/administratorius/profiliai">
              Profiliai
            </NavDropdown.Item>
            <NavDropdown.Item href="/administratorius/prideti-nuolaida">
              Pridėti nuolaidą
            </NavDropdown.Item>
          </NavDropdown>

          <NavDropdown title="Vadybininko Meniu" id="vadybininkas-dropdown">
            <NavDropdown.Item href="/vadybininkas/prideti-preke">
              Pridėti prekę
            </NavDropdown.Item>
            <NavDropdown.Item href="/vadybininkas/prideti-kategorija">
              Pridėti kategoriją
            </NavDropdown.Item>
          </NavDropdown>
        </Nav>
      </Navbar.Collapse>
    </Navbar>
  );
};

export default CustomNavbar;
