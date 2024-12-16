import React, { useState } from "react";
import axios from "axios";
import { toast, ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./prisijungimas.css";

const PrisijungimoLangas = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [errorMessage, setErrorMessage] = useState("");

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();

    const loginData = {
      slapyvardis: username,
      slaptazodis: password,
    };

    try {
      const response = await axios.post(
        "https://localhost:5210/api/Profilis/login",
        loginData
      );
      console.log("Login response:", response.data);

      localStorage.setItem("token", response.data.token);
      localStorage.setItem(
        "user",
        JSON.stringify({
          id: response.data.id,
          vardas: response.data.vardas,
          pavarde: response.data.pavarde,
          elektroninisPastas: response.data.elektroninisPastas,
          administratorius: response.data.administratorius,
        })
      );

      toast.success("Sėkmingai prisijungėte", {
        autoClose: 1000,
        hideProgressBar: true,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
      });

      setTimeout(() => {
        window.location.href = "/";
      }, 250);
    } catch (error: any) {
      console.error("Login error:", error);
      setErrorMessage(
        "Prisijungimas nepavyko: " +
          (error.response?.data || error.message || "Klaida")
      );

      toast.error("Prisijungimas nepavyko", {
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
      });
    }
  };

  return (
    <div className="login-container">
      <h1 className="login-header">Prisijungimas</h1>
      {errorMessage && <div className="error-message">{errorMessage}</div>}
      <form onSubmit={handleLogin} className="login-form">
        <div className="form-group">
          <label>Naudotojo vardas:</label>
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder="Įveskite savo naudotojo vardą"
            required
          />
        </div>
        <div className="form-group">
          <label>Slaptažodis:</label>
          <input
            type="password"
            value={password}
            placeholder="Įveskite savo slaptažodį"
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        <button type="submit" className="login-button">
          Prisijungti
        </button>
      </form>
      <a href="/registracija" className="register-link">
        Registracija
      </a>
      <ToastContainer />
    </div>
  );
};

export default PrisijungimoLangas;
