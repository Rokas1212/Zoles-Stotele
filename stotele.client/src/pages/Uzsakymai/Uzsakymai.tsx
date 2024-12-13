import React, { useEffect, useState } from "react";

const Uzsakymai = () => {
  const [orders, setOrders] = useState<any[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchOrders = async () => {
      try {
        const response = await fetch("https://localhost:5210/api/uzsakymu/uzsakymai", {
          credentials: "include",
          headers: {
            "Authorization": `Bearer ${localStorage.getItem("token")}`
          }
        });

        if (!response.ok) {
          throw new Error("Nepavyko gauti užsakymų.");
        }

        const data = await response.json();
        setOrders(data);
      } catch (error) {
        console.error("Klaida:", error);
        setError("Nepavyko gauti užsakymų.");
      } finally {
        setLoading(false);
      }
    };

    fetchOrders();
  }, []);

  if (loading) {
    return <div>Kraunama...</div>;
  }

  if (error) {
    return <div>{error}</div>;
  }

  return (
    <div>
      <h1>Uzsakymai</h1>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>Uzsakymo numeris</th>
            <th>Uzsakymo data</th>
            <th>Uzsakymo busena</th>
            <th>Uzsakymo suma</th>
          </tr>
        </thead>
        <tbody>
          {orders.map((order) => (
            <tr key={order.id}>
              <td>
                <a href={`/uzsakymas?id=${order.id}`}>{order.id}</a>
              </td>
              <td>{new Date(order.data).toLocaleDateString()}</td>
              <td>{order.busena}</td>
              <td>€{order.suma.toFixed(2)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default Uzsakymai;