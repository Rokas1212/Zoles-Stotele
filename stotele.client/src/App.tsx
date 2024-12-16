import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";
import Navbar from "./components/navbar";
import Pagrindinis from "./pages/Pagrindinis";
import Prisijungimas from "./pages/Profilis/Prisijungimas";
import Registracija from "./pages/Profilis/Registracija";
import Apmokejimas from "./pages/Uzsakymai/Apmokejimas";
import Uzsakymas from "./pages/Uzsakymai/Uzsakymas";
import Uzsakymai from "./pages/Uzsakymai/Uzsakymai";
import Krepselis from "./pages/Uzsakymai/Krepselis";
import Profilis from "./pages/Profilis/Profilis";
import Nuolaida from "./pages/Lojalumas/Nuolaida";
import Nuolaidos from "./pages/Lojalumas/Nuolaidos";
import PridetiNuolaida from "./pages/Lojalumas/PridetiNuolaida";
import Preke from "./pages/Prekes/Preke";
import Prekes from "./pages/Prekes/Prekes";
import PrekiuKategorijos from "./pages/Prekes/PrekiuKategorijos";
import RedaguotiProfili from "./pages/Profilis/RedaguotiProfili";
import BlokuotosRekomendacijos from "./pages/Rekomendacijos/BlokuotosRekomendacijos";
import MegstamosKategorijos from "./pages/Rekomendacijos/MegstamosKategorijos";
import PridetiPreke from "./pages/Prekes/PridetiPreke";
import PridetiKategorija from "./pages/Prekes/PridetiKategorija";
import Profiliai from "./pages/Profilis/Profiliai";
import RedaguotiPreke from "./pages/Prekes/RedaguotiPreke";
import Cancel from "./pages/Uzsakymai/Neapmoketa";
import Success from "./pages/Uzsakymai/Apmoketa";

const isAuthenticated = () => {
  return !!localStorage.getItem("token");
};

function App() {
  return (
    <Router>
      <Navbar />
      <Routes>
        <Route path="/prisijungimas" element={<Prisijungimas />} />
        <Route path="/registracija" element={<Registracija />} />
        <Route
          path="/"
          element={
            isAuthenticated() ? (
              <Pagrindinis />
            ) : (
              <Navigate to="/prisijungimas" />
            )
          }
        />
        <Route
          path="/success/"
          element={
            isAuthenticated() ? <Success /> : <Navigate to="/prisijungimas" />
          }
        />
        <Route
          path="/cancel"
          element={
            isAuthenticated() ? <Cancel /> : <Navigate to="/prisijungimas" />
          }
        />
        <Route
          path="/profilis"
          element={
            isAuthenticated() ? <Profilis /> : <Navigate to="/prisijungimas" />
          }
        />
        <Route
          path="/uzsakymai"
          element={
            isAuthenticated() ? <Uzsakymai /> : <Navigate to="/prisijungimas" />
          }
        />
        <Route
          path="/uzsakymas/:orderId"
          element={
            isAuthenticated() ? <Uzsakymas /> : <Navigate to="/prisijungimas" />
          }
        />
        <Route
          path="/krepselis"
          element={
            isAuthenticated() ? <Krepselis /> : <Navigate to="/prisijungimas" />
          }
        />
        <Route
          path="/apmokejimas/:orderId"
          element={
            isAuthenticated() ? (
              <Apmokejimas />
            ) : (
              <Navigate to="/prisijungimas" />
            )
          }
        />
        <Route
          path="/nuolaida"
          element={
            isAuthenticated() ? <Nuolaida /> : <Navigate to="/prisijungimas" />
          }
        />
        <Route
          path="/nuolaidos"
          element={
            isAuthenticated() ? <Nuolaidos /> : <Navigate to="/prisijungimas" />
          }
        />
        <Route
          path="/administratorius/profiliai"
          element={
            isAuthenticated() ? <Profiliai /> : <Navigate to="/prisijungimas" />
          }
        />
        <Route
          path="/administratorius/prideti-nuolaida"
          element={
            isAuthenticated() ? (
              <PridetiNuolaida />
            ) : (
              <Navigate to="/prisijungimas" />
            )
          }
        />
        <Route
          path="/preke"
          element={
            isAuthenticated() ? <Preke /> : <Navigate to="/prisijungimas" />
          }
        />
        <Route
          path="/prekes"
          element={
            isAuthenticated() ? <Prekes /> : <Navigate to="/prisijungimas" />
          }
        />
        <Route
          path="/prekiu-kategorijos"
          element={
            isAuthenticated() ? (
              <PrekiuKategorijos />
            ) : (
              <Navigate to="/prisijungimas" />
            )
          }
        />
        <Route
          path="/redaguoti-profili"
          element={
            isAuthenticated() ? (
              <RedaguotiProfili />
            ) : (
              <Navigate to="/prisijungimas" />
            )
          }
        />
        <Route
          path="/redaguoti-preke"
          element={
            isAuthenticated() ? (
              <RedaguotiPreke />
            ) : (
              <Navigate to="/prisijungimas" />
            )
          }
        />
        <Route
          path="/blokuotos-rekomendacijos"
          element={
            isAuthenticated() ? (
              <BlokuotosRekomendacijos />
            ) : (
              <Navigate to="/prisijungimas" />
            )
          }
        />
        <Route
          path="/megstamos-kategorijos"
          element={
            isAuthenticated() ? (
              <MegstamosKategorijos />
            ) : (
              <Navigate to="/prisijungimas" />
            )
          }
        />
        <Route
          path="vadybininkas/prideti-kategorija"
          element={
            isAuthenticated() ? (
              <PridetiKategorija />
            ) : (
              <Navigate to="/prisijungimas" />
            )
          }
        />
        <Route
          path="vadybininkas/prideti-preke"
          element={
            isAuthenticated() ? (
              <PridetiPreke />
            ) : (
              <Navigate to="/prisijungimas" />
            )
          }
        />
      </Routes>
    </Router>
  );
}

export default App;
