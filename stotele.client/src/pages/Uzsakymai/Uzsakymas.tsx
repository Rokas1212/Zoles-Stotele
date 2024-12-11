import { useEffect, useState } from "react";
import { useParams, Link, useNavigate } from "react-router-dom";

const Uzsakymas = () => {
  const { orderId } = useParams(); // Get the orderId from the URL
  const [order, setOrder] = useState<any>(null); // State to store the order details
  const [loading, setLoading] = useState<boolean>(true); // Loading state
  const [error, setError] = useState<string | null>(null); // Error state
  const navigate = useNavigate();

  useEffect(() => {
    const fetchOrder = async () => {
      try {
        const response = await fetch(`https://localhost:5210/api/uzsakymu/uzsakymas/${orderId}`, {
          credentials: "include",
        });
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

    fetchOrder();
  }, [orderId]);

  if (loading) {
    return <div>Kraunama užsakymo informacija...</div>;
  }

  if (error) {
    return <div>{error}</div>;
  }

  return (
    <div>
      <h1>Užsakymo ID: {order.id}</h1>
      <p>Data: {new Date(order.data).toLocaleDateString()}</p>
      <p>Bendra suma: €{order.suma.toFixed(2)}</p>
      <h2>Prekės</h2>
      <table className="table">
        <thead>
          <tr>
            <th>Prekė</th>
            <th>Kaina</th>
            <th>Kiekis</th>
            <th>Iš viso</th>
          </tr>
        </thead>
        <tbody>
          {order.prekesUzsakymai.map((item: any) => (
            <tr key={item.id}>
              <td>{item.preke.pavadinimas}</td>
              <td>€{item.preke.kaina.toFixed(2)}</td>
              <td>{item.kiekis}</td>
              <td>€{(item.preke.kaina * item.kiekis).toFixed(2)}</td>
            </tr>
          ))}
        </tbody>
      </table>
      <button onClick={() => navigate(`/apmokejimas/${order?.id}`)} className="btn btn-primary">
        Patvirtinti užsakymą
      </button>
    </div>
  );
};

export default Uzsakymas;