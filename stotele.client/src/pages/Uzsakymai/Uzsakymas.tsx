import { useEffect, useState, useRef } from "react";
import { useParams, useNavigate } from "react-router-dom";
import QRCodeGenerator from "../../components/qrgeneration";
import axios from "axios";
import Loading from "../../components/loading";

const Uzsakymas = () => {
  const { orderId } = useParams(); // Get the orderId from the URL
  const [order, setOrder] = useState<any>(null); // State to store the order details
  const [loading, setLoading] = useState<boolean>(true); // Loading state
  const [error, setError] = useState<string | null>(null); // Error state
  const [isPaid, setIsPaid] = useState<boolean>(false); // State to store if the order is paid
  const [isConfirmed, setIsConfirmed] = useState<boolean>(false); // State to store if the order is confirmed
  const navigate = useNavigate();
  const pollingIntervalRef = useRef<NodeJS.Timeout | null>(null);
  const previousOrderRef = useRef<any>(null);

  // Function to fetch order details
  const fetchOrder = async () => {
    try {
      const response = await fetch(`https://localhost:5210/api/uzsakymu/uzsakymas/${orderId}`, {
        credentials: "include",
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      });
      if (!response.ok) {
        throw new Error("Nepavyko gauti užsakymo informacijos.");
      }
      const data = await response.json();
      setOrder(data);
      if (data) {
        previousOrderRef.current = data;
      }
    } catch (err) {
      console.error("Klaida:", err);
      setError("Nepavyko gauti užsakymo informacijos.");
    } finally {
      setLoading(false);
    }
  };

  // Function to check if order is paid
  const checkIfPaid = async () => {
    try {
      const response = await axios.get(`https://localhost:5210/api/apmokejimu/is-paid`, {
        params: { orderId }, 
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      });
      setIsPaid(response.data);
      console.log('Is paid:', response.data);
    } catch (error) {
      console.error('Klaida:', error);
    }
  };

  // Function to check if order is confirmed
  const checkIfConfirmed = async () => {
    try {
      const response = await axios.get(`https://localhost:5210/api/uzsakymu/is-confirmed`, {
        params: { orderId }, 
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      });
      setIsConfirmed(response.data);
      console.log('Is confirmed:', response.data);
    } catch (error) {
      console.error('Klaida:', error);
    }
  };

  // Start polling mechanism
  const startPolling = () => {
    pollingIntervalRef.current = setInterval(() => {
      checkIfPaid();
      checkIfConfirmed();
    }, 30000); // Poll every 30 seconds
  };

  // Stop polling mechanism
  const stopPolling = () => {
    if (pollingIntervalRef.current) {
      clearInterval(pollingIntervalRef.current);
    }
  };

  useEffect(() => {
    fetchOrder();
    checkIfPaid();
    checkIfConfirmed();
    startPolling();

    return () => stopPolling();
  }, [orderId]);

  const handleBackToCart = () => {
    // Delete the created order
    axios.delete(`https://localhost:5210/api/uzsakymu/uzsakymas/${orderId}`, {
      withCredentials: true,
    });
    navigate("/krepselis");
  };

  const handleOrderUpdated = async () => {
    // Re-fetch the order after discounts are applied
    await fetchOrder();
  };

  if (loading) {
    return <Loading />;
  }

  if (error) {
    return <div>{error}</div>;
  }

  const displayOrder = order || previousOrderRef.current;

  if (!displayOrder) {
    return <div>Užsakymo informacija nerasta.</div>;
  }

  const handleConfirmOrder = async () => {
    try {
      const response = await axios.put(`https://localhost:5210/api/uzsakymu/confirm-order/`, null, {
        params: { orderId },
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      });
      console.log('Order confirmed:', response.data);
      setIsConfirmed(true);
      navigate(`/apmokejimas/${displayOrder?.id}`);
    } catch (error) {
      console.error('Klaida:', error);
    }
  };

  return (
    <div className="container mt-4">
      <h1>Užsakymo ID: {displayOrder.id}</h1>
      <p>Data: {new Date(displayOrder.data).toLocaleDateString()}</p>
      <h3 className="mt-3">
        Bendra suma: <span className="text-success">€{displayOrder.suma.toFixed(2)}</span>
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
          {displayOrder.prekesUzsakymai.map((item: any) => {
            const originalPrice = item.preke.kaina;
            const discountedPrice = item.kaina ?? originalPrice;
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

      {!isPaid ? 
      <>
        <div className="mt-4">
          <QRCodeGenerator orderId={displayOrder.id} onOrderUpdated={handleOrderUpdated} />
        </div>
        <button
          onClick={handleBackToCart}
          className="btn btn-secondary mt-3"
        >
          Grįžti į krepšelį
        </button>

        {isConfirmed ? 
        <button
          className="btn btn-success mt-3"
          onClick={() => navigate(`/apmokejimas/${displayOrder?.id}`)}
        >
          Apmokėti
        </button>
        :
        <button
          onClick={handleConfirmOrder}
          className="btn btn-primary mt-3"
        >
          Patvirtinti užsakymą
        </button>
        }
      </>
      : 
      <button
        className="btn btn-danger mt-3"
      >
        Atšaukti užsakymą
      </button> 
      }
    </div>
  );
};

export default Uzsakymas;