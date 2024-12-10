import React from "react";
import "bootstrap/dist/css/bootstrap.min.css";
import "./loading.css";

const Loading: React.FC = () => {
  return (
    <div className="loading-container">
      <div className="spinner-wrapper">
        <div className="spinner-border large-spinner" role="status"></div>
        <span className="loading-text">Žolės stotelė </span>
      </div>
    </div>
  );
};

export default Loading;
