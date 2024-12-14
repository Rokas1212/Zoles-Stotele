import React, { useState, useEffect } from "react";

type Product = {
  id: number;
  kaina: number;
  pavadinimas: string;
  kodas: number;
  galiojimoData: string;
  kiekis: number;
  ismatavimai: string;
  nuotraukosUrl: string;
  garantinisLaikotarpis: string;
  aprasymas: string;
  rekomendacijosSvoris: number;
  mase: number;
  vadybininkasId: number;
  vadybininkas: string | null;
};

const Pagrindinis = () => {
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
        `https://localhost:5210/api/Rekomendacija/rekomendacijos/${userId}`
      );
      const data = await response.json();
      setProducts(data.prekes);
    } catch (error) {
      console.error("Klaida gaunant rekomenduojamas prekes", error);
    }
  };

  const handleBlockRecommendation = async (productId: number) => {
    if (userId === null) {
      console.log("User ID not found.");
      return;
    }

    try {
      const response = await fetch(
        `https://localhost:5210/api/Rekomendacija/blokuotos-rekomendacijos/uzblokuoti/${userId}/${productId}`,
        {
          method: "PUT",
        }
      );

      if (response.ok) {
        setRefetch((prev) => !prev);
        alert("Rekomendacija sėkmingai užblokuota");
      } else {
        alert("Nepavyko užblokuoti rekomendacijos");
      }
    } catch (error) {
      console.error("Klaida blokuojant rekomendaciją", error);
    }
  };

  useEffect(() => {
    if (userId !== null) {
      fetchData();
    }
  }, [userId, refetch]);

  return (
    <div>
      <h1>Pagrindinis</h1>
      <ul className="list-group">
        <li className="list-group-item">
          <a href="/krepselis" className="nav-link">
            Krepšelis
          </a>
        </li>
        <li className="list-group-item">
          <a href="/nuolaidos" className="nav-link">
            Nuolaidos
          </a>
        </li>
        <li className="list-group-item">
          <a href="/uzsakymai" className="nav-link">
            Užsakymai
          </a>
        </li>
        <li className="list-group-item">
          <a href="/" className="nav-link">
            Redaguoti profilį
          </a>
        </li>
        <li className="list-group-item">
          <a href="/prekes" className="nav-link">
            Prekės
          </a>
        </li>
        <li className="list-group-item">
          <a href="/prekiu-kategorijos" className="nav-link">
            Visos kategorijos
          </a>
        </li>
      </ul>

      <h2>Rekomenduojamos prekės</h2>
      <div>
        {products.map((product, index) => (
          <div
            key={index}
            style={{
              display: "flex",
              alignItems: "center",
              marginBottom: "10px",
            }}
          >
            <img
              src={product.nuotraukosUrl}
              alt={product.pavadinimas}
              width="30%"
              height="30%"
            />
            <div style={{ marginLeft: "10px" }}>
              <p>Pavadinimas: {product.pavadinimas}</p>
              <p>Kaina: {product.kaina}</p>
              <button
                onClick={() => handleBlockRecommendation(product.id)}
                style={{ marginTop: "5px" }}
              >
                Blokuoti rekomendaciją
              </button>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Pagrindinis;
