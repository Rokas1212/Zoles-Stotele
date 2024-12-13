const apiUrl = "https://localhost:5210/api/krepselio";

export interface CartItem {
    id: string;
    pavadinimas: string;
    kaina: number;
    kiekis: number;
}

export async function fetchCart() {
    const response = await fetch(`${apiUrl}`, {
        credentials: "include",
    });
    return response.json();
}

export async function addToCart(id: string) {
    await fetch("https://localhost:5210/api/krepselio/add", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ id: id.toString() }),
      credentials: "include", // Include session cookie
    });
  }

export async function removeFromCart(id: string) {
    await fetch(`${apiUrl}/remove`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(id),
        credentials: "include",
    });
}

export async function clearCart() {
    await fetch(`${apiUrl}/clear`, {
        method: "POST",
        credentials: "include",
    });
}

export async function createOrder(cartItems: CartItem[]) {
    const response = await fetch("https://localhost:5210/api/uzsakymu/sukurti-uzsakyma", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${localStorage.getItem("token")}`
        },
        body: JSON.stringify(cartItems),
        credentials: "include",
    });

    if (!response.ok) {
        throw new Error("Nepavyko sukurti užsakymo.");
    }

    return await response.json();
}