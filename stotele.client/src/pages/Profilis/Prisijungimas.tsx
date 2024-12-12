import React, { useState } from "react";
import axios from "axios";
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
      // Send POST request to login API
      const response = await axios.post("https://localhost:5210/api/Profilis/login", loginData);

      console.log("Login successful:", response.data);

      // Store user information or token in local storage (if needed)
      localStorage.setItem("user", JSON.stringify(response.data));

      // Redirect to the home page after successful login
      window.location.href = "/";
    } catch (error: any) {
      setErrorMessage("Prisijungimas nepavyko: " + (error.response?.data || error.message || "Klaida"));
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
            required
          />
        </div>
        <div className="form-group">
          <label>Slaptažodis:</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        <button type="submit" className="login-button">Prisijungti</button>
      </form>
      <a href="/registracija" className="register-link">Registracija</a>
    </div>
  );
};

export default PrisijungimoLangas;
