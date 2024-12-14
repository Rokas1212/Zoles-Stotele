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

    generateQRCode();
  }, [orderId]); 

  const handleScanQR = async () => {
    try {
      await axios.get(
        `https://localhost:5210/api/Taskai/ApplyDiscounts?orderId=${orderId}`
      );

      const response = await axios.get(
        `https://localhost:5210/api/uzsakymu/uzsakymas/${orderId}`
      );
  
      console.log("Updated order fetched successfully:", response.data);
      onOrderUpdated(response.data);
    } catch (error: any) {
      console.error(
        "Failed to apply discounts or fetch updated order:",
        error.response?.data || error.message
      );
    }
  };
  
  

  return (
    <div className="qr-container">
      {qrCodeUrl ? (
        <div className="qr-display mt-3" style={{ border: "1px solid #ddd", padding: "20px", borderRadius: "10px", backgroundColor: "#f8f9fa" }}>
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
