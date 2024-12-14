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
  const [pvmMoketojoKodas, setPvmMoketojoKodas] = useState<string>(''); // Added for PVM Moketojo Kodas

  useEffect(() => {
    const fetchOrder = async () => {
      try {
        const response = await fetch(`https://localhost:5210/api/uzsakymu/uzsakymas/${orderId}`, {
          credentials: "include",
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
      } catch (error) {
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
    if (!pvmMoketojoKodas) {
      alert('Prašome įvesti PVM Mokėtojo Kodą.');
      return;
    }

    const stripe = await stripePromise;

    if (!stripe) {
      console.error('Stripe failed to initialize.');
      return;
    }

    try {
      const response = await axios.post(`https://localhost:5210/api/apmokejimu/create-checkout-session/${orderId}&${pvmMoketojoKodas}`);
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

  const handleCashPayment = () => {
    alert('Pasirinkote mokėti grynais, apmokėti galėsite atvykus kurjeriui.');
  };

  const handleBankTransfer = () => {
    alert('Apmokėjimo instrukcijos :).');
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
            <label htmlFor="pvmMoketojoKodas" style={{ display: 'block', marginBottom: '5px' }}>
              Įveskite PVM Mokėtojo Kodą:
            </label>
            <input
              id="pvmMoketojoKodas"
              type="text"
              value={pvmMoketojoKodas}
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