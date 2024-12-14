import React, { useEffect, useState } from "react";
import Loading from "../../components/loading";

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
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        });

        if (!response.ok) {
          throw new Error("Nepavyko gauti užsakymų.");
        }

        const data = await response.json();
        console.log("Fetched orders:", data);
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
    return <Loading />;
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
            <th>Užsakymo numeris</th>
            <th>Užsakymo data</th>
            <th>Užsakymo būsena</th>
            <th>Suma</th>
          </tr>
        </thead>
        <tbody>
          {orders.map((item, index) => {
            const order = item.order;
            const payment = item.payments?.[0]; // Get the first payment or undefined
            return (
              <tr key={order.id || index}>
                <td>
                  <a href={`/uzsakymas/${order.id}`}>{order.id}</a>
                </td>
                <td>{new Date(order.data).toLocaleDateString()}</td>
                <td>{payment?.mokejimoStatusas || "Nepasirinktas mokėjimo būdas"}</td>
                <td>€{order.suma.toFixed(2)}</td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </div>
  );
};

export default Uzsakymai;