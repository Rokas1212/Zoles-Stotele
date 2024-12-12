import React from "react";
import { Navbar, Nav, NavDropdown } from "react-bootstrap";
import { useNavigate } from "react-router-dom"; // React Router hook for navigation
import "./Navbar.css";
import "bootstrap/dist/css/bootstrap.min.css";
import useAuth from "../hooks/useAuth";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

const CustomNavbar: React.FC = () => {
  const { isAuthenticated, user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    toast.success("Atsijungta", {
      position: "bottom-center",
      autoClose: 3000,
      hideProgressBar: true,
      closeOnClick: true,
      pauseOnHover: true,
      draggable: true,
    });

    setTimeout(() => {
      navigate("/"); 
    }, 1000); 
  };

  return (
    <>
      <Navbar bg="light" expand="lg" className="mb-3">
        <Navbar.Brand href="/">ŽS</Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
          <Nav className="ml-auto">
            {!isAuthenticated ? (
              <>
                <Nav.Link href="/prisijungimas">Prisijungimas</Nav.Link>
                <Nav.Link href="/registracija">Registracija</Nav.Link>
              </>
            ) : (
              <>
                <Nav.Link href={`/profilis?id=${user?.id || ""}`}>Profilis</Nav.Link>
                <Nav.Link onClick={handleLogout}>Atsijungti</Nav.Link>

                {user?.administratorius ? (
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
                ) : null}
              </>
            )}

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

      <ToastContainer />
    </>
  );
};

export default CustomNavbar;
