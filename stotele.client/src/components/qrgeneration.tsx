import React, { useState, useEffect } from "react";
import axios from "axios";
import "./qrgeneration.css";

interface QRCodeGeneratorProps {
  orderId: number;
  onOrderUpdated: (updatedOrder: any) => void;
}

const QRCodeGenerator: React.FC<QRCodeGeneratorProps> = ({
  orderId,
  onOrderUpdated,
}) => {
  const [qrCodeUrl, setQrCodeUrl] = useState<string | null>(null);

  useEffect(() => {
    const generateQRCode = async () => {
      try {
        const token = localStorage.getItem('token');
        if (!token) {
          console.error("No token found. Please log in first.");
          return;
        }

        const response = await axios.post(
          "https://localhost:5210/api/Taskai/GenerateQR",
          { orderId },
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        setQrCodeUrl(response.data.qrCodeUrl);
        console.log("QR code generated successfully:", response.data.qrCodeUrl);
      } catch (error) {
        console.error("Failed to generate QR code:", error);
      }
    };

    generateQRCode();
  }, [orderId]);

  const handleScanQR = async () => {
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        return; 
      }
  
      const user = localStorage.getItem('user');
      const userId = user ? JSON.parse(user).id : null;
      if (!userId) {
        return;
      }
  
      const response = await axios.get(
        `https://localhost:5210/api/Taskai/ApplyDiscounts`,
        {
          params: { orderId, userId },
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
  
      onOrderUpdated(response.data.UpdatedOrder);
    } catch (error: any) {
    }
  };
  

  return (
    <div className="qr-container">
      {qrCodeUrl ? (
        <div className="qr-display mt-3"
             style={{ border: "1px solid #ddd", padding: "20px", borderRadius: "10px", backgroundColor: "#f8f9fa" }}>
          <img
            className="qr-image"
            src={qrCodeUrl}
            alt="Order QR Code"
            style={{ width: "200px", height: "200px", border: "2px solid #198754", borderRadius: "8px" }}
          />
          <p className="mt-2" style={{ fontStyle: "italic" }}>
            Nuskenuokite QR kodą norint pritaikyti nuolaidas.
          </p>
          <button
            className="qr-btn btn btn-success"
            onClick={handleScanQR}
          >
            Simuliuoti QR nuskaitymą
          </button>
        </div>
      ) : (
        <p className="mt-3">QR kodas generuojamas...</p>
      )}
    </div>
  );
};

export default QRCodeGenerator;
