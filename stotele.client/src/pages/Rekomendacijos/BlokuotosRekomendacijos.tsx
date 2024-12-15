import React, { useState, useEffect } from "react";

type Product = {
  id: number;
  klientasId: number;
  prekeId: number;
  nuotraukosUrl: string;
  pavadinimas: string;
  aprasymas: string;
};

const BlokuotosRekomendacijos = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const [userId, setUserId] = useState<number | null>(null);
  const [refetch, setRefetch] = useState<boolean>(false);

  useEffect(() => {
    const user = localStorage.getItem("user");

    if (user) {
      const userObject = JSON.parse(user);
      setUserId(userObject.id);
    } else {
      console.log("Naudotojas neprisijungęs");
    }
  }, []);

  const fetchData = async () => {
    if (userId === null) {
      console.log("User ID not found.");
      return;
    }

    try {
      const response = await fetch(
        `https://localhost:5210/api/Rekomendacija/blokuotos-rekomendacijos/${userId}`
      );
      const data = await response.json();
      const sortedProducts = data.sort((a: Product, b: Product) => a.id - b.id);
      setProducts(sortedProducts);
    } catch (error) {
      console.error("Klaida gaunant uzblokuotas prekes", error);
    }
  };

  const handleUnblockRecommendation = async (productId: number) => {
    if (userId === null) {
      console.log("User ID not found.");
      return;
    }

    const isConfirmed = window.confirm(
      "Ar tikrai norite atblokuoti šią rekomendaciją?"
    );

    if (!isConfirmed) {
      return;
    }

    try {
      const response = await fetch(
        `https://localhost:5210/api/Rekomendacija/blokuotos-rekomendacijos/atblokuoti/${userId}/${productId}`,
        {
          method: "DELETE",
        }
      );

      if (response.ok) {
        setRefetch((prev) => !prev);
        alert("Rekomendacija sėkmingai atblokuota");
      } else {
        alert("Nepavyko atblokuoti rekomendacijos");
      }
    } catch (error) {
      console.error("Klaida atblokuojant rekomendaciją", error);
    }
  };

  useEffect(() => {
    if (userId !== null) {
      fetchData();
    }
  }, [userId, refetch]);

  return (
    <div>
      <h1>Blokuotos rekomendacijos</h1>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>ID</th>
            <th>Prekė</th>
            <th>Aprasymas</th>
            <th>Veiksmas</th>
          </tr>
        </thead>
        <tbody>
          {products.map((item, index) => (
            <tr key={item.id}>
              <td>{index + 1}</td>
              <td>{item.pavadinimas}</td>
              <td>{item.aprasymas}</td>
              <td>
                <button
                  onClick={() => handleUnblockRecommendation(item.prekeId)}
                >
                  Pašalinti
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default BlokuotosRekomendacijos;
