import React from "react";

const MegstamosKategorijos = () => {
  const categories = ["Trąšos", "Sėklos", "Pjovimo įrankiai", "Laistymo sistemos"];

  return (
    <div>
      <h1>Megstamos kategorijos</h1>
      <ul>
        {categories.map((category, index) => (
          <li key={index}>
            {category}
            <button style={{ marginLeft: "10px" }}>Pašalinti</button>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default MegstamosKategorijos;
