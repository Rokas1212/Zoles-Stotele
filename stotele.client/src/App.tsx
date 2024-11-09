import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
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

function App() {
  return (
    <Router>
      <Navbar />
      <Routes>
        <Route path="/" element={<Pagrindinis />} />
        <Route path="/profilis" element={<Profilis />} />
        <Route path="/prisijungimas" element={<Prisijungimas />} />
        <Route path="/registracija" element={<Registracija />} />
        <Route path="/uzsakymai" element={<Uzsakymai />} />
        <Route path="/uzsakymas" element={<Uzsakymas />} />
        <Route path="/krepselis" element={<Krepselis />} />
        <Route path="/apmokejimas" element={<Apmokejimas />} />
        <Route path="/nuolaida" element={<Nuolaida />} />
        <Route path="/nuolaidos" element={<Nuolaidos />} />
        <Route path="/administratorius/profiliai" element={<Profiliai />} />
        <Route
          path="/administratorius/prideti-nuolaida"
          element={<PridetiNuolaida />}
        />
        <Route path="/preke" element={<Preke />} />
        <Route path="/prekes" element={<Prekes />} />
        <Route path="/prekiu-kategorijos" element={<PrekiuKategorijos />} />
        <Route path="/redaguoti-profilį" element={<RedaguotiProfili />} />
        <Route
          path="/blokuotos-rekomendacijos"
          element={<BlokuotosRekomendacijos />}
        />
        <Route
          path="/megstamos-kategorijos"
          element={<MegstamosKategorijos />}
        />
        <Route
          path="vadybininkas/prideti-kategorija"
          element={<PridetiKategorija />}
        />
        <Route path="vadybininkas/prideti-preke" element={<PridetiPreke />} />
      </Routes>
    </Router>
  );
}

export default App;
