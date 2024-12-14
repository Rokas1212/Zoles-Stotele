import React, { useState } from "react";
import axios from "axios";
import "./qrgeneration.css";

interface QRCodeGeneratorProps {
  orderId: number;
}

const QRCodeGenerator: React.FC<QRCodeGeneratorProps> = ({ orderId }) => {
  const [qrCodeUrl, setQrCodeUrl] = useState<string | null>(null);
  const [updatedOrder, setUpdatedOrder] = useState<any | null>(null);

  const generateQRCode = async () => {
    try {
      const response = await axios.post(
        "https://localhost:5210/api/Taskai/GenerateQR",
        { orderId }
      );
      setQrCodeUrl(response.data.qrCodeUrl);
      console.log("QR code generated successfully:", response.data.qrCodeUrl);
    } catch (error) {
      console.error("Failed to generate QR code:", error);
    }
  };

  const handleScanQR = async () => {
    try {
      await axios.post(
        `https://localhost:5210/api/Taskai/ApplyDiscounts?orderId=${orderId}`
      );

      const response = await axios.get(
        `https://localhost:5210/api/uzsakymu/uzsakymas/${orderId}`
      );
      setUpdatedOrder(response.data);
      console.log("Updated order fetched successfully:", response.data);
    } catch (error: any) {
      console.error(
        "Failed to apply discounts or fetch updated order:",
        error.response?.data || error.message
      );
    }
  };

  return (
    <div className="qr-container">
      <button className="qr-btn" onClick={generateQRCode}>
        Generate QR Code
      </button>
      {qrCodeUrl && (
        <div className="qr-display">
          <img className="qr-image" src={qrCodeUrl} alt="Order QR Code" />
          <p>Scan the QR code to apply discounts.</p>
          <button className="qr-btn scan-btn" onClick={handleScanQR}>
            Simulate QR Scan
          </button>
        </div>
      )}
      {updatedOrder && (
        <div className="updated-order">
          <h3>Updated Order:</h3>
          <table className="order-table">
            <thead>
              <tr>
                <th>Prekė</th>
                <th>Kaina</th>
                <th>Kiekis</th>
                <th>Iš viso</th>
              </tr>
            </thead>
            <tbody>
              {updatedOrder.prekesUzsakymai.map((item: any) => (
                <tr key={item.id}>
                  <td>{item.preke.pavadinimas}</td>
                  <td>€{item.preke.kaina.toFixed(2)}</td>
                  <td>{item.kiekis}</td>
                  <td>€{(item.preke.kaina * item.kiekis).toFixed(2)}</td>
                </tr>
              ))}
            </tbody>
          </table>
          <h3 className="total-sum">
            Bendra suma: €{updatedOrder.suma.toFixed(2)}
          </h3>
        </div>
      )}
    </div>
  );
};

export default QRCodeGenerator;
