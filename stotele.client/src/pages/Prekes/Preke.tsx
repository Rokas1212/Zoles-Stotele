import React, { useEffect, useState } from "react";
import axios from "axios";
import { useSearchParams } from "react-router-dom";
import "./preke.css";
import Loading from "../../components/loading";
import useAuth from "../../hooks/useAuth";
import MapComponent from "../../components/mapComponent";

interface Preke {
  id: number;
  pavadinimas: string;
  kaina: number;
  aprasymas: string;
  nuotraukosUrl: string;
}

const Preke: React.FC = () => {
  const { user } = useAuth(); // Access the authenticated user
  const [product, setProduct] = useState<Preke | null>(null);
  const [addresses, setAddresses] = useState<string[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>(true);

  const [searchParams] = useSearchParams();
  const id = searchParams.get("id");

  useEffect(() => {
    const fetchPreke = async () => {
      if (!id) {
        setError("Nenurodytas prekės ID.");
        setLoading(false);
        return;
      }

      try {
        const response = await axios.get<Preke>(
          `https://localhost:5210/api/Preke/${id}`
        );
        setProduct(response.data);
      } catch (err: any) {
        setError(
          err.response?.data || "Nepavyko gauti prekės informacijos iš serverio."
        );
      }
    };

    const fetchAddresses = async () => {
      if (!id) return;

      try {
        const response = await axios.get<string[]>(
          `https://localhost:5210/api/Preke/${id}/addresses`
        );
        setAddresses(response.data);
      } catch (err: any) {
        console.error("Klaida gaunant parduotuvių adresus:", err.response?.data);
      }
    };

    setLoading(true);
    Promise.all([fetchPreke(), fetchAddresses()]).finally(() => {
      setLoading(false);
    });
  }, [id]);

  if (loading) {
    return <Loading />;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  if (!product) {
    return <div className="error">Prekė nerasta.</div>;
  }

  return (
    <div className="preke-container">
      <h1 className="preke-title">{product.pavadinimas}</h1>
      <img
        src={product.nuotraukosUrl}
        alt={product.pavadinimas}
        className="preke-image"
      />
      <p className="preke-price">
        <strong>Kaina:</strong> €{product.kaina.toFixed(2)}
      </p>
      <p className="preke-description">{product.aprasymas}</p>

      {/* Render Redaguoti button only for admin users */}
      {user?.administratorius && (
        <a href={`/redaguoti-preke?id=${product.id}`}>
          <button className="edit-button">Redaguoti</button>
        </a>
      )}

      <h2>Parduotuvės</h2>
      {addresses.length > 0 ? (
        <MapComponent addresses={addresses} />
      ) : (
        <p>Šios prekės nėra parduotuvių sandėliuose.</p>
      )}
    </div>
  );
};

export default Preke;