import React, { useEffect, useState } from "react";
import { Navbar, Nav, NavDropdown } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import "./Navbar.css";
import "bootstrap/dist/css/bootstrap.min.css";
import useAuth from "../hooks/useAuth";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

const CustomNavbar: React.FC = () => {
  const { isAuthenticated, user, logout } = useAuth();
  const navigate = useNavigate();

  // State to track Vadybininkas status
  const [isVadybininkas, setIsVadybininkas] = useState<boolean>(false);

  useEffect(() => {
    const checkVadybininkasStatus = async () => {
      if (user?.id) {
        try {
          const response = await fetch(
            `https://localhost:5210/api/Profilis/is-vadybininkas/${user.id}`
          );
          setIsVadybininkas(response.ok); // Sets true if user is a vadybininkas
        } catch (error) {
          console.error("Error checking Vadybininkas status:", error);
        }
      }
    };

    if (isAuthenticated) {
      checkVadybininkasStatus();
    }
  }, [user, isAuthenticated]);

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

    window.location.reload();
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
                <Nav.Link href={`/profilis?id=${user?.id || ""}`}>
                  Profilis
                </Nav.Link>
                <Nav.Link onClick={handleLogout}>Atsijungti</Nav.Link>

                {user?.administratorius && (
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
                )}

                {/* Conditional display of Vadybininko Meniu */}
                {isVadybininkas && (
                  <NavDropdown
                    title="Vadybininko Meniu"
                    id="vadybininkas-dropdown"
                  >
                    <NavDropdown.Item href="/vadybininkas/prideti-preke">
                      Pridėti prekę
                    </NavDropdown.Item>
                    <NavDropdown.Item href="/vadybininkas/prideti-kategorija">
                      Pridėti kategoriją
                    </NavDropdown.Item>
                  </NavDropdown>
                )}
              </>
            )}
          </Nav>
        </Navbar.Collapse>
      </Navbar>

      <ToastContainer />
    </>
  );
};

export default CustomNavbar;
