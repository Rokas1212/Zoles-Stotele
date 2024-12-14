import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import QRCodeGenerator from "../../components/qrgeneration";

const Uzsakymas = () => {
  const { orderId } = useParams();
  const [order, setOrder] = useState<any>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  const fetchOrder = async () => {
    try {
      const response = await fetch(
        `https://localhost:5210/api/uzsakymu/uzsakymas/${orderId}`,
        { credentials: "include" }
      );
      if (!response.ok) {
        throw new Error("Nepavyko gauti užsakymo informacijos.");
      }
      const data = await response.json();
      setOrder(data);
    } catch (error) {
      console.error("Klaida:", error);
      setError("Nepavyko gauti užsakymo informacijos.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchOrder();
  }, [orderId]);

  const handleOrderUpdated = (updatedOrder: any) => {
    setOrder(updatedOrder); // Update the order state with the latest data
  };

  if (loading) {
    return <div>Kraunama užsakymo informacija...</div>;
  }

  if (error) {
    return <div>{error}</div>;
  }

  return (
    <div className="container mt-4">
      <h1>Užsakymo ID: {order.id}</h1>
      <p>Data: {new Date(order.data).toLocaleDateString()}</p>
      <h3 className="mt-3">
        Bendra suma: <span className="text-success">€{order.suma.toFixed(2)}</span>
      </h3>

      <h2 className="mt-4">Prekės</h2>
      <table className="table table-striped table-hover mt-3">
        <thead style={{ backgroundColor: "#198754", color: "#fff" }}>
          <tr>
            <th>Prekė</th>
            <th>Pradinė kaina</th>
            <th>Galutinė kaina</th>
            <th>Kiekis</th>
            <th>Iš viso</th>
            <th>Nuolaida</th>
          </tr>
        </thead>
        <tbody>
          {order.prekesUzsakymai.map((item: any) => {
            const originalPrice = item.preke.kaina;
            const discountedPrice = item.kaina;
            const quantity = item.kiekis;
            const totalLine = discountedPrice * quantity;

            const discountAmount = originalPrice - discountedPrice;
            const discountPercentage =
              discountAmount > 0
                ? Math.round((discountAmount / originalPrice) * 100)
                : 0;

            return (
              <tr key={item.id}>
                <td>{item.preke.pavadinimas}</td>
                <td>€{originalPrice.toFixed(2)}</td>
                <td>
                  {discountAmount > 0 ? (
                    <span className="text-success">€{discountedPrice.toFixed(2)}</span>
                  ) : (
                    <span>€{discountedPrice.toFixed(2)}</span>
                  )}
                </td>
                <td>{quantity}</td>
                <td>€{totalLine.toFixed(2)}</td>
                <td>
                  {discountAmount > 0 ? (
                    <span className="badge bg-success">-{discountPercentage}%</span>
                  ) : (
                    <span className="badge bg-secondary">Nėra nuolaidos</span>
                  )}
                </td>
              </tr>
            );
          })}
        </tbody>
      </table>

      <div className="mt-4">
        <QRCodeGenerator orderId={order.id} onOrderUpdated={handleOrderUpdated} />
      </div>

      <button
        onClick={() => navigate(`/apmokejimas/${order?.id}`)}
        className="btn btn-primary mt-3"
      >
        Patvirtinti užsakymą
      </button>
    </div>
  );
};

export default Uzsakymas;
