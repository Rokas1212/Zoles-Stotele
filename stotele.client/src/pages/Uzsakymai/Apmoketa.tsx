import React from 'react';
import { useNavigate } from 'react-router-dom';

const Success: React.FC = () => {
  const navigate = useNavigate();

  const handleGoHome = () => {
    navigate('/'); // Navigate back to the home page or orders page
  };

  return (
    <div style={{ textAlign: 'center', marginTop: '50px' }}>
      <h1>Apmokėjimas Sėkmingas!</h1>
      <p>Ačiū, kad perkate!</p>
      <p>Sąskaitą gausite į El. Paštą</p>
      <button
        onClick={handleGoHome}
        style={{
          padding: '10px 20px',
          fontSize: '16px',
          backgroundColor: '#4caf50',
          color: '#fff',
          border: 'none',
          borderRadius: '4px',
          cursor: 'pointer',
        }}
      >
        Grįžti Į Parduotuvę
      </button>
    </div>
  );
};

export default Success;