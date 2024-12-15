import React, { useState, useEffect } from "react";
import axios from "axios";
import "./LoyaltyPoints.css";

interface LoyaltyPointsProps {
  orderId: number;
  totalOrderPrice: number;
  onOrderUpdated: (updatedOrder: any) => void;
  onPointsUpdated: (remainingPoints: number) => void;
}

const LoyaltyPoints: React.FC<LoyaltyPointsProps> = ({
  orderId,
  totalOrderPrice,
  onOrderUpdated,
  onPointsUpdated,
}) => {
  const [pointsToUse, setPointsToUse] = useState<string>(""); // Start as empty string
  const [error, setError] = useState<string | null>(null);
  const [usedPoints, setUsedPoints] = useState<number | null>(null);
  const [remainingPoints, setRemainingPoints] = useState<number | null>(null);

  const maxPointsAllowed = Math.floor(totalOrderPrice * 10 * 0.2);

  useEffect(() => {
    const fetchUsedPoints = async () => {
      try {
        const user = localStorage.getItem("user");
        const userId = user ? JSON.parse(user).id : null;

        if (!userId) {
          setError("User not authenticated");
          return;
        }

        const response = await axios.get(
          `https://localhost:5210/api/Taskai/GetUsedPointsForOrder`,
          {
            params: { userId, orderId },
            headers: {
              Authorization: `Bearer ${localStorage.getItem("token")}`,
            },
          }
        );

        const pointsUsed = response.data.usedPoints;
        if (pointsUsed > 0) {
          setUsedPoints(pointsUsed);
        }
      } catch (err: any) {
        console.error(err?.response?.data || err.message);
      }
    };

    fetchUsedPoints();
  }, [orderId]);

  const handleInputChange = (value: string) => {
    // Remove leading zeros
    const sanitizedValue = value.replace(/^0+/, "");

    if (/^\d*$/.test(sanitizedValue)) {
      // Allow only numbers and no leading zeros
      const numberValue = Number(sanitizedValue);

      if (numberValue > maxPointsAllowed) {
        setError(`Maksimalus taškų kiekis yra ${maxPointsAllowed}.`);
        setPointsToUse(String(maxPointsAllowed));
      } else {
        setPointsToUse(sanitizedValue);
        setError(null);
      }
    }
  };

  const handleApplyPoints = async () => {
    try {
      const user = localStorage.getItem("user");
      const userId = user ? JSON.parse(user).id : null;

      if (!userId) {
        setError("User not authenticated");
        return;
      }

      const pointsNumber = Number(pointsToUse);
      if (pointsNumber <= 0) {
        setError("Taškų kiekis turi būti didesnis nei 0.");
        return;
      }

      const response = await axios.post(
        `https://localhost:5210/api/Taskai/UsePoints`,
        {
          userId,
          orderId,
          pointsToUse: pointsNumber,
        },
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );

      const updatedOrder = response.data.updatedOrder;
      const remainingPointsAfterUse = response.data.remainingPoints;

      onOrderUpdated(updatedOrder);
      onPointsUpdated(remainingPointsAfterUse);

      setUsedPoints(pointsNumber);
      setRemainingPoints(remainingPointsAfterUse);
      setPointsToUse("");
      setError(null);
    } catch (err: any) {
      console.error(err?.response?.data || err.message);
      setError(err?.response?.data?.message || "Nepavyko panaudoti taškų.");
    }
  };

  if (usedPoints) {
    return (
      <div className="loyalty-points-container">
        <h4>Lojalumo taškai</h4>
        <p className="success-message">
          Panaudota {usedPoints} taškų (€{(usedPoints / 10).toFixed(2)})
        </p>
      </div>
    );
  }

  return (
    <div className="loyalty-points-container">
      <h4>Lojalumo taškai</h4>
      <p className="max-points">
        Maksimalus taškų kiekis: {maxPointsAllowed} taškų (€{(maxPointsAllowed / 10).toFixed(2)})
      </p>
      <p>Naudojama suma: €{(Number(pointsToUse) / 10 || 0).toFixed(2)}</p>
      <div className="input-group">
        <input
          type="number"
          className="form-control points-input"
          placeholder="Įveskite taškų skaičių"
          value={pointsToUse}
          onChange={(e) => handleInputChange(e.target.value)}
          onBlur={() => {
            if (Number(pointsToUse) > maxPointsAllowed) {
              setPointsToUse(String(maxPointsAllowed));
              setError(`Maksimalus taškų kiekis yra ${maxPointsAllowed}.`);
            } else {
              setError(null);
            }
          }}
        />
        <button
          className="btn apply-points-btn"
          onClick={handleApplyPoints}
          disabled={!pointsToUse || Number(pointsToUse) <= 0}
        >
          Pritaikyti taškus
        </button>
      </div>
      {error && <p className="error-message">{error}</p>}
    </div>
  );
};

export default LoyaltyPoints;
