import React, { useState } from "react";
import axios from "axios";

interface LoyaltyPointsProps {
  orderId: number;
  onOrderUpdated: (updatedOrder: any) => void; // Callback to update the entire order
  onPointsUpdated: (remainingPoints: number) => void; // Callback to update remaining points
}

const LoyaltyPoints: React.FC<LoyaltyPointsProps> = ({
  orderId,
  onOrderUpdated,
  onPointsUpdated,
}) => {
  const [pointsToUse, setPointsToUse] = useState<number>(0);
  const [error, setError] = useState<string | null>(null);

  const handleApplyPoints = async () => {
    try {
      const user = localStorage.getItem("user");
      const userId = user ? JSON.parse(user).id : null;

      if (!userId) {
        setError("User not authenticated");
        return;
      }

      if (pointsToUse <= 0) {
        setError("Taškų kiekis turi būti didesnis nei 0.");
        return;
      }

      const response = await axios.post(
        `https://localhost:5210/api/Taskai/UsePoints`,
        {
          userId,
          orderId,
          pointsToUse,
        },
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );

      const updatedOrder = response.data.updatedOrder;
      const remainingPoints = response.data.remainingPoints; // Get remaining points from backend

      onOrderUpdated(updatedOrder); // Update the order
      onPointsUpdated(remainingPoints); // Update the points in parent
      setError(null);
    } catch (err: any) {
      console.error(err?.response?.data || err.message); // Log detailed error
      setError(err?.response?.data?.message || "Nepavyko panaudoti taškų.");
    }
  };

  return (
    <div className="loyalty-points-container mt-4">
      <h4>Lojalumo taškai</h4>
      <p>Naudojama suma: €{(pointsToUse / 10).toFixed(2)}</p>
      <div className="input-group mt-2">
        <input
          type="number"
          className="form-control"
          placeholder="Įveskite naudojamus taškus"
          value={pointsToUse}
          onChange={(e) => setPointsToUse(Number(e.target.value))}
        />
        <button className="btn btn-primary" onClick={handleApplyPoints}>
          Pritaikyti taškus
        </button>
      </div>
      {error && <p className="text-danger mt-2">{error}</p>}
    </div>
  );
};

export default LoyaltyPoints;
