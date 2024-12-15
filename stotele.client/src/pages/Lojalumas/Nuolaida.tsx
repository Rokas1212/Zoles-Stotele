import React, { useState, useEffect } from "react";
import ConfirmationModal from "../../components/confirmationModal";
import useAuth from "../../hooks/useAuth"; 
import { toast, ToastContainer } from "react-toastify"; 
import "react-toastify/dist/ReactToastify.css"; 
import "./nuolaida.css";
import Loading from "../../components/loading";

interface INuolaida {
  id: number;
  procentai: number;
  galiojimoPabaiga: string;
  prekesPavadinimas: string;
  prekesKaina: number;
  prekesKainaPoNuolaidos: number;
}

const Nuolaida: React.FC = () => {
  const { user } = useAuth(); 
  const searchParams = new URLSearchParams(window.location.search);
  const id = searchParams.get("id");

  const [nuolaida, setNuolaida] = useState<INuolaida | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [showModal, setShowModal] = useState<boolean>(false);

  useEffect(() => {
    if (!id) {
      setError("Nuolaida ID is missing from the query parameters.");
      setLoading(false);
      return;
    }

    const fetchNuolaida = async () => {
      try {
        const response = await fetch(`https://localhost:5210/api/Nuolaida/${id}`);
        if (!response.ok) {
          throw new Error(
            `Failed to fetch nuolaida: ${response.status} - ${response.statusText}`
          );
        }

        const data: INuolaida = await response.json();
        setNuolaida(data);
      } catch (err: any) {
        console.error("Error fetching nuolaida:", err);
        setError(err.message || "An unknown error occurred");
      } finally {
        setLoading(false);
      }
    };

    fetchNuolaida();
  }, [id]);

  const handleDelete = async () => {
    try {
      const response = await fetch(`https://localhost:5210/api/Nuolaida/${id}`, {
        method: "DELETE",
      });

      if (!response.ok) {
        throw new Error(
          `Failed to delete nuolaida: ${response.status} - ${response.statusText}`
        );
      }

      toast.success("Nuolaida sėkmingai ištrinta!");
      setTimeout(() => {
        window.location.href = "/nuolaidos";
      }, 2000);
    } catch (err: any) {
      console.error("Error deleting nuolaida:", err);
      toast.error(err.message || "Nepavyko ištrinti nuolaidos. Bandykite dar kartą.");
    }
  };

  if (loading) {
    return <Loading />;
  }

  if (error) {
    return <div>Error: {error}</div>;
  }

  if (!nuolaida) {
    return <div>Nuolaida not found</div>;
  }

  return (
    <div className="nuolaida-container">
      <ToastContainer />
      <h1>Nuolaida</h1>
      <p>
        <strong>Nuolaidos kodas:</strong> {nuolaida.id}
      </p>
      <p>
        <strong>Nuolaidos dydis (%):</strong> {nuolaida.procentai}%
      </p>
      <p>
        <strong>Galiojimo pabaiga:</strong>{" "}
        <span
          className={
            new Date(nuolaida.galiojimoPabaiga) < new Date() ? "expired-date" : ""
          }
        >
          {new Date(nuolaida.galiojimoPabaiga).toLocaleDateString()} - nebegalioja
        </span>
      </p>
      <p>
        <strong>Prekės pavadinimas:</strong> {nuolaida.prekesPavadinimas}
      </p>
      <p>
        <strong>Prekės kaina (prieš nuolaidą):</strong> {nuolaida.prekesKaina.toFixed(2)} €
      </p>
      <p>
        <strong>Prekės kaina (po nuolaidos):</strong> {nuolaida.prekesKainaPoNuolaidos.toFixed(2)} €
      </p>

      {user?.administratorius && (
        <>
          <button className="delete-button" onClick={() => setShowModal(true)}>
            Ištrinti nuolaidą
          </button>
          {showModal && (
            <ConfirmationModal
              message="Ar tikrai norite ištrinti šią nuolaidą?"
              onConfirm={() => {
                setShowModal(false);
                handleDelete();
              }}
              onCancel={() => setShowModal(false)}
            />
          )}
        </>
      )}
    </div>
  );
};

export default Nuolaida;
