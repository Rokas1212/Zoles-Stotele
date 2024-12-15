import { useState, useEffect } from "react";
import { loadStripe } from "@stripe/stripe-js";
import axios from "axios";
import { useNavigate, useParams } from "react-router-dom";
import Loading from "../../components/loading";

// Load your Stripe publishable key
const stripePromise = loadStripe(
  "pk_test_51BrUMKBUtGUmzKJGptzSuF8yfO5pzm83qFs6We6l7ZM7l3ko9vmbrwBdkCNlBeBWzIyMQpcmVsoslM3pFTBm7HJw00f9qd9h0J"
);

const Apmokejimas = () => {
  const { orderId } = useParams();
  const [order, setOrder] = useState<any>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [isPaid, setIsPaid] = useState<boolean>(false);
  const [PvmMoketojoKodas, setPvmMoketojoKodas] = useState<string>(""); // Added for PVM Moketojo Kodas
  const navigate = useNavigate();
  const [selectedMethod, setSelectedMethod] = useState<string>("card");
  const [validationError, setValidationError] = useState<string | null>(null);

  useEffect(() => {
    const fetchOrder = async () => {
      try {
        const response = await fetch(
          `https://localhost:5210/api/uzsakymu/uzsakymas/${orderId}`,
          {
            credentials: "include",
            headers: {
              Authorization: `Bearer ${localStorage.getItem("token")}`,
            },
          }
        );
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
        const response = await axios.get(
          `https://localhost:5210/api/apmokejimu/is-paid`,
          {
            params: { orderId },
          }
        );
        setIsPaid(response.data);
        console.log("Is paid:", response.data);
      } catch (error: any) {
        if (error.response.status === 404) {
          return;
        }
        console.error("Klaida:", error);
      }
    };

    checkIfPaid();
    fetchOrder();
  }, [orderId]);

  if (loading) {
    return <Loading />;
  }

  if (error) {
    return <div>{error}</div>;
  }
  const validatePvmKodas = () => {
    const regex = /^\d{11}$/; // 11-digit number validation
    if (!regex.test(PvmMoketojoKodas)) {
      setValidationError("PVM Mokėtojo Kodas turi būti 11 skaitmenų skaičius.");
      return false;
    }
    setValidationError(null);
    return true;
  };

  const handlePayment = () => {
    if (!validatePvmKodas()) {
      return;
    }

    if (selectedMethod === "card") {
      handleCheckout();
    } else if (selectedMethod === "cash") {
      handleCashPayment();
    } else if (selectedMethod === "bank") {
      handleBankTransfer();
    }
  };

  const handleCheckout = async () => {
    const stripe = await stripePromise;

    if (!stripe) {
      console.error("Stripe klaida.");
      return;
    }

    try {
      const response = await axios.post(
        `https://localhost:5210/api/apmokejimu/create-checkout-session/`,
        null,
        {
          params: { orderId, PvmMoketojoKodas },
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );
      const sessionId = response.data.sessionId;

      // Redirect to the Stripe Checkout page
      const { error } = await stripe.redirectToCheckout({
        sessionId,
      });

      if (error) {
        console.error("Klaida:", error.message);
      }
    } catch (error) {
      console.error("Klaida:", error);
    }
  };

  const handleCashPayment = async () => {
    try {
      const response = await axios.post(
        `https://localhost:5210/api/apmokejimu/create-checkout-cash`,
        null,
        {
          params: { orderId, PvmMoketojoKodas },
          headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
        }
      );

      if (response.status === 200) {
        setIsPaid(true);
        alert(
          "Pasirinkote mokėti grynais, apmokėti galėsite atvykus kurjeriui. Ačiū!"
        );
      }
    } catch (error) {
      console.error("Klaida:", error);
    }
  };
  const handleBankTransfer = () => {
    axios.post(
      `https://localhost:5210/api/apmokejimu/create-checkout-transfer`,
      null,
      {
        params: { orderId, PvmMoketojoKodas },
        headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
      }
    );

    alert("Apmokėjimo instrukcijos išsiųstos į El. Paštą.");
    navigate(`/uzsakymas/${orderId}`);
  };

  return (
    <div className="container mt-5 d-flex justify-content-center">
      <div
        className="card shadow-sm p-4"
        style={{ maxWidth: "600px", width: "100%" }}
      >
        <h1 className="text-center mb-4">Apmokėjimas</h1>

        {isPaid ? (
          <div className="alert alert-success text-center">
            Užsakymas apmokėtas sėkmingai!
          </div>
        ) : (
          <>
            <p className="text-center mb-4">Pasirinkite apmokėjimo būdą:</p>

            {/* PVM Input with Label */}
            <div className="mb-4">
              <label htmlFor="PvmMoketojoKodas" className="form-label">
                Įveskite PVM Mokėtojo Kodą:
              </label>
              <input
                id="PvmMoketojoKodas"
                type="text"
                className={`form-control ${
                  validationError ? "is-invalid" : ""
                }`}
                value={PvmMoketojoKodas}
                onChange={(e) => setPvmMoketojoKodas(e.target.value)}
                placeholder="12345678901"
              />
              {validationError && (
                <div className="invalid-feedback">{validationError}</div>
              )}
            </div>

            {/* Radio Buttons for Payment Method */}
            <div className="mb-4">
              <div className="form-check">
                <input
                  className="form-check-input"
                  type="radio"
                  name="paymentMethod"
                  id="card"
                  value="card"
                  onChange={() => setSelectedMethod("card")}
                  defaultChecked
                />
                <label className="form-check-label" htmlFor="card">
                  Mokėti Kortele
                </label>
              </div>
              <div className="form-check">
                <input
                  className="form-check-input"
                  type="radio"
                  name="paymentMethod"
                  id="cash"
                  value="cash"
                  onChange={() => setSelectedMethod("cash")}
                />
                <label className="form-check-label" htmlFor="cash">
                  Mokėti Grynaisiais
                </label>
              </div>
              <div className="form-check">
                <input
                  className="form-check-input"
                  type="radio"
                  name="paymentMethod"
                  id="bank"
                  value="bank"
                  onChange={() => setSelectedMethod("bank")}
                />
                <label className="form-check-label" htmlFor="bank">
                  Mokėti Banko Pavedimu
                </label>
              </div>
            </div>

            {/* Single Payment Button */}
            <div className="text-center">
              <button className="btn btn-primary w-100" onClick={handlePayment}>
                Mokėti
              </button>
            </div>
          </>
        )}
      </div>
    </div>
  );
};
export default Apmokejimas;
