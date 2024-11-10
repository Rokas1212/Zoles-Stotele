import React from "react";
import { useLocation } from "react-router-dom";

const Nuolaida = () => {
  const location = useLocation();
  const searchParams = new URLSearchParams(location.search);
  const id = searchParams.get("id");

  const discounts = [
    { id: "1", name: "Žolės šventės nuolaida", amount: 10, startDate: "2021-10-10", endDate: "2021-10-20" },
    { id: "2", name: "Black friday", amount: 20, startDate: "2021-11-11", endDate: "2021-11-31" },
  ];

  const discount = discounts.find((discount) => discount.id === id);

  if (!discount) {
    return <div>Nuolaida nerasta</div>;
  }

  return (
    <div>
      <h1>Nuolaida</h1>
      <p><strong>Nuolaidos kodas:</strong> {discount.id}</p>
      <p><strong>Pavadinimas:</strong> {discount.name}</p>
      <p><strong>Nuolaidos dydis:</strong> {discount.amount}</p>
      <p><strong>Galiojimo pradžia:</strong> {discount.startDate}</p>
      <p><strong>Galiojimo pabaiga:</strong> {discount.endDate}</p>
    </div>
  );
};

export default Nuolaida;
