import { useState, useEffect } from 'react';
import { loadStripe } from '@stripe/stripe-js';
import axios from 'axios';
import { useParams } from 'react-router-dom';

// Load your Stripe publishable key
const stripePromise = loadStripe('pk_test_51BrUMKBUtGUmzKJGptzSuF8yfO5pzm83qFs6We6l7ZM7l3ko9vmbrwBdkCNlBeBWzIyMQpcmVsoslM3pFTBm7HJw00f9qd9h0J');

const Apmokejimas = () => {
  const { orderId } = useParams();
  const [order, setOrder] = useState<any>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [isPaid, setIsPaid] = useState<boolean>(false);
  const [PvmMoketojoKodas, setPvmMoketojoKodas] = useState<string>(''); // Added for PVM Moketojo Kodas

  useEffect(() => {
    const fetchOrder = async () => {
      try {
        const response = await fetch(`https://localhost:5210/api/uzsakymu/uzsakymas/${orderId}`, {
          credentials: "include",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        });
        if (!response.ok) {
          throw new Error("Nepavyko gauti užsakymo informacijos.");
        }
        const data = await response.json();
        setOrder(data);
      } catch (error) {
        console.error("Klaida:", error);
        setError("Nepavyko gauti užsakymo informacijos.");
      } finally {
        setLoading(false);
      }
    };

    const checkIfPaid = async () => {
      try {
        const response = await axios.get(`https://localhost:5210/api/apmokejimu/is-paid`, {
          params: { orderId }, 
        });
        setIsPaid(response.data);
        console.log('Is paid:', response.data);
      } catch (error: any) {
        if (error.response.status === 404) {
          return;
        }
        console.error('Klaida:', error);
      }
    };

    checkIfPaid();
    fetchOrder();
  }, [orderId]);

  if (loading) {
    return <div>Kraunama apmokėjimo informacija...</div>;
  }

  if (error) {
    return <div>{error}</div>;
  }

  const handleCheckout = async () => {
    if (!PvmMoketojoKodas) {
      alert('Prašome įvesti PVM Mokėtojo Kodą.');
      return;
    }

    const stripe = await stripePromise;

    if (!stripe) {
      console.error('Stripe klaida.');
      return;
    }

    try {
      const response = await axios.post(`https://localhost:5210/api/apmokejimu/create-checkout-session/${orderId}&${PvmMoketojoKodas}`, {
        withCredentials: true,
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        }
      }
      );
      const sessionId = response.data.sessionId;

      // Redirect to the Stripe Checkout page
      const { error } = await stripe.redirectToCheckout({
        sessionId,
      });

      if (error) {
        console.error('Klaida:', error.message);
      }
    } catch (error) {
      console.error('Klaida:', error);
    }
  };

  const handleCashPayment = async () => {
    if (!PvmMoketojoKodas) {
      alert('Prašome įvesti PVM Mokėtojo Kodą.');
      return;
    }

    const confirmPayment = window.confirm('Ar tikrai norite apmokėti grynaisiais?');
    if (!confirmPayment) {
      return;
    }
  
    try {
      const response = await axios.post(`https://localhost:5210/api/apmokejimu/create-checkout-cash`, null, {
        params: { orderId, PvmMoketojoKodas },
        headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
      });
  
      if (response.status === 200) {
        setIsPaid(true);
        alert('Pasirinkote mokėti grynais, apmokėti galėsite atvykus kurjeriui. Ačiū!');
      }
    } catch (error) {
      console.error('Klaida:', error);
    }
  };
  const handleBankTransfer = () => {
    if (!PvmMoketojoKodas) {
      alert('Prašome įvesti PVM Mokėtojo Kodą.');
      return;
    }

    const confirmPayment = window.confirm('Ar tikrai norite apmokėti banko pavedimu?');
    if (!confirmPayment) {
      return;
    }

    axios.post(`https://localhost:5210/api/apmokejimu/create-checkout-transfer`, null, {
      params: { orderId, PvmMoketojoKodas },
      headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
    });

    alert('Apmokėjimo instrukcijos išsiųstos į El. Paštą.');
  };

  return (
    <div>
      <h1>Apmokėjimas</h1>
      {isPaid ? (
        <div style={{ color: 'green' }}>Užsakymas apmokėtas sėkmingai!</div>
      ) : (
        <>
          <p>Pasirinkite apmokėjimo būdą:</p>

          {/* Input field for PVM Mokėtojo Kodas */}
          <div style={{ marginBottom: '20px' }}>
            <label htmlFor="PvmMoketojoKodas" style={{ display: 'block', marginBottom: '5px' }}>
              Įveskite PVM Mokėtojo Kodą:
            </label>
            <input
              id="PvmMoketojoKodas"
              type="text"
              value={PvmMoketojoKodas}
              onChange={(e) => setPvmMoketojoKodas(e.target.value)}
              placeholder="123456789"
              style={{
                padding: '10px',
                fontSize: '16px',
                borderRadius: '4px',
                border: '1px solid #ccc',
                width: '100%',
                maxWidth: '300px',
              }}
            />
          </div>

          <div style={{ display: 'flex', gap: '10px' }}>
            {/* Button for card payment */}
            <button
              onClick={handleCheckout}
              style={{ padding: '10px 20px', fontSize: '16px', cursor: 'pointer' }}
            >
              Mokėti Kortele
            </button>

            {/* Button for cash payment */}
            <button
              onClick={handleCashPayment}
              style={{ padding: '10px 20px', fontSize: '16px', cursor: 'pointer' }}
            >
              Mokėti Grynaisias
            </button>

            {/* Button for bank transfer */}
            <button
              onClick={handleBankTransfer}
              style={{ padding: '10px 20px', fontSize: '16px', cursor: 'pointer' }}
            >
              Mokėti Banko Pavedimu
            </button>
          </div>
        </>
      )}
    </div>
  );
};

export default Apmokejimas;