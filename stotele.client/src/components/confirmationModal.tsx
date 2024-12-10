import React from "react";
import "./confirmationModal.css";

interface ConfirmationModalProps {
  message: string;
  onConfirm: () => void;
  onCancel: () => void;
}

const ConfirmationModal: React.FC<ConfirmationModalProps> = ({
  message,
  onConfirm,
  onCancel,
}) => {
  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <p>{message}</p>
        <div className="modal-actions">
          <button className="confirm-btn" onClick={onConfirm}>
            Patvirtinti
          </button>
          <button className="cancel-btn" onClick={onCancel}>
            Atšaukti
          </button>
        </div>
      </div>
    </div>
  );
};

export default ConfirmationModal;
