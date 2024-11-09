import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Pagrindinis from "./pages/Pagrindinis";
import Prisijungimas from "./pages/Prisijungimas";
import Registracija from "./pages/Registracija";
// Importuokite kitus puslapius pagal poreikį

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Pagrindinis />} />
        <Route path="/prisijungimas" element={<Prisijungimas />} />
        <Route path="/registracija" element={<Registracija />} />
        {/* Pridėkite daugiau maršrutų */}
      </Routes>
    </Router>
  );
}

export default App;
