import React from "react";

const Navbar: React.FC = () => {
  return (
    <nav>
      <a href="/prisijungimas">Prisijungimas</a>
      <a href="/registracija">Registracija</a>
      <a href="/profilis">Profilis</a>
    </nav>
  );
};

export default Navbar;
