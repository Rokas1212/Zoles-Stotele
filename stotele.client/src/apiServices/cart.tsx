const apiUrl = "https://localhost:5120/api/krepselio";

export interface CartItem {
    productId: string;
    name: string;
    price: number;
    quantity: number;
}

export async function fetchCart() {
    const response = await fetch(`${apiUrl}`, {
        credentials: "include",
    });
    return response.json();
}

export async function addToCart(item: { productId: string; name: string; quantity: number; price: number }) {
    await fetch(`${apiUrl}/add`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(item),
        credentials: "include",
    });
}

export async function removeFromCart(productId: string) {
    await fetch(`${apiUrl}/remove`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(productId),
        credentials: "include",
    });
}

export async function clearCart() {
    await fetch(`${apiUrl}/clear`, {
        method: "POST",
        credentials: "include",
    });
}