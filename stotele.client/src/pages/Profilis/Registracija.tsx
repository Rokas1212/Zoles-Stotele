import React, { useState } from "react";
import axios from "axios";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./registracija.css";
import "rc-slider/assets/index.css";
import Slider from "rc-slider";

const RegistracijosLangas = () => {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [gender, setGender] = useState("Vyras");
  const [city, setCity] = useState("");
  const [address, setAddress] = useState("");
  const [postalCode, setPostalCode] = useState("");
  const [birthDate, setBirthDate] = useState("");
  const [errorMessage, setErrorMessage] = useState("");

  const resetForm = () => {
    setUsername("");
    setEmail("");
    setPassword("");
    setFirstName("");
    setLastName("");
    setGender("Vyras");
    setCity("");
    setAddress("");
    setPostalCode("");
    setBirthDate("");
    setErrorMessage("");
  };

  const validateForm = () => {
    if (
      !username.trim() ||
      !email.trim() ||
      !password.trim() ||
      !firstName.trim() ||
      !lastName.trim() ||
      !city.trim() ||
      !address.trim() ||
      !postalCode.trim() ||
      !birthDate
    ) {
      setErrorMessage("Visi laukeliai yra privalomi.");
      return false;
    }

    if (!email.includes("@") || !email.includes(".")) {
      setErrorMessage("Nurodykite teisingą el. pašto adresą.");
      return false;
    }

    if (isNaN(parseInt(postalCode, 10))) {
      setErrorMessage("Pašto kodas turi būti skaičius.");
      return false;
    }

    setErrorMessage("");
    return true;
  };

  const handleRegistration = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    const registrationData = {
      lytis: gender,
      elektroninisPastas: email.trim(),
      slaptazodis: password.trim(),
      vardas: firstName.trim(),
      slapyvardis: username.trim(),
      pavarde: lastName.trim(),
      miestas: city.trim(),
      adresas: address.trim(),
      pastoKodas: parseInt(postalCode, 10),
      gimimoData: new Date(birthDate),
    };

    try {
      await axios.post("/api/Profilis/register", registrationData);

      // Show success toast message
      toast.success("Sėkminga registracija!");

      resetForm();

      // Optionally, redirect after success
      setTimeout(() => {
        window.location.href = "/";
      }, 250);
    } catch (error: any) {
      const backendMessage =
        error.response?.data?.message || "Nežinoma klaida iš serverio.";
      setErrorMessage(`Registracija nepavyko: ${backendMessage}`);
    }
  };
  const [genderValue, setGenderValue] = useState(0.5);

  const handleSliderChange = (value: any) => {
    setGenderValue(value);
    setGender(value >= 0.5 ? "Vyras" : "Moteris");
  };

  return (
    <div className="registration-container">
      <h1 className="registration-header">Registracija</h1>
      {errorMessage && <div className="error-message">{errorMessage}</div>}
      <form onSubmit={handleRegistration} className="registration-form">
        <div className="form-row">
          <div className="form-group">
            <label>Naudotojo vardas:</label>
            <input
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
              placeholder="Įveskite norimą naudotojo vardą"
            />
          </div>
          <div className="form-group">
            <label>El. paštas:</label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="Įveskite savo el. paštą"
              required
            />
          </div>
        </div>
        <div className="form-row">
          <div className="form-group">
            <label>Slaptažodis:</label>
            <input
              type="password"
              value={password}
              onChange={(e) => {
                setPassword(e.target.value);
              }}
              placeholder="Įveskite slaptažodį"
              required
            />
          </div>
          <div className="form-group">
            <label>Vardas:</label>
            <input
              type="text"
              value={firstName}
              onChange={(e) => {
                const regex = /^[a-zA-ZÀ-ž\s]*$/;
                if (regex.test(e.target.value)) {
                  setFirstName(e.target.value);
                }
              }}
              placeholder="Įveskite vardą"
              required
            />
          </div>
        </div>
        <div className="form-row">
          <div className="form-group">
            <label>Pavardė:</label>
            <input
              type="text"
              value={lastName}
              onChange={(e) => {
                const regex = /^[a-zA-ZÀ-ž\s]*$/;
                if (regex.test(e.target.value)) {
                  setLastName(e.target.value);
                }
              }}
              placeholder="Įveskite pavardę"
              required
            />
          </div>
          <div className="form-group">
            <label>Lytis:</label>
            <Slider
              min={0}
              max={1}
              step={0.01}
              value={genderValue}
              onChange={handleSliderChange}
            />
            <div className="text-center mt-2">
              <strong>{gender}</strong>
            </div>
          </div>
        </div>
        <div className="form-row">
          <div className="form-group">
            <label>Miestas:</label>
            <input
              type="text"
              value={city}
              onChange={(e) => {
                const regex = /^[a-zA-ZÀ-ž\s]*$/;
                if (regex.test(e.target.value)) {
                  setCity(e.target.value);
                }
              }}
              placeholder="Įveskite miesto pavadinimą"
              required
            />
          </div>

          <div className="form-group">
            <label>Adresas:</label>
            <input
              type="text"
              value={address}
              placeholder="Įveskite adresą"
              onChange={(e) => setAddress(e.target.value)}
              required
            />
          </div>
        </div>
        <div className="form-row">
          <div className="form-group">
            <label>Pašto kodas:</label>
            <input
              type="text"
              value={postalCode}
              onChange={(e) => {
                const regex = /^\d{0,5}$/; // Allows up to 5 digits
                if (regex.test(e.target.value)) {
                  setPostalCode(e.target.value); // Update only if it matches the regex
                }
              }}
              placeholder="12345"
              required
            />
            {/* Inline validation message */}
            {postalCode && !/^\d{5}$/.test(postalCode) && (
              <small className="text-danger">
                Pašto kodas privalo būti 5 skaičių formate.
              </small>
            )}
          </div>
          <div className="form-group">
            <label>Gimimo data:</label>
            <input
              type="date"
              value={birthDate}
              onChange={(e) => {
                const selectedDate = new Date(e.target.value);
                const today = new Date();
                const oneYearAgo = new Date(
                  today.getFullYear() - 1,
                  today.getMonth(),
                  today.getDate()
                );

                if (selectedDate > oneYearAgo) {
                  alert("Gimimo data turi būti bent prieš vienerius metus.");
                } else {
                  setBirthDate(e.target.value);
                }
              }}
              required
            />
          </div>
        </div>
        <button type="submit" className="registration-button">
          Registruotis
        </button>
      </form>
      <ToastContainer />
    </div>
  );
};

export default RegistracijosLangas;
