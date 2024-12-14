import { useEffect, useState, useRef } from "react";
import { useParams, useNavigate } from "react-router-dom";
import QRCodeGenerator from "../../components/qrgeneration";
import axios from "axios";

const Uzsakymas = () => {
  const { orderId } = useParams();
  const [order, setOrder] = useState<any>(null);
  const [loading, setLoading] = useState<boolean>(true); 
  const [error, setError] = useState<string | null>(null); 
  const [isPaid, setIsPaid] = useState<boolean>(false); 
  const navigate = useNavigate();
  const pollingIntervalRef = useRef<NodeJS.Timeout | null>(null);

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

  const checkIfPaid = async () => {
    try {
      const response = await axios.get(`https://localhost:5210/api/apmokejimu/is-paid`, {
        params: { orderId },
      });
      setIsPaid(response.data);
      console.log("Is paid:", response.data);

      if (response.data || (order && order.suma !== order.originalSuma)) {
        stopPolling();
      }
    } catch (error) {
      console.error("Klaida:", error);
    }
  };

  const startPolling = () => {
    if (!pollingIntervalRef.current) {
      pollingIntervalRef.current = setInterval(() => {
        fetchOrder();
        checkIfPaid();
      }, 1000);
    }
  };

  const stopPolling = () => {
    if (pollingIntervalRef.current) {
      clearInterval(pollingIntervalRef.current);
      pollingIntervalRef.current = null;
    }
  };

  useEffect(() => {
    fetchOrder();
    checkIfPaid();
    startPolling();

    return () => stopPolling();
  }, [orderId]);

  const handleOrderUpdated = (updatedOrder: any) => {
    setOrder(updatedOrder);
    if (updatedOrder.suma !== order?.suma) {
      stopPolling();
    }
  };

  if (loading) {
    return <div>Kraunama užsakymo informacija...</div>;
  }

  if (error) {
    return <div>{error}</div>;
  }

  if (!order) {
    return <div>Užsakymo informacija nerasta.</div>;
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
            const originalPrice = item.preke.kaina; // Original price from the product
            const discountedPrice = item.kaina ?? originalPrice; // Fallback to original price if `kaina` is null
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

      {!isPaid ? (
        <>
          <div className="mt-4">
            <QRCodeGenerator orderId={order.id} onOrderUpdated={handleOrderUpdated} />
          </div>
          <button
            onClick={() => navigate(`/apmokejimas/${order?.id}`)}
            className="btn btn-primary mt-3"
          >
            Patvirtinti užsakymą
          </button>
        </>
      ) : (
        <button className="btn btn-danger mt-3">Atšaukti užsakymą</button>
      )}
    </div>
  );
};

export default Uzsakymas;
