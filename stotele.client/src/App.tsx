import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Navbar from "./components/navbar"; // Import the Navbar component
import Pagrindinis from "./pages/Pagrindinis";
import Prisijungimas from "./pages/Prisijungimas";
import Registracija from "./pages/Registracija";
// Import other pages as needed

function App() {
  return (
    <Router>
      <Navbar />
      <Routes>
        <Route path="/" element={<Pagrindinis />} />
        <Route path="/prisijungimas" element={<Prisijungimas />} />
        <Route path="/registracija" element={<Registracija />} />
        {/* Add more routes */}
      </Routes>
    </Router>
  );
}

export default App;
