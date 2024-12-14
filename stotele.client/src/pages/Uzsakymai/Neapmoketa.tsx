import React from 'react';
import { useNavigate } from 'react-router-dom';

const Cancel: React.FC = () => {
  const navigate = useNavigate();

  const handleRetryPayment = () => {
    navigate('/uzsakymai');
  };

  return (
    <div style={{ textAlign: 'center', marginTop: '50px' }}>
      <h1>Apmokėjimas neįvyko</h1>
      <p>Norint grįžti prie užsakymų, spauskite mygtuką</p>
      <button
        onClick={handleRetryPayment}
        style={{
          padding: '10px 20px',
          fontSize: '16px',
          backgroundColor: '#ff4d4f',
          color: '#fff',
          border: 'none',
          borderRadius: '4px',
          cursor: 'pointer',
        }}
      >
        Užsakymai
      </button>
    </div>
  );
};

export default Cancel;